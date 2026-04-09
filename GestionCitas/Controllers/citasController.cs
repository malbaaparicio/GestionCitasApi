using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionCitas.Models;
using GestionCitas.DTOs;
using AutoMapper;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace GestionCitas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class citasController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public citasController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaGetDto>>> Getcitas([FromQuery]CitaGetFechasDto citaFechas)
        {

            // 1. Si no me pasan fechas, por defecto muestro el DIA ACTUAL (Protección de rendimiento)
            DateTime inicio = citaFechas.fecha_desde ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime fin = citaFechas.fecha_hasta ?? inicio;

            // 2. Ajustamos el fin para que incluya todo el último día (hasta las 23:59:59)
            // Usamos una variable local, no modificamos el parámetro de entrada
            DateTime finAjustado = new DateTime(fin.Year, fin.Month, fin.Day, 23, 59, 59);

            var citas = await _context.citas
                .Include(c => c.cliente)
                .Include(c => c.empleado)
                .Include(c => c.cita_servicios).ThenInclude(cs => cs.servicio) // Incluimos los servicios para el mapeo manual
                .Where(c => c.fecha_hora_inicio >= inicio && c.fecha_hora_inicio <= finAjustado)
                .OrderByDescending(c => c.fecha_hora_inicio)
                .ToListAsync();

            var citasDto = _mapper.Map<List<CitaGetDto>>(citas);
            return Ok(citasDto);

        }

        // GET: api/citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitaGetDto>> Getcita(int id)
        {
            var cita = await _context.citas                
                .FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }
            var citaDto = _mapper.Map<CitaGetDto>(cita);
            return Ok(citaDto);

            
        }

        // PUT: api/citas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<CitaGetDto>> Putcita(int id, CitaUpdateDto cita)
        {
            var citaExistente = await _context.citas
                .Include(c => c.cita_servicios)
                .FirstOrDefaultAsync(c => c.citaid == id);

            if (citaExistente == null)
            {
                return NotFound();
            }
            
            var serviciosDb = await _context.servicios
              .Where(z => cita.serviciosids.Contains(z.servicioid))
              .ToListAsync();

            //verificamos si existen todos
            if (serviciosDb.Count() != cita.serviciosids.Count())
            {
                return BadRequest("Uno o más servicios no existen");
            }

            // 2. CÁLCULOS DE NEGOCIO
            var duracionTotal = serviciosDb.Sum(s => s.duracion ?? 0); // Asumimos 0 si es null
            var precioTotal = serviciosDb.Sum(s => s.precio_actual ?? 0);

            var fechaInicio = cita.fecha_hora_inicio;
            var fechaFin = fechaInicio.AddMinutes(duracionTotal);

            // VALIDACIÓN DE AGENDA: Comprobar si el empleado ya está ocupado
            // Buscamos citas que se solapen con el intervalo deseado
            bool existeSolapamiento = await _context.citas
                .AnyAsync(c =>
                    c.empleadoid == (cita.empleadoid ?? citaExistente.empleadoid) && // Mismo empleado (nuevo o existente)
                    c.citaid != id &&               // Excluir la cita que estamos editando
                    c.estado != "Cancelada" &&            // Ignorar canceladas                        
                    fechaFin > c.fecha_hora_inicio && // La cita solicitada acaba después de que otra empiece
                    fechaInicio < c.fecha_hora_fin // La cita solicitada empieza antes de que otra acabe
                );

            if (existeSolapamiento)
            {
                return Conflict($"El empleado ya tiene una cita en ese horario ({fechaInicio} - {fechaFin}).");
            }
            citaExistente.clienteid = cita.clienteid ?? citaExistente.clienteid;
            citaExistente.empleadoid = cita.empleadoid ?? citaExistente.empleadoid;
            citaExistente.fecha_hora_inicio = fechaInicio;
            citaExistente.fecha_hora_fin = fechaFin;     // Calculado
            citaExistente.precio_total = precioTotal;    // Calculado
            citaExistente.precio_sugerido = precioTotal;    // Calculado
            citaExistente.observaciones = cita.observaciones;
            citaExistente.duracion_total = duracionTotal; // Calculado

            
            _context.cita_servicios.RemoveRange(citaExistente.cita_servicios);

            // Añadimos los nuevos servicios
            foreach (var servicio in serviciosDb)
            {
                var nuevoDetalle = new Cita_Servicio
                {
                    servicioid = servicio.servicioid,
                    // Importante: Volvemos a "congelar" el precio y duración actuales
                    precio_aplicado = servicio.precio_actual,
                    duracion = servicio.duracion,
                    // Vinculamos a la cita existente
                    citaid = citaExistente.citaid,
                    // OJO: EF Core a veces necesita ayuda con el NegocioId en entidades hijas insertadas manualmente
                    // aunque el SaveChanges lo intente arreglar, es bueno asignarlo si lo tienes a mano, 
                    // pero tu SaveChangesAsync ya se encarga de esto.
                };

                // Añadimos a la tabla directa o a la colección
                _context.cita_servicios.Add(nuevoDetalle);
            }

            try
            {
                // EF Core ejecutará los DELETE y luego los INSERT en una sola transacción
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!citaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/citas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CitaGetDto>> Postcita(CitaCreateDto cita)
        {
            var serviciosDb = await _context.servicios
                .Where(z => cita.serviciosids.Contains(z.servicioid))
                .ToListAsync();

            //verificamos si existen todos
            if (serviciosDb.Count() != cita.serviciosids.Count())
            {
                return BadRequest("Uno o más servicios no existen");
            }

            // 2. CÁLCULOS DE NEGOCIO
            var duracionTotal = serviciosDb.Sum(s => s.duracion ?? 0); // Asumimos 0 si es null
            var precioTotal = serviciosDb.Sum(s => s.precio_actual ?? 0);

            var fechaInicio = cita.fecha_hora_inicio;
            var fechaFin = fechaInicio.AddMinutes(duracionTotal);

            // VALIDACIÓN DE AGENDA: Comprobar si el empleado ya está ocupado
            // Buscamos citas que se solapen con el intervalo deseado
            bool existeSolapamiento = await _context.citas
                .AnyAsync(c =>
                    c.empleadoid == cita.empleadoid && // Mismo empleado
                    c.estado != "Cancelada" &&            // Ignorar canceladas                        
                    fechaFin > c.fecha_hora_inicio && // La cita solicitada acaba después de que otra empiece
                    fechaInicio < c.fecha_hora_fin // La cita solicitada empieza antes de que otra acabe
                );

            if (existeSolapamiento)
            {
                return Conflict($"El  empleado ya tiene una cita en ese horario ({fechaInicio} - {fechaFin}).");
            }

            // 3. CREACIÓN DE LA ENTIDAD MAESTRA (Cabecera)
            // Aquí NO usamos AutoMapper directo porque hay mucha lógica calculada
            var nuevaCita = new Cita
            {
                clienteid = cita.clienteid,
                empleadoid = cita.empleadoid,
                fecha_hora_inicio = fechaInicio,
                fecha_hora_fin = fechaFin,     // Calculado
                precio_sugerido = precioTotal,    // Calculado
                precio_total = precioTotal,    // Calculado
                observaciones = cita.observaciones,
                duracion_total = duracionTotal, // Calculado
                estado = "Pendiente" // Estado inicial por defecto
                // NegocioId se inyecta solo en SaveChangesAsync
            };

            // 4. CREACIÓN DE LOS DETALLES (Relación Cita_Servicio)
            foreach (var servicio in serviciosDb)
            {
                var detalle = new Cita_Servicio
                {
                    servicioid = servicio.servicioid,
                    // IMPORTANTE: Guardamos el precio "congelado" al momento de la cita
                    // Si el servicio cambia de precio mañana, esta cita histórica no cambia.
                    precio_aplicado = servicio.precio_actual,
                    duracion = servicio.duracion,

                    // Vinculamos a la cita padre (EF Core entiende esto automáticamente)
                    cita = nuevaCita
                };

                // Añadimos a la colección de la cita
                _context.cita_servicios.Add(detalle);
            }

            // 5. GUARDADO ATÓMICO
            // Primero añadimos la cita (que ya trae los detalles dentro gracias a la navegación)
            _context.citas.Add(nuevaCita);

            // Al guardar, EF Core hace todo: inserta Cita, obtiene ID, inserta Cita_Servicios con ese ID.
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<CitaGetDto>(nuevaCita);

            // Rellenamos manual lo que falta (porque nuevaCita.cliente es null al no haber hecho Include al guardar)
            // Opcional: Podrías hacer una query extra para traer nombres si es vital devolverlos en el Response del POST.

            return CreatedAtAction("GetCitas", new { id = nuevaCita.citaid }, resultDto);

        }

        // DELETE: api/citas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecita(int id)
        {
            var cita = await _context.citas
                .Include(c=>c.cita_servicios)
                .FirstOrDefaultAsync(c => c.citaid == id);

            if (cita == null)
            {
                return NotFound();
            }
            _context.cita_servicios.RemoveRange(cita.cita_servicios);
            _context.citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PUT: api/citas/5/cancelar
        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> CancelarCita(int id, CitaCancelacionDto citaCancelacionDto)
        {
            var cita = await _context.citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            if (cita.estado == "Cancelada")
            {
                return BadRequest("La cita ya está cancelada.");
            }
            cita.estado = "Cancelada";
            cita.observaciones = citaCancelacionDto.Observaciones;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool citaExists(int id)
        {
            return _context.citas.Any(e => e.citaid == id);
        }
    }
}

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

namespace GestionCitas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class precio_servicioController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public precio_servicioController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/precio_servicio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrecioServicioGetDto>>> Getprecio_servicios()
        {
            var servicios = await _context.precio_servicios.ToListAsync();
            var serviciosDto = _mapper.Map<List<PrecioServicioGetDto>>(servicios);
            return Ok(serviciosDto);
            
        }

        // GET: api/precio_servicio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrecioServicioGetDto>> Getprecio_servicio(int id)
        {
            var precio_servicio = await _context.precio_servicios.FindAsync(id);

            if (precio_servicio == null)
            {
                return NotFound();
            }
            var precioServicioDto = _mapper.Map<PrecioServicioGetDto>(precio_servicio);
            return Ok(precioServicioDto);
                        
        }

        // PUT: api/precio_servicio/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putprecio_servicio(int id, PrecioServicioCreateOrUpdateDto precio_servicio)
        {
            var precioServicioExistente = await _context.precio_servicios.FindAsync(id);
            if (precioServicioExistente == null)
            {
                return NotFound();
            }
            _mapper.Map(precio_servicio, precioServicioExistente);
                     

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!precio_servicioExists(id))
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

        // POST: api/precio_servicio
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrecioServicioGetDto>> Postprecio_servicio(PrecioServicioCreateOrUpdateDto precio_servicio)
        {
            var nuevoPrecioServicio = _mapper.Map<precio_servicio>(precio_servicio);

            _context.precio_servicios.Add(nuevoPrecioServicio);
            await _context.SaveChangesAsync();

            var precio_servicioDto = _mapper.Map<PrecioServicioGetDto>(nuevoPrecioServicio);

            return CreatedAtAction("Getprecio_servicio", new { id = nuevoPrecioServicio.precio_servicioid }, precio_servicioDto);
        }

        // DELETE: api/precio_servicio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteprecio_servicio(int id)
        {
            var precio_servicio = await _context.precio_servicios.FindAsync(id);
            if (precio_servicio == null)
            {
                return NotFound();
            }

            _context.precio_servicios.Remove(precio_servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool precio_servicioExists(int id)
        {
            return _context.precio_servicios.Any(e => e.precio_servicioid == id);
        }
    }
}

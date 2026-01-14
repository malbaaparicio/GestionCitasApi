using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionCitas.Models;
using AutoMapper;
using GestionCitas.DTOs;

namespace GestionCitas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class cita_servicioController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public cita_servicioController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Cita_Servicio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CitaServicioGetDto>>> Getcita_servicios()
        {
            var cita_servicios = await _context.cita_servicios.ToListAsync();
            var citaServiciosDto = _mapper.Map<List<CitaServicioGetDto>>(cita_servicios);
            return Ok(citaServiciosDto);
            
        }

        // GET: api/Cita_Servicio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitaServicioGetDto>> Getcita_servicio(int id)
        {
            var cita_servicio = await _context.cita_servicios.FindAsync(id);

            if (cita_servicio == null)
            {
                return NotFound();
            }
            var citaServicioDto = _mapper.Map<CitaServicioGetDto>(cita_servicio);

            return Ok(citaServicioDto);

            
        }

        // PUT: api/Cita_Servicio/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putcita_servicio(int id, CitaServicioCreateOrUpdateDto cita_servicio)
        {
            var citaServicioExistente = await _context.cita_servicios.FindAsync(id);
            if (citaServicioExistente == null)
            {
                return NotFound();
            }

            _mapper.Map(cita_servicio, citaServicioExistente);
                       

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!cita_servicioExists(id))
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

        // POST: api/Cita_Servicio
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CitaServicioGetDto>> Postcita_servicio(CitaServicioCreateOrUpdateDto cita_servicio)
        {
            var nuevaCitaServicio = _mapper.Map<Cita_Servicio>(cita_servicio);

            _context.cita_servicios.Add(nuevaCitaServicio);
            await _context.SaveChangesAsync();

            var cita_servicioDto = _mapper.Map<CitaServicioGetDto>(nuevaCitaServicio);

            return CreatedAtAction("Getcita_servicio", new { id = nuevaCitaServicio.cita_serviciosid }, cita_servicioDto);
        }

        // DELETE: api/Cita_Servicio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecita_servicio(int id)
        {
            var cita_servicio = await _context.cita_servicios.FindAsync(id);
            if (cita_servicio == null)
            {
                return NotFound();
            }

            _context.cita_servicios.Remove(cita_servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool cita_servicioExists(int id)
        {
            return _context.cita_servicios.Any(e => e.cita_serviciosid == id);
        }
    }
}

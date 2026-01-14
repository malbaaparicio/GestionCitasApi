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
        public async Task<ActionResult<IEnumerable<CitaGetDto>>> Getcitas()
        {
            var citas = await _context.citas.ToListAsync();
            var citasDto = _mapper.Map<List<CitaGetDto>>(citas);
            return Ok(citasDto);
            
        }

        // GET: api/citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CitaGetDto>> Getcita(int id)
        {
            var cita = await _context.citas.FindAsync(id);

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
        public async Task<IActionResult> Putcita(int id, CitaCreateOrUpdateDto cita)
        {
            var citaExistente = await _context.citas.FindAsync(id);
            if (citaExistente == null)
            {
                return NotFound();
            }
            
            _mapper.Map(cita, citaExistente);
                       

            try
            {
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
        public async Task<ActionResult<CitaGetDto>> Postcita(CitaCreateOrUpdateDto cita)
        {
            var nuevaCita = _mapper.Map<Cita>(cita);
            _context.citas.Add(nuevaCita);

            await _context.SaveChangesAsync();
            var citaDto = _mapper.Map<CitaGetDto>(nuevaCita);

            return CreatedAtAction("Getcita", new { id = nuevaCita.citaid }, citaDto);
        }

        // DELETE: api/citas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecita(int id)
        {
            var cita = await _context.citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }

            _context.citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool citaExists(int id)
        {
            return _context.citas.Any(e => e.citaid == id);
        }
    }
}

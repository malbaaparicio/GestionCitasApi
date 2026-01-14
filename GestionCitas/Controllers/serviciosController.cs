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
    public class serviciosController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public serviciosController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/servicios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServicioGetDto>>> Getservicios()
        {
            var servicios = await _context.servicios.ToListAsync();
            var serviciosDto = _mapper.Map<List<ServicioGetDto>>(servicios);
            return Ok(serviciosDto);
          
        }

        // GET: api/servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServicioGetDto>> Getservicio(int id)
        {
            var servicio = await _context.servicios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound();
            }
            var servicioDto = _mapper.Map<ServicioGetDto>(servicio);
            return Ok(servicioDto);
                       
        }

        // PUT: api/servicios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putservicio(int id, ServicioCreateOrUpdateDto servicio)
        {
            var servicioExistente = await _context.servicios.FindAsync(id);
            if (servicioExistente == null)
            {
                return NotFound();
            }

            _mapper.Map(servicio, servicioExistente);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!servicioExists(id))
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

        // POST: api/servicios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServicioGetDto>> Postservicio(ServicioCreateOrUpdateDto servicio)
        {
            var nuevoServicio = _mapper.Map<Servicio>(servicio);

            _context.servicios.Add(nuevoServicio);
            await _context.SaveChangesAsync();

            var servicioDto = _mapper.Map<ServicioGetDto>(nuevoServicio);


            return CreatedAtAction("Getservicio", new { id = nuevoServicio.servicioid }, servicioDto);
        }

        // DELETE: api/servicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteservicio(int id)
        {
            var servicio = await _context.servicios.FindAsync(id);
            if (servicio == null)
            {
                return NotFound();
            }

            _context.servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool servicioExists(int id)
        {
            return _context.servicios.Any(e => e.servicioid == id);
        }
    }
}

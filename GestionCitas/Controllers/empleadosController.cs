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
    public class empleadosController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public empleadosController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/empleados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoGetDto>>> Getempleados()
        {
            var empleados = await _context.empleados.ToListAsync();
            var empleadosDto = _mapper.Map<List<EmpleadoGetDto>>(empleados);
            return Ok(empleadosDto);

        }

        // GET: api/empleados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoGetDto>> GetEmpleado(int id)
        {
            var empleado = await _context.empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }
            var empleadoDto = _mapper.Map<EmpleadoGetDto>(empleado);

            return Ok(empleadoDto);
        }

        // PUT: api/empleados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, EmpleadoCreateOrUpdateDto empleado)
        {
            var empleadoExistente = await _context.empleados.FindAsync(id);
            if (empleadoExistente == null)
            {
                return NotFound();
            }

            _mapper.Map(empleado, empleadoExistente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(id))
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

        // POST: api/empleados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmpleadoGetDto>> PostEmpleado(EmpleadoCreateOrUpdateDto empleado)
        {
            var nuevoEmpleado = _mapper.Map<Empleado>(empleado);

            _context.empleados.Add(nuevoEmpleado);
            await _context.SaveChangesAsync();

            var empleadoDto = _mapper.Map<EmpleadoGetDto>(nuevoEmpleado);

            return CreatedAtAction("GetEmpleado", new { id = nuevoEmpleado.empleadoid }, empleadoDto);
        }

        // DELETE: api/empleados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            _context.empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpleadoExists(int id)
        {
            return _context.empleados.Any(e => e.empleadoid == id);
        }
    }
}

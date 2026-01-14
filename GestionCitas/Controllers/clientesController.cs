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
using Humanizer;

namespace GestionCitas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class clientesController : ControllerBase
    {
        private readonly GestionCitasContext _context;
        private readonly IMapper _mapper;

        public clientesController(GestionCitasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteGetDto>>> Getclientes()
        {
            var clientes = await _context.clientes.ToListAsync();
            var clientesDto = _mapper.Map<List<ClienteGetDto>>(clientes);
            return Ok(clientesDto);
           
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteGetDto>> Getcliente(int id)
        {
            var cliente = await _context.clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }
            var clienteDto = _mapper.Map<ClienteGetDto>(cliente);

            return Ok(clienteDto);
        }

        // PUT: api/clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putcliente(int id, ClienteCreateOrUpdateDto cliente)
        {
            var clienteExistente = await _context.clientes.FindAsync(id);
            if (clienteExistente == null)
            {
                return NotFound();
            }
                       
            _mapper.Map(cliente, clienteExistente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!clienteExists(id))
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

        // POST: api/clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClienteGetDto>> Postcliente(ClienteCreateOrUpdateDto cliente)
        {
            var nuevoCliente = _mapper.Map<Cliente>(cliente);

            _context.clientes.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            var clienteDto = _mapper.Map<ClienteGetDto>(nuevoCliente);

            return CreatedAtAction("Getcliente", new { id = nuevoCliente.clienteid }, clienteDto);
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecliente(int id)
        {
            var cliente = await _context.clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool clienteExists(int id)
        {
            return _context.clientes.Any(e => e.clienteid == id);
        }
    }
}

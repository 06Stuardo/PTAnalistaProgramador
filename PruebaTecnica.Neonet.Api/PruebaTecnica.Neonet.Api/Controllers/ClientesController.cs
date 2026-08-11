using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Neonet.Api.DTOs.Cliente;
using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Services;

namespace PruebaTecnica.Neonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        { 
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<List<Cliente>> ObtenerTodos()
        {
            return await _clienteService.ObtenerTodosAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RegistrarClienteDto cliente)
        {
            var clienteId = await _clienteService.RegistrarAsync(cliente);
            return Ok(new{clienteId});
        }

    }
}

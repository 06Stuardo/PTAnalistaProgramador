using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PruebaTecnica.Neonet.Api.DTOs.Venta;
using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Services;

namespace PruebaTecnica.Neonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly VentaService _ventaService;

        public VentasController(VentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [HttpGet]
        public async Task<List<Venta>>  ObtenerTodos()
        {
            return await _ventaService.ObtenerTodosConDetallesAsync();
        }

        [HttpGet("resumen")]
        public async Task<List<Venta>> ObtenerResumen()
        {
            return await _ventaService.ObtenerTodosAsync();
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<List<Venta>>  ObtenerPorCliente(int clienteId)
        {
            return await  _ventaService.ObtenerPorClienteAsync(clienteId);
        }

        [HttpGet("{idVenta}/detalles")]
        public async Task<List<DetalleVenta>> ObtenerDetalles(int idVenta)
        {
            return await _ventaService.ObtenerDetallesPorVentaAsync(idVenta);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] RegistrarVentaDto venta)
        {
            try
            {
                var resultado = await _ventaService.RegistrarVentaAsync(venta);
                return Ok(
                    new
                {
                    ventaId = resultado.VentaId,
                    total = resultado.Total
                }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                {
                    mensaje = ex.Message
                }
                );
            }
            catch (SqlException ex)
            {
                return BadRequest(
                    new
                {
                    mensaje = ex.Message
                });
            }
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Services;

namespace PruebaTecnica.Neonet.Api.Controllers
{
    [ApiController]
    [Route ("api/[controller]")]
    public class ProductosController
    {
        private readonly ProductoService _productoService;

        public ProductosController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<List<Producto>> ObtenerTodos()
        {
            var produtos = await _productoService.ObtenerTodosAsync();
            return produtos;
        } 
    }
}

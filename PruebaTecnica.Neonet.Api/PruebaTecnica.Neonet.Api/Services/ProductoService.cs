using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Repositories;

namespace PruebaTecnica.Neonet.Api.Services
{
    public class ProductoService
    {
        private readonly ProductoRepository _productoRepository;

        public ProductoService(ProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _productoRepository.ObtenerTodosAsync();
        }
    }
}

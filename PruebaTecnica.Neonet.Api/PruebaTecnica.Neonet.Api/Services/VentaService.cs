using PruebaTecnica.Neonet.Api.DTOs.Venta;
using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Repositories;

namespace PruebaTecnica.Neonet.Api.Services
{
    public class VentaService
    {
        private readonly VentaRepository _ventaRepository;
        private readonly DetalleVentaRepository _detalleVentaRepository;

        public VentaService(
            VentaRepository  ventaRepository,
            DetalleVentaRepository detalleVentaRepository
        )
        {
            _ventaRepository  = ventaRepository;
            _detalleVentaRepository =  detalleVentaRepository;
        }

        public async Task<List<Venta>> ObtenerTodosAsync()
        {
            return await _ventaRepository.ObtenerTodosAsync();
        }

        public async Task<List<Venta>> ObtenerTodosConDetallesAsync()
        {
            var ventas= await _ventaRepository.ObtenerTodosAsync();

            foreach (var venta in ventas)
            {
                venta.detalles =await _detalleVentaRepository.ObtenerPorVentaAsync(venta.id);
            }
            return ventas;
        }

        public async Task<List<Venta>> ObtenerPorClienteAsync(int clienteId)
        {
            return await  _ventaRepository.ObtenerPorClienteAsync(clienteId);
        }

        public async Task<List<DetalleVenta>> ObtenerDetallesPorVentaAsync(int idVenta)
        {
            return await _detalleVentaRepository.ObtenerPorVentaAsync(idVenta);
        }

        public async Task<(int VentaId, decimal Total)> RegistrarVentaAsync(RegistrarVentaDto venta)
        {
            if (venta.detalles == null ||  venta.detalles.Count == 0)
                throw new ArgumentException("La venta debe contener al menos un detalle.");

            foreach (var detalle in venta.detalles)
            {
                if (detalle.cantidad <= 0)
                    throw new ArgumentException("La cantidad debe ser mayor a cero.");

                if (detalle.precioUnitario < 0)
                    throw new ArgumentException("El precio unitario no puede ser negativo. ");
            }

            return await _ventaRepository.RegistrarVentaAsync(venta);
        }
    }
}

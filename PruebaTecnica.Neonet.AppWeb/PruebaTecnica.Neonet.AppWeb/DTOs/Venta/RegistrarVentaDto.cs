namespace PruebaTecnica.Neonet.AppWeb.DTOs.Venta
{
    public class RegistrarVentaDto
    {
        public int clienteId { get; set; }
        public DateTime fecha { get; set; }
        public List<RegistrarDetalleVentaDto> detalles { get; set; } = [];
    }
}

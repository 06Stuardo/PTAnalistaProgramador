namespace PruebaTecnica.Neonet.AppWeb.Models
{
    public class DetalleVenta
    {
        public int id { get; set; }
        public int productoId { get; set; }
        public int ventaId { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }
    }
}

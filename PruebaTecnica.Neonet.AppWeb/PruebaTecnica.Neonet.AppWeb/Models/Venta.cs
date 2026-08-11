namespace PruebaTecnica.Neonet.AppWeb.Models
{
    public class Venta
    {
        public int id { get; set; }
        public int clienteId { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
        public List<DetalleVenta> detalles { get; set; }
    }
}

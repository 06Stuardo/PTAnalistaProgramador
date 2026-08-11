namespace PruebaTecnica.Neonet.Api.Models
{
    public class Producto
    {
        public int id { get; set; }
        public string nombre { get; set; } = string.Empty;
        public decimal precio {  get; set; }
        public int stock { get; set; }
    }
}

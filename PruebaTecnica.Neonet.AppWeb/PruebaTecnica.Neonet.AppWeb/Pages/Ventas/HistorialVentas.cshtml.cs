using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PruebaTecnica.Neonet.AppWeb.Models;

namespace PruebaTecnica.Neonet.AppWeb.Pages.Ventas
{
    public class HistorialVentasModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<Cliente> Clientes { get; set; } = [];
        public List<Venta> Ventas { get; set; } = [];
        public List<DetalleVenta> Detalles { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public int? ClienteId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? VentaId { get; set; }

        public HistorialVentasModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("NeonetApi");

            Clientes =
                await client.GetFromJsonAsync<List<Cliente>>("api/Clientes")?? [];
             
            if (ClienteId.HasValue)
            {
                Ventas =await client.GetFromJsonAsync<List<Venta>>($"api/Ventas/cliente/{ClienteId.Value}")?? [];
            }
            if (VentaId.HasValue)
            {
                Detalles = await client.GetFromJsonAsync<List<DetalleVenta>>($"api/Ventas/{VentaId.Value}/detalles") ?? [];
            }
        }
    }
}

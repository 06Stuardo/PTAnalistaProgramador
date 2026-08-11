using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PruebaTecnica.Neonet.AppWeb.DTOs.Venta;
using PruebaTecnica.Neonet.AppWeb.Models;

namespace PruebaTecnica.Neonet.AppWeb.Pages.Ventas
{
    public class RegistrarVentaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public List<Cliente> Clientes { get; set; } = [];
        public List<Producto> Productos { get; set; } = [];

        [BindProperty]
        public RegistrarVentaDto Venta { get; set; } = new();

        public RegistrarVentaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            await CargarDatosAsync();
            Venta.fecha = DateTime.Now;
        }

        public async Task<IActionResult> OnPostAsync()

        {

            Console.WriteLine("ENTRÓ AL ONPOST");

            var client = _httpClientFactory.CreateClient("NeonetApi");

            Console.WriteLine($"Cliente: {Venta.clienteId}");
            Console.WriteLine($"Fecha: {Venta.fecha}");
            Console.WriteLine($"Detalles: {Venta.detalles.Count}");

            var response = await client.PostAsJsonAsync("api/Ventas", Venta);
            var contenido = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Respuesta API: {response.StatusCode}");
            Console.WriteLine($"Contenido API: {contenido}");

            Console.WriteLine($"Respuesta API: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["MensajeError"] = $"Ocurrió un error: {error}";
                ModelState.AddModelError(string.Empty, error);
                await CargarDatosAsync();
                return Page();
            }

            TempData["MensajeExito"] = "Venta registrada correctamente.";
            return RedirectToPage("/Ventas/RegistrarVenta");
        }

        private async Task CargarDatosAsync()
        {
            var client = _httpClientFactory.CreateClient("NeonetApi");
            Clientes = await client.GetFromJsonAsync<List<Cliente>>("api/Clientes")?? [];
            Productos = await client.GetFromJsonAsync<List<Producto>>("api/Productos") ?? [];
        }
    }
}

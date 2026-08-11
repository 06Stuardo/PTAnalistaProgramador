using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PruebaTecnica.Neonet.AppWeb.Models;

namespace PruebaTecnica.Neonet.AppWeb.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory  _httpClientFactory;

        public List<Producto> Productos { get; set; } = [];

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient("NeonetApi");

            Productos = await client.GetFromJsonAsync<List<Producto>>("api/Productos") ?? [];
        }
    }
}

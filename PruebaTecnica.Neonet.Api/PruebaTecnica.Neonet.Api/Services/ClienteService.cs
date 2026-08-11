using PruebaTecnica.Neonet.Api.DTOs.Cliente;
using PruebaTecnica.Neonet.Api.Models;
using PruebaTecnica.Neonet.Api.Repositories;

namespace PruebaTecnica.Neonet.Api.Services
{
    public class ClienteService
    {
        private readonly ClienteRepository _clienteRepository;

        public ClienteService(ClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _clienteRepository.ObtenerTodosAsync();
        }

        public async Task<int> RegistrarAsync(RegistrarClienteDto cliente)
        {
            return await _clienteRepository.RegistrarAsync(cliente);
        }
    }
}

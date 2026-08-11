using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Neonet.Api.DTOs.Cliente
{
    public class RegistrarClienteDto
    {
        public string nombre { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "El formato del correo es inválido.")]
        public string email { get; set; } = string.Empty;
    }
}

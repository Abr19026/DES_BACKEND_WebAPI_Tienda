using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.DTOs
{
    public class CredencialesUsuario
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}

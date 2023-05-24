using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

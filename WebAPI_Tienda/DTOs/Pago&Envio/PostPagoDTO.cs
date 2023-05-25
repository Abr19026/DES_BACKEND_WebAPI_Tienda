using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.DTOs
{
    public class PostPagoDTO
    {
        [Required]
        [CreditCard]
        public string NumeroTarjeta { get; set; }
        [Required]
        [Range(0,12)]
        public int YYExpiracion { get; set; }
        [Required]
        [Range(1900, 9999)]
        public int MMExpiracion { get; set; }
    }
}

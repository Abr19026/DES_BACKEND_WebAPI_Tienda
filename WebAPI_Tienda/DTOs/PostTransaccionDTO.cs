using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.DTOs
{
    public class PostTransaccionDTO
    {
        [Required]
        public PostPagoDTO pago { get; set; }
        [Required]
        public DatosEnvioDTO envio { get; set; }
    }
}

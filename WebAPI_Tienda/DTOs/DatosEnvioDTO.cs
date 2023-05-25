using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class DatosEnvioDTO
    {
        [Required]
        public int CodigoPostal { get; set; }
        [Required]
        public string Dirección { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }
    }
}

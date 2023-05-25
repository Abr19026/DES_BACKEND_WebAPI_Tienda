using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class DatosEnvioDTO
    {
        public int CodigoPostal { get; set; }
        public string Dirección { get; set; }
        public string Telefono { get; set; }
    }
}

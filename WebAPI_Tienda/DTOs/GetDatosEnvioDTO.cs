using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class GetDatosEnvioDTO
    {
        public int ID { get; set; }
        public int CodigoPostal { get; set; }
        public string Dirección { get; set; }
        public string Telefono { get; set; }
    }
}

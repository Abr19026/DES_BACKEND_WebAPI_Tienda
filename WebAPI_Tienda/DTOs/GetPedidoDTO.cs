using Microsoft.AspNetCore.Identity;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class GetPedidoDTO
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Estado { get; set; }
        public GetPagoDTO Pago { get; set; }
        public GetDatosEnvioDTO Envio { get; set; }
        public float Total { get; set; }
        public ICollection<GetConceptodPedidoDTO> ConceptosPedido { get; set; }
    }
}

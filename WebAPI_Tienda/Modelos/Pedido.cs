using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    public class Pedido
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string UserID { get; set; }
        [Required]
        public bool Confirmado { get; set; }    // Indica que hay un pago, si no hay pago es un "carrito"
        // Propiedades de navegación
        public Pago Pago { get; set; }
        public DatosEnvio Envio { get; set; }
        public IdentityUser User { get; set; }
        public ICollection<ConceptoPedido> ConceptosPedido { get; set; }
    }
}

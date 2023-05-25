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
        public EstadoPedido Estado { get; set; }    // Indica que hay un pago, si no hay pago es un "carrito"
        public float Total { get; set; }
        // Propiedades de navegación
        public Pago Pago { get; set; }
        public DatosEnvio Envio { get; set; }
        public IdentityUser User { get; set; }
        public ICollection<ConceptoPedido> ConceptosPedido { get; set; }
    }

    public enum EstadoPedido { 
        EnCarrito,  //Indica que el pedido es un carrito
        ConfirmacionPendiente,  // Indica que inició proceso de pago
        Confirmado, // Indica que terminó proceso de pago
        Cancelado   //Indica que se canceló
    }
}

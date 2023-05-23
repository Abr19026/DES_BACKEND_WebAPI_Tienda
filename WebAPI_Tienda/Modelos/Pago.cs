using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    public class Pago
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int PedidoID { get; set; }
        [Required]
        public Pedido Pedido { get; set; }
        // Datos de pago
        [Required]
        [CreditCard]
        public string NumeroTarjeta { get; set; }
        [Required]
        [Range(1900,9999)]
        public int YYExpiracion { get; set; }// Año de expiración
        [Required]
        [Range(1,12)]
        public int MMExpiracion { get; set; }// Mes de expiración
        [Required]
        public DateTime FechaPago { get; set; }
        
    }
}

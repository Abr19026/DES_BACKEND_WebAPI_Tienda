using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    public class DatosEnvio
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int PedidoID { get; set; }
        // Datos de envío
        [Required]
        public int CodigoPostal { get; set; }
        [Required]
        public string Dirección { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Telefono { get; set; }

        // Propiedad de navegación
        public Pedido Pedido { get; set; }
    }
}

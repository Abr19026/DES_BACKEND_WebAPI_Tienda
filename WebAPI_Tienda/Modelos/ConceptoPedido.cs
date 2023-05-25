using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    // Llave compuesta ProductoID,PedidoID
    public class ConceptoPedido
    {
        [Required]
        public int ProductoID { get; set; }
        [Required]
        public int PedidoID { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public EstadoEntrega EstadoEntrega { get; set; }
        public float PrecioUnitario { get; set; }
        // Propiedades de navegación
        public Producto Producto { get; set; }
        public Pedido Pedido { get; set; }        
    }

    public enum EstadoEntrega
    {
        Despachando,
        Enviando,
        Recibido,
        Cancelado
    }
}
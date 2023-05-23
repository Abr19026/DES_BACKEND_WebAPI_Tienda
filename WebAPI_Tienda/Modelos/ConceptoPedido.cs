using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    [PrimaryKey(nameof(ProductoID), nameof(PedidoID))]
    public class ConceptoPedido
    {
        [Required]
        public int ProductoID { get; set; }
        [Required]
        public int PedidoID { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public float PrecioUnitario { get; set; }
        [Required]
        public EstadoEntrega EstadoEntrega { get; set; }
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
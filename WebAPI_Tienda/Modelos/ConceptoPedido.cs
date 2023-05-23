namespace WebAPI_Tienda.Modelos
{
    public class ConceptoPedido
    {
        public int ID { get; set; }
        public int ProductoID { get; set; }
        public int PedidoID { get; set; }
        public int Cantidad { get; set; }
        public float PrecioUnitario { get; set; } 
        public EstadoEntrega EstadoEntrega { get; set; }

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
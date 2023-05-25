namespace WebAPI_Tienda.DTOs
{
    public class GetConceptodPedidoDTO
    {
        public int Cantidad { get; set; }
        public float PrecioUnitario { get; set; }
        public GetProductoPedidoDTO Producto { get; set; }
    }
}

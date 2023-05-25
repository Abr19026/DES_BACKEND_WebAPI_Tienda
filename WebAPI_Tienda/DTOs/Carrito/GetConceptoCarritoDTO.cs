namespace WebAPI_Tienda.DTOs
{
    public class GetConceptoCarritoDTO
    {
        public int Cantidad { get; set; }
        // Propiedades de navegación
        public GetProductoPedidoDTO Producto { get; set; }
        public float PrecioUnitario { get; set; }
        public string EstadoEntrega { get; set; }
    }
}

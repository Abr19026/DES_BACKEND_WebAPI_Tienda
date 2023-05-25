namespace WebAPI_Tienda.DTOs
{
    public class ConceptoPedidoEstadoDTO
    {
        public int PedidoID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public float PrecioUnitario { get; set; }
        public string EstadoEnvio { get; set; }
        public string NombreProducto { get; set; }
    }
}

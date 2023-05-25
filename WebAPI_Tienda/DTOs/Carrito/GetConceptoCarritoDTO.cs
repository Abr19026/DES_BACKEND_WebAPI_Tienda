namespace WebAPI_Tienda.DTOs
{
    public class GetConceptoCarritoDTO
    {
        public int Cantidad { get; set; }
        // Propiedades de navegación
        public GetResumenProdutcoDTO Producto { get; set; }        
    }
}

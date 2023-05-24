using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class GetProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Existencias { get; set; }
        public float Precio { get; set; }
        // Campos no obligatorios
        public string Descripcion { get; set; }
        // Propiedades de navegación
        public ICollection<GetCategoriaDTO> Categorias { get; set; }
    }
}

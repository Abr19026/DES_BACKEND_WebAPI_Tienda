using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.DTOs
{
    public class PostProductoDTO
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Range(0, double.PositiveInfinity)]
        public int Existencias { get; set; }
        [Required]
        [Range(0, double.PositiveInfinity)]
        public float Precio { get; set; }
        // Campos no obligatorios
        public IFormFile Foto { get; set; }
        public string Descripcion { get; set; }
        public List<string> IdsDeCategorias { get; set; }
    }
}

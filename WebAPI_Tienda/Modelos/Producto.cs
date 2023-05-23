using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Range(0,double.PositiveInfinity)]
        public int Existencias { get; set; }
        [Required]
        [Range(0, double.PositiveInfinity)]
        public float Precio { get; set; }
        // Campos no obligatorios
        public string Descripcion { get; set; }
        public byte[] Foto { get; set; }
        // Propiedades de navegación
        public ICollection<Categoria> Categorias { get; set; }
    }
}

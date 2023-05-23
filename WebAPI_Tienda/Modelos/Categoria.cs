using System.ComponentModel.DataAnnotations;

namespace WebAPI_Tienda.Modelos
{
    public class Categoria
    {
        [Key]
        public string ID { get; set; }
        // Propiedad de navegación
        public ICollection<Producto> Productos {  get; set; }
    }
}

namespace WebAPI_Tienda.Modelos
{
    public class Categoria
    {
        public string ID { get; set; }
        public ICollection<Producto> Productos {  get; set; }
    }
}

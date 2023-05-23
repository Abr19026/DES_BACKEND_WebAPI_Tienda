namespace WebAPI_Tienda.Modelos
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Existencias { get; set; }
        public float Precio { get; set; }
        public byte[] Foto { get; set; }
        public ICollection<Categoria> Categorias { get; set; }
    }
}

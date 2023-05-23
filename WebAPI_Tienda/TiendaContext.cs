using WebAPI_Tienda.Modelos;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_Tienda
{
    public class TiendaContext: DbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<ConceptoPedido> ConceptosPedidos { get; set; }

        /*
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Course>().ToTable("Course");
        }*/
    }
}

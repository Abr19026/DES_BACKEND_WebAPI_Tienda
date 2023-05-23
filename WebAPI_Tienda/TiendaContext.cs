using WebAPI_Tienda.Modelos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebAPI_Tienda
{
    public class TiendaContext: IdentityDbContext
    {
        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<ConceptoPedido> ConceptosPedidos { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Producto>().ToTable("NuevoNombreTablaProductos"); para cambiar el nombre de la tabla
        }
    }
}

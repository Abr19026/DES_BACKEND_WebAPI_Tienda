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
            // Agrega tablas de autenticación
            base.OnModelCreating(modelBuilder);
            // Marca llave compuesta de ConceptoPedido
            modelBuilder.Entity<ConceptoPedido>()
                .HasKey(c => new { c.ProductoID, c.PedidoID });
            //modelBuilder.Entity<Producto>().ToTable("NuevoNombreTablaProductos"); para cambiar el nombre de la tabla
        }
    }
}

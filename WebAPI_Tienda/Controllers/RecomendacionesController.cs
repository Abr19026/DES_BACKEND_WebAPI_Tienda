using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("api/recomendaciones")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RecomendacionesController : ControllerBase
    {
        private readonly TiendaContext _context;
        private readonly IMapper _mapper;

        public RecomendacionesController(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetProductoDTO>>> ObtenerRecomendaciones()
        {
            // Obtener el usuario actual o su identificador
            var userId = User.Identity.Name;

            // Obtener la categoría más frecuente de compras del usuario
            var categoriaMasFrecuente = await _context.ConceptosPedidos
                .Include(cp => cp.Pedido)
                .Include(cp => cp.Producto)
                .Where(cp => cp.Pedido.UserID == userId && cp.Pedido.Estado == EstadoPedido.Confirmado)
                .GroupBy(cp => cp.Producto.Categorias.FirstOrDefault().ID)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (categoriaMasFrecuente == null)
            {
                // No se encontraron compras confirmadas para el usuario
                return NotFound();
            }

            // Obtener productos relacionados con la categoría más frecuente
            var productosRecomendados = await _context.Productos
                .Include(p => p.Categorias)
                .Where(p => p.Categorias.Any(c => c.ID == categoriaMasFrecuente))
                .ToListAsync();

            return productosRecomendados.Select(producto => _mapper.Map<GetProductoDTO>(producto)).ToList();
        }
    }
}



using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("Carrito")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CarritoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;
        public CarritoController(TiendaContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // Es necesario guardar la base de datos cuando se termine de usar el elemento
        private Pedido CreaCarrito(string UserID)
        {
            var carrito = new Pedido()
            {
                UserID = UserID,
                Confirmado = false
            };
            _context.Pedidos.Add(carrito);
            return carrito;
        }

        [HttpGet]
        public async Task<ActionResult<List<GetCarritoResumenDTO>>> GetCarritos()
        {
            var userId = HttpContext.User.Claims.Where(claim => claim.Type == "userId").FirstOrDefault().Value;
            var carritos = await
                _context.Pedidos
                    .Where(pedido => pedido.UserID == userId && pedido.Confirmado == false)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .ToListAsync();

            if (carritos.Count < 1)
            {
                carritos.Add(CreaCarrito(userId));
                await _context.SaveChangesAsync();
            }
            return carritos.Select(carrito => _mapper.Map<GetCarritoResumenDTO>(carrito)).ToList();
        }

        [HttpGet("{carritoid:int}")]
        public async Task<ActionResult<GetCarritoResumenDTO>> GetCarrito(int carritoid)
        {
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            var carrito = await _context.Pedidos
                    .Where(pedido =>
                            pedido.ID == carritoid &&
                            pedido.UserID == userId &&
                            pedido.Confirmado == false)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .FirstOrDefaultAsync();

            if (carrito == null)
            {
                return NotFound();
            }
            return _mapper.Map<GetCarritoResumenDTO>(carrito);
        }

        [HttpPost("{carritoId:int}")]
        public async Task<ActionResult> agregarProductoCarrito([Required] int productoId, int carritoId, [Required] int cantidad)
        {
            // Validación
            if (cantidad < 1)
            {
                return BadRequest("Cantidad no válida");
            }
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            var carrito = await _context.Pedidos
                    .Where(pedido =>
                            pedido.ID == carritoId &&
                            pedido.UserID == userId &&
                            pedido.Confirmado == false)
                    .Include(pedido => pedido.ConceptosPedido)
                .FirstOrDefaultAsync();
            
            if (carrito == null)
            {
                return NotFound("Carrito no existe");
            }

            var producto = await _context.Productos
                .Where(prod => prod.Id == productoId).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound("Producto no existe");
            }
            // Agrega un concepto con ese producto al carrito
            carrito.ConceptosPedido.Add(new ConceptoPedido()
            {
                Producto = producto,
                Pedido = carrito,
                Cantidad = cantidad,
            });

            _context.Update(carrito);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{carritoId:int}")]
        public async Task<ActionResult> quitarProductoCarrito([Required] int productoId, int carritoId)
        {
            // Validación
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            var conceptoCarrito = await _context.ConceptosPedidos
                .Where(concepto => 
                        concepto.ProductoID == productoId && 
                        concepto.PedidoID == carritoId &&
                        concepto.Pedido.UserID == userId)
                .FirstOrDefaultAsync();

            if (conceptoCarrito == null)
            {
                return NotFound();
            }

            // quita el concepto del carrito
            _context.Remove(conceptoCarrito);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}

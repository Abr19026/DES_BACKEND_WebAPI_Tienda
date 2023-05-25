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
                Estado = EstadoPedido.EnCarrito
            };
            _context.Pedidos.Add(carrito);
            return carrito;
        }

        private async Task<float> PrecioTotalCarrito(int pedidoid)
        {
            return await _context.ConceptosPedidos
                  .Where(concepto => concepto.PedidoID == pedidoid)
                  .Include(concepto => concepto.Producto)
                  .SumAsync(concepto => concepto.Cantidad * concepto.Producto.Precio);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetCarritoResumenDTO>>> GetCarritos()
        {
            var userId = HttpContext.User.Claims.Where(claim => claim.Type == "userId").FirstOrDefault().Value;
            var carritos = await
                _context.Pedidos
                    .Where(pedido => pedido.UserID == userId && pedido.Estado == EstadoPedido.EnCarrito)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .ToListAsync();

            if (carritos.Count < 1)
            {
                carritos.Add(CreaCarrito(userId));
                await _context.SaveChangesAsync();
            }
            var carritosDTOS = carritos.Select(carrito => _mapper.Map<GetCarritoResumenDTO>(carrito)).ToList();
            foreach (var carro in carritosDTOS)
            {
                carro.total = await PrecioTotalCarrito(carro.ID);
            }
            return carritosDTOS;
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
                            pedido.Estado == EstadoPedido.EnCarrito)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .FirstOrDefaultAsync();

            if (carrito == null)
            {
                return NotFound();
            }
            var carro = _mapper.Map<GetCarritoResumenDTO>(carrito);
            carro.total = await PrecioTotalCarrito(carro.ID);
            return carro;
        }

        [HttpPost("{carritoId:int}/productos")]
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
                            pedido.Estado == EstadoPedido.EnCarrito)
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

        [HttpDelete("{carritoId:int}/productos")]
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

        [HttpGet("transaccion")]
        public async Task<ActionResult<List<GetPedidoDTO>>> GetPagosPendientes()
        {
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            var pedidos_pendientes = await _context.Pedidos
                .Where(pedido =>
                       pedido.UserID == userId &&
                       pedido.Estado == EstadoPedido.ConfirmacionPendiente)
                    .Include(pedido => pedido.Pago)
                    .Include(pedido=> pedido.Envio)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                    .Select(pedido=> _mapper.Map<GetPedidoDTO>(pedido))
                .ToListAsync();

            return pedidos_pendientes;
        }

        [HttpPost("transaccion/{carritoid:int}")]
        public async Task<ActionResult<GetPedidoDTO>> iniciarPago([Required]PostTransaccionDTO datostransaccion ,int carritoid)
        {
            // Valida autorización y existencia
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;
            var pedido_valido = await _context.Pedidos
                .Where(pedido => pedido.ID == carritoid &&
                       pedido.UserID == userId &&
                       pedido.Estado == EstadoPedido.EnCarrito)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .FirstOrDefaultAsync();
            if (pedido_valido == null)
            {
                return NotFound();
            }
            // Guarda y valida datos de envio
            var datosEnv = _mapper.Map<DatosEnvio>(datostransaccion.envio);
            datosEnv.PedidoID = carritoid;
            // Guarda y valida datos de pago
            var datosPago = _mapper.Map<Pago>(datostransaccion.pago);
            datosPago.PedidoID = carritoid;
            datosPago.FechaPago = DateTime.Now;
            // Crea Pedido
            pedido_valido.Estado = EstadoPedido.ConfirmacionPendiente;
            pedido_valido.Pago = datosPago;
            pedido_valido.Envio = datosEnv;

            float costo_total = 0;
            foreach(var concepProd in pedido_valido.ConceptosPedido)
            {
                concepProd.PrecioUnitario = concepProd.Producto.Precio;
                costo_total += concepProd.PrecioUnitario * concepProd.Cantidad;
            }
            pedido_valido.Total = costo_total;
            _context.Update(pedido_valido);
            await _context.SaveChangesAsync();

            // Retorna datos dados
            var pedido_retorno = _mapper.Map<GetPedidoDTO>(pedido_valido);
            pedido_retorno.Total = costo_total;
            return pedido_retorno;
        }
    }
}
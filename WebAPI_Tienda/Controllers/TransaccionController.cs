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
    [Route("transaccion")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransaccionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;
        public TransaccionController(TiendaContext context, IMapper mapper) { 
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
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
                    .Include(pedido => pedido.Envio)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                    .Select(pedido => _mapper.Map<GetPedidoDTO>(pedido))
                .ToListAsync();

            return pedidos_pendientes;
        }

        [HttpPost("iniciar/{carritoid:int}")]
        public async Task<ActionResult<GetPedidoDTO>> iniciarPago([Required] PostTransaccionDTO datostransaccion, int carritoid)
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
            _context.Add(datosEnv);
            // Guarda y valida datos de pago
            var datosPago = _mapper.Map<Pago>(datostransaccion.pago);
            datosPago.FechaPago = DateTime.Now;
            _context.Add(datosPago);
            // Crea Pedido
            pedido_valido.Estado = EstadoPedido.ConfirmacionPendiente;
            pedido_valido.Pago = datosPago;
            pedido_valido.Envio = datosEnv;

            float costo_total = 0;
            foreach (var concepProd in pedido_valido.ConceptosPedido)
            {
                concepProd.PrecioUnitario = concepProd.Producto.Precio;
                costo_total += concepProd.PrecioUnitario * concepProd.Cantidad;
            }
            pedido_valido.Total = costo_total;
            _context.Update(pedido_valido);
            await _context.SaveChangesAsync();

            // Retorna datos dados
            var pedido_retorno = _mapper.Map<GetPedidoDTO>(pedido_valido);
            return pedido_retorno;
        }

        [HttpPost("cancelar/{carritoid:int}")]
        public async Task<ActionResult> cancelarPago(int carritoid)
        {
            // Valida permisos
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;
            var pedido_valido = await _context.Pedidos
                .Where(pedido => pedido.ID == carritoid &&
                       pedido.UserID == userId &&
                       pedido.Estado == EstadoPedido.ConfirmacionPendiente)
                    .Include(pedido => pedido.Pago)
                    .Include(pedido=> pedido.Envio)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .FirstOrDefaultAsync();
            if (pedido_valido == null)
            {
                return NotFound();
            }

            pedido_valido.Estado = EstadoPedido.Cancelado;
            foreach (var conc_pedid in pedido_valido.ConceptosPedido)
            {
                conc_pedid.EstadoEntrega = EstadoEntrega.Cancelado;
            }
            _context.Remove(pedido_valido.Pago);
            _context.Remove(pedido_valido.Envio);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("confirmar/{carritoid:int}")]
        public async Task<ActionResult<String>> confirmarPago(int carritoid)
        {
            // Valida permisos
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;
            var pedido_valido = await _context.Pedidos
                .Where(pedido => pedido.ID == carritoid &&
                       pedido.UserID == userId &&
                       pedido.Estado == EstadoPedido.ConfirmacionPendiente)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .FirstOrDefaultAsync();
            if (pedido_valido == null)
            {
                return NotFound();
            }

            // confirma que las existencias estén en los límites
            var productos_sobrestock = pedido_valido.ConceptosPedido
                                      .Where(concepto => concepto.Cantidad > concepto.Producto.Existencias).ToList();
            if (productos_sobrestock.Count > 0)
            {
                var prods_sobrestock_mensaje = "";
                foreach (var concepto in productos_sobrestock)
                {
                    prods_sobrestock_mensaje += concepto.Producto.Nombre;
                }
                return BadRequest($"Siguientes conceptos sobrepasan existencias: {prods_sobrestock_mensaje}");
            }
            // confirma la compra
            pedido_valido.Estado = EstadoPedido.Confirmado;
            // Resta las existencias y marca los envíos como despachando
            foreach (var conc_pedid in pedido_valido.ConceptosPedido)
            {
                conc_pedid.Producto.Existencias -= conc_pedid.Cantidad;
                conc_pedid.EstadoEntrega = EstadoEntrega.Despachando;
            }

            _context.Update(pedido_valido);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

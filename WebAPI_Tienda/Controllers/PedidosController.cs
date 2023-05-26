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
    [Route("pedidos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PedidosController: Controller
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;

        public PedidosController(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<GetPedidoDTO>>> GetHistorial() {
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            return await _context.Pedidos
                .Where(pedido =>
                       pedido.UserID == userId &&
                       (
                        pedido.Estado == EstadoPedido.Confirmado ||
                        pedido.Estado == EstadoPedido.Cancelado
                       ))
                    .Include(pedido => pedido.Envio)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .Select(pedido => _mapper.Map<GetPedidoDTO>(pedido))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetPedidoDTO>> GetPedido(int id)
        {
            var userId = HttpContext.User.Claims.
                        Where(claim => claim.Type == "userId").
                        FirstOrDefault().Value;

            var retorno = await _context.Pedidos
                .Where(pedido =>
                       pedido.UserID == userId &&
                       pedido.ID == id &&
                       (
                        pedido.Estado == EstadoPedido.Confirmado ||
                        pedido.Estado == EstadoPedido.Cancelado
                       ))
                    .Include(pedido => pedido.Envio)
                    .Include(pedido => pedido.ConceptosPedido)
                    .ThenInclude(concepto => concepto.Producto)
                .Select(pedido => _mapper.Map<GetPedidoDTO>(pedido))
                .FirstOrDefaultAsync();

            if (retorno == null)
            {
                return NotFound();
            }
            return retorno;
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpGet("pendientes")]
        public async Task<ActionResult<List<ConceptoPedidoEstadoDTO>>> GetPedidosConPendientes()
        {
            return await _context
                .ConceptosPedidos
                .Include(concepto => concepto.Pedido)
                .Where(conceptopedido =>
                       (
                        conceptopedido.EstadoEntrega == EstadoEntrega.Enviando ||
                        conceptopedido.EstadoEntrega == EstadoEntrega.Despachando 
                       ) &&
                       conceptopedido.Pedido.Estado == EstadoPedido.Confirmado
                )
                .Include(concepto => concepto.Producto)
                .Select(concepto => _mapper.Map<ConceptoPedidoEstadoDTO>(concepto))
                .ToListAsync();
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpPost("pendientes")]
        public async Task<ActionResult> CambiarEstadoConcepto([Required] int pedidoid, [Required] int productoid, [Required] EstadoEntrega nuevo_estado)
        {
            var concepto = await _context
                .ConceptosPedidos
                .Where(concepto => concepto.ProductoID == productoid && concepto.PedidoID == pedidoid)
                .Include(concepto => concepto.Pedido)
                .FirstOrDefaultAsync();
            if (concepto == null)
            {
                return NotFound();
            }
            if(concepto.Pedido.Estado != EstadoPedido.Confirmado)
            {
                return BadRequest("Este concepto es de un pedido cancelado o no confirmado");
            }
            concepto.EstadoEntrega = nuevo_estado;
            _context.Update(concepto);
            await _context.SaveChangesAsync();
            //enviar correo
            return Ok();
        }

    }
}

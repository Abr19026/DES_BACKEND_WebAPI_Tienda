using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        /*
        [HttpGet]
        public async Task<ActionResult<List<Pedido>>> GetCarritos()
        {
            return await _context.Categorias.Select(x => _mapper.Map<GetCategoriaDTO>(x)).ToListAsync();
        }*/
    }
}

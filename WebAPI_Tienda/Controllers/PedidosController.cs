using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI_Tienda.Controllers
{
    public class PedidosController: Controller
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;
        public PedidosController(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;
using WebAPI_Tienda.Utilidades;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("api/busqueda")]
    public class BusquedaController : ControllerBase
    {
        private readonly TiendaContext _context;
        private readonly IMapper _mapper;

        public BusquedaController(TiendaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<GetProductoDTO>> BuscarProductos(string terminoBusqueda, string categoria)
        {
            var query = _context.Productos.Include(p => p.Categorias).AsQueryable();

            if (!string.IsNullOrEmpty(terminoBusqueda))
            {
                query = query.Where(p => p.Nombre.Contains(terminoBusqueda));
            }

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(p => p.Categorias.Any(c => c.ID.Contains(categoria)));
            }

            var productos = query.ToList();

            var productosDTO = _mapper.Map<List<GetProductoDTO>>(productos);

            return Ok(productosDTO);
        }
    }
}

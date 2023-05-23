using Microsoft.AspNetCore.Mvc;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController: ControllerBase
    {
        private readonly TiendaContext _context;
        public ProductosController(TiendaContext context) {
            _context = context;
        }

        /*
        [HttpGet]
        public ActionResult<List<GetProductoDTO>> Get()
        {
            
            //return await _context.Productos.ToListAsync();
            
        }

        [HttpPost]
        public async Task<ActionResult> Post(PostProductoDTO producto)
        {
            
            //_context.Add(producto);
            //await _context.SaveChangesAsync();
            //return Ok();
        }*/
    }
}

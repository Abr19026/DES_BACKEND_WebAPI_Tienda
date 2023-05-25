using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;
using WebAPI_Tienda.Utilidades;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("api/productos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;
        public static IWebHostEnvironment _webHostEnvironment;
        public ProductosController(TiendaContext context, IWebHostEnvironment webHostEnvironment, IMapper mapper) {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<GetProductoDTO>>> Get()
        {
            var it1 = await _context.Productos
                        .Include(ProductoDB => ProductoDB.Categorias)
                        .ToListAsync();
            return it1.Select(producto => _mapper.Map<GetProductoDTO>(producto)).ToList();
        }

        [AllowAnonymous]
        [HttpGet("imagen/{id:int}")]
        public async Task<IActionResult> GetImagen(int id)
        {
            var prod = await _context.Productos.FirstOrDefaultAsync(productoBD => productoBD.Id == id);
            if (prod == null)
            {
                return NotFound();
            }
            if (prod.Foto != null)
            {
                return File(prod.Foto, "image/*");
            } else
            {
                return File(new byte[0], "image/*");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PostProductoDTO productodto)
        {
            // crear producto y guardar foto
            var producto = _mapper.Map<Producto>(productodto);
            // Agrega categorias del producto (Si no existe manda error)
            foreach (var IdCat in productodto.IdsDeCategorias)
            {
                var cat = await _context.Categorias.FirstOrDefaultAsync(x => x.ID == IdCat);
                if (cat != null)
                {
                    producto.Categorias.Add(cat);
                } else
                {
                    return NotFound($"La categoría <<{IdCat}>> no existe");
                }
            }
            // Agrega producto a la base de datos
            _context.Add(producto);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PostProductoDTO productodto)
        {
            var exist = await _context.Productos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            // crear producto y guardar foto
            var producto = _mapper.Map<Producto>(productodto);
            producto.Id = id;
            // Agrega categorias del producto (Si no existe manda error)
            foreach (var IdCat in productodto.IdsDeCategorias)
            {
                var cat = await _context.Categorias.FirstOrDefaultAsync(x => x.ID == IdCat);
                if (cat != null)
                {
                    producto.Categorias.Add(cat);
                }
                else
                {
                    return NotFound($"La categoría <<{IdCat}>> no existe");
                }
            }
            // Agrega producto a la base de datos
            _context.Update(producto);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _context.Productos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            _context.Remove(new Producto()
            {
                Id = id
            });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

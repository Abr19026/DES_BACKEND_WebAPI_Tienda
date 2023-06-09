﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Tienda.DTOs;
using WebAPI_Tienda.Modelos;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoriasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly TiendaContext _context;
        public CategoriasController(TiendaContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<GetCategoriaDTO>>> Get()
        {
            return await _context.Categorias.Select(x=> _mapper.Map<GetCategoriaDTO>(x)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(PostCategoriaDTO nuevaCat)
        {
            _context.Add(_mapper.Map<Categoria>(nuevaCat));
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{nombre}")]
        public async Task<ActionResult> Delete(string nombre)
        {
            var exist = await _context.Categorias.AnyAsync(x => x.ID == nombre);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            _context.Remove(new Categoria()
            {
                ID = nombre
            });
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}

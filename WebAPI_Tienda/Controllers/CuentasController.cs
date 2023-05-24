using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_Tienda.DTOs;

namespace WebAPI_Tienda.Controllers
{
    [ApiController]
    [Route("cuentas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuracion;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuracion, SignInManager<IdentityUser> signInManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.configuracion = configuracion;
            this.signInManager = signInManager;
            this._mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credenciales)
        {
            var user = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var result = await userManager.CreateAsync(user, credenciales.Password);

            if (result.Succeeded)
            {
                // Retorna el JWT
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credenciales)
        {
            var result = await signInManager.PasswordSignInAsync(
                credenciales.Email,
                credenciales.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Retorna el JWT
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpGet("admins")]
        public async Task<ActionResult<List<GetUserDTO>>> GetAdmins()
        {
            var admins = await userManager.GetUsersForClaimAsync(new Claim("esAdmin", "1"));
            return admins.Select(user => _mapper.Map<GetUserDTO>(user)).ToList();
        }


        //[Authorize(Policy = "RequiereAdmin")]
        [HttpPost("admins")]
        public async Task<ActionResult> AgregarAdmin(EditarAdminDTO admindto)
        {
            var usuario = await userManager.FindByEmailAsync(admindto.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return Ok();
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpDelete("admins")]
        public async Task<ActionResult> QuitarAdmin(EditarAdminDTO admindto)
        {
            var usuario = await userManager.FindByEmailAsync(admindto.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return Ok();
        }

        // Solo llamar después de que se haya ingresado exitosamente
        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            // Claims son información del cliente en la cual podemos confiar
            // No deben contener info sensible como contraseñas o tarjetas de crédito
            
            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email),
                new Claim("userId", usuario.Id)
            };
            // Agrega Claims de la base de datos (Tiene que Entrar de nuevo para actualizarse)
            claims.AddRange(claimsDB);

            // Crea respuesta autenticación
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracion["keyJWT"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddHours(1); // Debe ser minutos o segundos
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, signingCredentials: creds, expires: expiration);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }
    }
}

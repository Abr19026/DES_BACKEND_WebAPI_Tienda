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
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuracion;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuracion, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuracion = configuracion;
            this.signInManager = signInManager;
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
                return ConstruirToken(credenciales);
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
                return ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpPost("ElevarPrivilegios")]
        private async Task<ActionResult> AgregarAdmin(EditarAdminDTO admindto)
        {
            var usuario = await userManager.FindByEmailAsync(admindto.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return Ok();
        }

        [Authorize(Policy = "RequiereAdmin")]
        [HttpPost("QuitarPrivilegios")]
        private async Task<ActionResult> QuitarAdmin(EditarAdminDTO admindto)
        {
            var usuario = await userManager.FindByEmailAsync(admindto.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return Ok();
        }

        private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credenciales)
        {
            // Claims son información del cliente en la cual podemos confiar
            // No deben contener info sensible como contraseñas o tarjetas de crédito
            var claims = new List<Claim>
            {
                new Claim("email", credenciales.Email)
                //new Claim("otroClaim", "abdcsdfse")
            };

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

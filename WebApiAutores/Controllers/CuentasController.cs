using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        /*
         * REGISTRAMOS UN USUARIO CON CREACIÓN DE TOKENs
         * 
         * RUTA: api/cuentas/registrar
         */
        [HttpPost("registrar")] 
        public async Task<ActionResult<RespuestaAutentificacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded) 
            {
                return ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }


        /*
         * LOGIN DE USUARIO
         */
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutentificacion>> Login (CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto"); //No le devolvemos los errores que ha tenido para no revelar credenciales a un usuario malintencionado
            }
        }


        /*
         * CONSTRUIMOS TOKEN PARA EL REGISTRO DEL USUARIO
         */
        private RespuestaAutentificacion ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            //Un Claim es llave - valor, nosotros podemos ver los claims pero el usuario también va a verlos(no se puede poner datos sensibles en el claim, password, tarjetas de crédito...)
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),

            };
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutentificacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }
        
    }
}

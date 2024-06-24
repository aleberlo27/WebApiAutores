using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    /*
     * Ruta raiz que va a devolver lo que el usuario puede hacer
     */
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {

        /*
         * Necesitamos verificar si el usuario tiene estos tipos de permisos para poder decirle lo que PUEDE hacer o NO
         * Utilizaremos un servicio que nos permita hacerlo que nos lo ofrece .NETCore
         */
        private readonly IAuthorizationService authorizationService;

        //Dependencia 1: IAuthorizationService la cual usamos para saber si el usuario es administrador (Para hacer el test no necesitamos saber qué hace) => MOCKS
        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DateHATEOAS>>> Get()
        {
            var datosHateoas = new List<DateHATEOAS>();

            //Con esta variable podemos saber si es admin y poder mostrarle lo que verdaderamente puede hacer dentro de nuestra app
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            //Con "self" nos referimos a donde apuntamos, apuntamos a donde el mismo usuario se encuentra
            datosHateoas.Add(new DateHATEOAS(
                enlace: Url.Link("ObtenerRoot", new { }), //Dependencia 2: Url que viene heredado de ControllerBase (Para hacer el test no necesitamos saber qué hace) => MOCKS
                descripcion: "self",
                metodo: "GET"));

            datosHateoas.Add(new DateHATEOAS(
                enlace: Url.Link("obtenerAutores", new { }),
                descripcion: "autores",
                metodo: "GET"));

            //Si la variable declarada anteriormente da que el usuario tiene la política de serAdmin entonces le mostrará también estos métodos:
            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DateHATEOAS(
                    enlace: Url.Link("crearAutor", new { }),
                    descripcion: "autor-crear",
                    metodo: "POST"));

                datosHateoas.Add(new DateHATEOAS(
                    enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-crear",
                    metodo: "POST"));
            }

            /*
             * No ponemos más como por ejemplo crearComentario porque crearComentario depende de un LibroId asi que eso lo colocamos en los enlaces de Libro y 
             * no en los enlaces generales de la app porque no tenemos manera de especificar el id del libro desde el ROOT
             */
            return datosHateoas;
        }
    }
}

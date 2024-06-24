using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController] //Permite hacer validaciones automaticas respecto a la data recibida en nuestro controlador
    //[Route("api/v1/autores")] //Declaramos la ruta en la que = esta clase va a recibir las peticiones
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version","1")]
    //Con authorize lo que hacemos es que salte un 401 para el usuario (unauthorized) y no pueda obtener el listado de autores
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")] //Usamos el AddIdentity en la clase startup, entonces tenemos que poner el JwtBearerDefaults
    //Aqui controlamos las respuestas que da el servidor al usuario, las documentamos en cada petición que podría hacer el usuario a la bd 
    [ApiConventionType(typeof(DefaultApiConventions))] //También se pueden controlar en cada peticion (ej en el GET con ID de autor)
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutoresv1")]
        [AllowAnonymous] //Para que el atributo authorize en este campo sea NO autorizado, que no tengas que poner tus credenciales ni el token 
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync(); 

            return mapper.Map<List<AutorDTO>>(autores);

        }


        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        //Si controlamos varios errores al meterte dentro del get con id abajo antes de ejecutar te dirá que te pueden salir estas respuestas
        //[ProducesResponseType(404)] //Con este response controlamos que la app pueda sacar un 404 documentado, sino sacaria un 404 unndocumented
        //[ProducesResponseType(200)] //Este response controlamos el 200 que pueda dar la app

        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorlibroDB => autorlibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id); //Traer solo un resultado que coincida con la condicion

            if (autor == null)
            {
                return NotFound(); //404
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);

            return dto;
        }

        //Buscamos en la bd por el nombre del autores, no podemos poner restricciones porque un dato string nos daría error
        [HttpGet("{nombre}", Name = "obtenerAutorPorNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpPost(Name = "crearAutorv1")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (await existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autores con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }


        [HttpPut("{id:int}", Name = "actualizarAutorv1")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            //Nos aseguramos de que el id que ha metido por parámetro exista en la bd
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            //Aqui no hacemos la actualización, solo marcamos el autores que queremos actualizar
            context.Update(autor);

            //La actualización a nivel de base de datos se marca por el SaveChangesAsync
            await context.SaveChangesAsync();
            return Ok();

        }

        //Con 3 slash agregamos el comentario que queremos que salga en pantalla cuando vayamos a borrar el autor, se tiene que configurar en Startup
        /// <summary>
        /// Borra un autor
        /// </summary>
        /// <param name="id"> ID del autor a borrar </param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "borrarAutorv1")] //  api/autores/id
        public async Task<ActionResult> Delete(int id)
        {
            //AnyAsync quiere decir que si existe alguno en la bd, estamos llendo a la tabla autores y a ver si existe
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            //Borramos el registro
            context.Remove(new Autor() { Id = id });

            //El borrado real se realiza cuando hacemos el SaveChangesAsync
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

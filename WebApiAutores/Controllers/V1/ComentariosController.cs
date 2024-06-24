using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Utilidades;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId, [FromQuery] PaginacionDTO paginacionDTO)
        {
            var existeLibros = await context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);
            if (!existeLibros)
            {
                return NotFound();
            }

            var queryable = context.Comentarios.Where(comentarioBD => comentarioBD.LibroId == libroId).AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);

            var comentarios = await queryable.OrderBy(comentario => comentario.Id).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "obtenerComentarios")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);
            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name = "crearComentario")]
        //Necesitamos autorización de usuario para poder escribir comentarios en los libros
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            /*
             * Los claims (información sobre el usuario) los estamos sacando del HttpContext el cual me permite acceder a los claims 
             * del json web token, no colocamos el ID del usuario como un parámetro dentro del comentarioCreacionDTO porque tenemos
             * un problema de seguridad por el simple hecho de que cualquier usuario puede colocar cualquier comentario en cualquier
             * libro.
             * 
             * Para que nosotros podamos tener esos Claims primero el usuario tuvo que autentificarse, tuvo que presentar sus credenciales,
             * Eso nos da cierta seguridad de que sea el usuario el que se está loggeando con su cuenta.
             * 
             * Es mala práctica recibir un parámetro por el método, hay que hacerlo a traves de los Claims de los JWT
             * 
             */
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;

            var usuario = await userManager.FindByEmailAsync(email);

            var usuarioId = usuario.Id;

            var existeLibros = await context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);

            if (!existeLibros)
            {
                return NotFound();
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);

            //Igualamos las variables una vez autentificado el usuario para que el comentario en la tabla sql tenga el campo usuarioId
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentarios", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibros = await context.Libros.AnyAsync(libroBD => libroBD.Id == libroId);
            if (!existeLibros)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!existeComentario)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;
            context.Update(comentario);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}

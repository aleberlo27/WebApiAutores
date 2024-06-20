using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context, IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            /*
             * Con el include en esta líne lo que estamos diciendole a la tabla libros es que incluya la lista de
             * comentarios que hemos creado en la entidad Libros (como un Join en sql)
             * 
             * IMPORTANTE
             * Va a depender de que cuando queramos obtener información de un libro incluyamos los comentarios o no,
             * para no gastar data de los usuarios incluyendo a lo mejor los comentarios del libro (que lo mismo ni
             * los quieren leer) sería mejor que cada entidad tuviera un espacio en la aplicación.
             * 
             * var libro = await context.Libros.Include(libroBD => libroBD.Comentarios).FirstOrDefaultAsync(x => x.Id == id);
             */
            var libro = await context.Libros.Include(libroDB => libroDB.AutoresLibros).ThenInclude(autorLibroDB => autorLibroDB.Autor).FirstOrDefaultAsync(x => x.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }

        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un lbiro sin autores.");
            }
            
            var autoresIds = await context.Autores.Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).Select(x => x.Id).ToListAsync();

            if(libroCreacionDTO.AutoresIds.Count !=  autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados.");
            }
            
            var libro = mapper.Map<Libro>(libroCreacionDTO);

            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i<libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }
        
    }
}

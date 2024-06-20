using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    [ApiController] //Permite hacer validaciones automaticas respecto a la data recibida en nuestro controlador
    [Route("api/autores")] //Declaramos la ruta en la que = esta clase va a recibir las peticiones
    
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<AutorDTO>> Get()
        {
            var autores = await context.Autores.ToListAsync(); //Traer un listado de nuestra tabla de autores
            return mapper.Map<List<AutorDTO>>(autores);
        }


        [HttpGet("{id:int}")]
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

            return mapper.Map<AutorDTOConLibros>(autor); 
        }

        //Buscamos en la bd por el nombre del autores, no podemos poner restricciones porque un dato string nos daría error
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);
        }

        
        [HttpPost]
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
            return Ok();
        }

      
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id) 
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autores no coincide con el id de la URL.");
            }

            //Nos aseguramos de que el id que ha metido por parámetro exista en la bd
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            //Aqui no hacemos la actualización, solo marcamos el autores que queremos actualizar
            context.Update(autor);

            //La actualización a nivel de base de datos se marca por el SaveChangesAsync
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("{id:int}")] //  api/autores/id
        public async Task<ActionResult> Delete(int id)
        {
            //AnyAsync quiere decir que si existe alguno en la bd, estamos llendo a la tabla autores y a ver si existe
            var existe= await context.Autores.AnyAsync(x => x.Id == id);

            if(!existe)
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

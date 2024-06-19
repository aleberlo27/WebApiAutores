using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers
{
    //Lo que va dentro de los corchetes son atributos

    [ApiController] //Permite hacer validaciones automaticas respecto a la data recibida en nuestro controlador
    [Route("api/autores")] //Declaramos la ruta en la que = esta clase va a recibir las peticiones
    
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
       
        public AutoresController(ApplicationDbContext context) 
        {
            this.context = context;
        }

        [HttpGet]//  api/autores
        public async Task<List<Autor>> Get()
        {
            return await context.Autores/*.Include(x => x.Libros)*/.ToListAsync();
        }


        /*
         * En la ruta podemos definir variables para obtener solo el autor, en este caso, que queremos visualizar
         * Si declaramos en la ruta la variable id como int, si metes un string nos retornará un 404.
         */
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null) 
            {
                return NotFound();
            }
            return autor; 
        }

        //Buscamos en la bd por el nombre del autor, no podemos poner restricciones porque un dato string nos daría error
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }
        /*
         * Creación del autor en la base de datos
         */
        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            var existeAutorConElMismoNombre = context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);

            if (await existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        /*
         * Actualización de los datos de la bd
         * Cuando colocamos el valor de id como int dentro del httpPut lo que hacemos es que solo
         * valgan valores enteros en ese campo.
         */
        [HttpPut("{id:int}")] //  api/autores/1 (o 2, o 3, o 4...)
        public async Task<ActionResult> Put(Autor autor, int id) 
        {
            if (autor.Id != id)
            {
                return BadRequest("El id del autor no coincide con el id de la URL.");
            }

            //Nos aseguramos de que el id que ha metido por parámetro exista en la bd
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            //Aqui no hacemos la actualización, solo marcamos el autor que queremos actualizar
            context.Update(autor);

            //La actualización a nivel de base de datos se marca por el SaveChangesAsync
            await context.SaveChangesAsync();
            return Ok();

        }

        /*
         * Borrado de algun dato de la base de datos
         */
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

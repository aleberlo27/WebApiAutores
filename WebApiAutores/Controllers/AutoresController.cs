using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Servicios;

namespace WebApiAutores.Controllers
{
    //Lo que va dentro de los corchetes son atributos

    [ApiController] //Permite hacer validaciones automaticas respecto a la data recibida en nuestro controlador
    [Route("api/autores")] //Declaramos la ruta en la que = esta clase va a recibir las peticiones
    //Si quisieramos proteger todas las peticiones HTTP deberíamos poner aquí arriba al controller [Authorize] 
    public class AutoresController : ControllerBase
    {

        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(ApplicationDbContext context, IServicio servicio, ServicioTransient servicioTransient,
            ServicioScoped servicioScoped, ServicioSingleton servicioSingleton, ILogger<AutoresController> logger) 
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        /*
         * Si llega una primera http a la ruta se le retorna el OK de ActionResult, pero las proximas peticiones HTTP que lleguen
         * en los próximos 10 segundos se van a servir del caché, es decir, el contenido del return ok se guardará en memoria y eso va a ser 
         * lo que se le va a responder a los usuarios dentro de los 10 segundos después de la última respuesta.
         */
        // [ResponseCache(Duration = 10)]
        [ServiceFilter(typeof(MiFiltroDeAccion))] //Nuestro filtro de acción
        public ActionResult ObtenerGuids()
        {
            return Ok(new
            {
                AutoresController_Transient = servicioTransient.Guid,
                ServicioA_Transient = servicio.ObrenerTransient(),
                AutoresController_Scoped = servicioScoped.Guid,
                ServicioA_Scoped = servicio.ObrenerScoped(),
                AutoresController_Singleton = servicioSingleton.Guid,
                ServicioA_Singleton = servicio.ObrenerSingleton()
            });

        }
        /*
         * Obtener datos de la bd (enviarle datos al cliente)
         */
        [HttpGet]//  api/autores
        [HttpGet("listado")]//   api/autores/listado
        [HttpGet("/listado")]//  listado    (Las rutas se heredan desde la ruta que ponemos arriba, si la queremos cambiar tenemos que ponerle la barra)
        [Authorize] //Solo con poner Authorize estamos protegiendo este end point que tenemos en lista de autores, no tenemos lista de usuarios pero podemos usarlo igual
        //En cuanto intentemos sacar este listado nos dará un error 401 unauthorized porque no estamos en la lista de usuarios que pueden acceder
        public async Task<ActionResult<List<Autor>>> Get()
        {
            throw new NotImplementedException();
            logger.LogInformation("Estamos obteniendo los autores");
            servicio.RealizarTarea();
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        /*
         * Si no cambiamos la ruta al httpGet (dentro de los paréntesis) vamos a tener un error a la hora de
         * ejecutar o abrir la página porque cogería la misma ruta para los dos httpGet, tenemos que cambiarle la última
         * palabra de la ruta para que no sea la misma.
         */
        [HttpGet("primero")] //Ruta: api/autores/primero
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int mivalor)
        {
            //Con FirstOrDefaultAsync obtenemos el primer registro de la tabla o null si no hay ningun registro
            return await context.Autores.FirstOrDefaultAsync();
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

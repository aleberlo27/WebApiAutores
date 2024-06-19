using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Controllers;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using static WebApiAutores.Servicios.IServicio;

namespace WebApiAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                //Declaramos de manera global el filtro que hemos creado como personalizado
                opciones.Filters.Add(typeof(FiltroDeExcepcion));

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            /*
            services.AddTransient<IServicio, ServicioA>();

            services.AddTransient<ServicioTransient>();

            services.AddScoped<ServicioScoped>();

            services.AddSingleton<ServicioSingleton>();

            //Configuramos el serviicio que hemos configurado para el filtro que hemos creado de acción
            services.AddTransient<MiFiltroDeAccion>();

            //Configuramos el serviicio que hemos configurado para el filtro que hemos creado de escribir en archivo
            services.AddHostedService<EscribirEnArchivo>(); 
            */

            //Configuramos los servicios del filtro de caché, con estos dos pasos ya está listo el caché para usarlo
            //services.AddResponseCaching();

            /* 
             * Vamos a configurar los servicios del filtro de autentificación, para usar JwtBearerDefaults hay que instalar el packagin 
             * que es uno de los servicios que contiene C#, no viene instalado.
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddSwaggerGen(c => 
            { 
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
            });

        }

        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();   
            //Esta línea de abajo es la misma que la de arriba solo que no exposeamos la clase que estamos usando, solo el método 

            app.UseLoguearRespuestaHTTP();

            /*
             * Con la función Map lo que hacemos es hacer una bifurcación de la tubería de procesos
             * Si el usuario hace una petición a ruta1, se va a ejecutar este middleware y ya, del contrario
             * si el usuario hace una petición a cualquier otra ruta se ejecutarán los otros middleware que hay
             * por debajo de este map.
             
            app.Map("/ruta1", app =>
            {
                /*
                * Para que no se ejecute la página y le salga un mensaje al cliente tenemos este método que
                * lo que hace es correr esa línea de comandos sin dejar que entre a configurar la página HTTP
                * Es un MIDDLEWARE, deteniendo la ejecución de los otros middleware
                */
            /*
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería.");
                });

            });
        */

            /*
             * Si el usuario no elige la ruta1 y elige cualquier otra de usuarios o la que podamos tener
             * se bajará a estos middleware para ejecutar la página normalmente.
             * 
             */
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Configuramos el filtro de caché y arriba configuramos sus servicios
            //app.UseResponseCaching();

            //Configuramos el middleware/filtro de autorización y arriba configuramos sus servicios
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;
using WebApiAutores.Servicios;
using WebApiAutores.Utilidades;

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

                //Declaramos el controlador de versiones con la clase que hemos creado en utilidades
                opciones.Conventions.Add(new SwaggerAgrupaPorVersion());

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson(); //Aquí añadimos la configuración de NewtonSoft

            /*
             * CONFIGURACIÓN DE SERVICIO PARA LA CONEXIÓN A LA BD 
             * 
             * Esta es la clase que utilizamos en el código de nuestra aplicación para interactuar con la base de datos subyacente.
             * Es esta clase la que administra la conexión de la base de datos y se utiliza para recuperar y guardar datos en la base de datos.
             */
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            /* 
             * Vamos a configurar los servicios del filtro de autentificación, para usar JwtBearerDefaults hay que instalar el packagin 
             * que es uno de los servicios que contiene C#, no viene instalado.
             * 
             * Los parametros de validacion del token también se configuran con JwtBearer
             * 
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters 
            { 
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["llavejwt"])),ClockSkew = TimeSpan.Zero
            });

            //CONFIGURACIÓN SWAGGER PARA QUE UTILICE LOS JWT (Json Web Token)
            services.AddSwaggerGen(c => 
            {
                //Configurando los servicios para que pueda usar las 2 versiones que tenemos de autoresController
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2" });

                //Configuración de parámetros en swagger
                c.OperationFilter<AgregarParametroHATEOAS>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name= "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });

            });


            //Servicio para poder usar automapper en la clase Utilidades
            services.AddAutoMapper(typeof(Startup));

            //Servicios para el Identity Framework, creación de tablas objeto relacionales pra .NET
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            /*
             * Con este servicio lo que hacemos es que añadimos políticas a la hora de autorización de permisos a un usuario, 
             * si la app nos detecta que estamos logueados pero no tenemos esos permisos lo que hace es que nos tira un 403
             * lo que significa que está prohibido realizar la acción que intento realizar.
             */
            services.AddAuthorization(opciones =>
            {
                //Agregar una política de seguridad para que un usuario Admin pueda realizar acciones que los usuarios normales no pueden
                opciones.AddPolicy("esAdmin", politica => politica.RequireClaim("esAdmin"));

                //Podemos tener diferentes claims que puedan hacer distintas acciones unos de otros
                //opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
            });

            //Con este añadido tenemos acceso a la protección de datos 
            services.AddDataProtection();


            //Configuramos el servicio de HASH (Como este servicio no guarda estado lo ponemos como Transient)
            services.AddTransient<HashService>();

            /*
             * Configurando el servicio de cors (uso compartido de recursos entre orígene, una ampliación de la política del mismo origen)
             * 
             * Aqui estamos configurando Cors para permitir realizar peticiones HTTP desde un sitio web específico, para una app movil no sirve, solo para
             * apps web. (Solamente aplicaciones que corren con Angular, React...)
             */
            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    /*
                     * Al agregar una política por defecto, agregamos las URLs que tienen acceso a nuestra web api
                     * Method se refiere a métodos HTTP como post, delete, put...
                     * AllowAnyHeader para permitir cualquier cabecera, si tu necesitas poner cabeceras que vas a devolver desde tu webAPI
                     * tu puedes añadir: [].WithExposedHeaders();
                     */
                    builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader();
                });

            });

            //Configuramos los servicios del GeneradorEnlaces para poder usarlo en las demás clases
            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();   
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        }

        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
           
            app.UseLoguearRespuestaHTTP();

            /*
             * Si el usuario no elige la ruta1 y elige cualquier otra de usuarios o la que podamos tener
             * se bajará a estos middleware para ejecutar la página normalmente.
             */
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Configurando la app para que pueda usar las 2 versiones que tenemos de autoresController
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");
                });

            /*
             * Este middleware sirve para redirigir peticiones HTTP a HTTPS, obligamos a nuestros clientes
             * a conectarse a nuestra app con HTTPS.
             */
            app.UseHttpsRedirection();

            app.UseRouting();

            //Configurar o añadir el Cors (activar el Cors en nuestra WebAPI)
            app.UseCors();

            //Configuramos el middleware/filtro de autorización y arriba configuramos sus servicios
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

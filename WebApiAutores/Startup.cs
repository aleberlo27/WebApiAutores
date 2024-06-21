using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

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

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson(); //Aquí añadimos la configuración de NewtonSoft

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
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
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
        }

        public void Configure (IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
           
            //Esta línea de abajo es la misma que la de arriba solo que no exposeamos la clase que estamos usando, solo el método 

            app.UseLoguearRespuestaHTTP();

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

            //Configuramos el middleware/filtro de autorización y arriba configuramos sus servicios
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

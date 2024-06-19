using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace WebApiAutores.Middlewares
{
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }
    /*
     * Si por algun casual quisieramos guardar todas las respuestas HTTP que realiza nuestro servidor
     * deberíamos poner el método aqui arriba ya que de un middleware pasa al siguiente y asi sucesivamente, 
     * cuando llega al final del método 'configure' vuelve a dar la vuelta hacia arriba y la última instrucción
     * que devuelve la respuesta es la que está arriba del método.
     * Usamos Useing porque queremos agregar nuestro propio proceso pero permitir que los demás procesos se sigan
     * utilizando, si no quisiéramos que los demás procesos se ejecuten en vez de 'use' usaríamos 'Run' como en el
     * caso de abajo.
     */
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                //Cuando usamos siguiente es para que le permitamos a la tubería de procesos continuar
                await siguiente(contexto);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(respuesta);
            }
        }
    }
}

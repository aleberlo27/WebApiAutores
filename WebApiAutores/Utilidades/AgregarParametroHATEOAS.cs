using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiAutores.Utilidades
{
    /*
     * Para poder utilizar esta clase en otras debemos configurar sus servicios en la clase Startup
     */
    public class AgregarParametroHATEOAS : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //Estamos filtrando solo para que los métodos GET puedan usar la opción de aplicar el filtro de saber qué pueden hacer o no
            if (context.ApiDescription.HttpMethod != "GET")
            {
                return;
            }

            if(operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "incluirHATEOAS",
                In = ParameterLocation.Header,
                Required = false
            });
        }
    }
}

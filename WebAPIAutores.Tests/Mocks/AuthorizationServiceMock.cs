using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIAutores.Tests.Mocks
{
    /*
     * Generamos esta clase para verificar en los tests de rootcontrollerTests si el usuaro es administrador, como hemos hecho la clase genérica, 
     * el Success o el Failed lo controlaremos dentro del test.
     * Usamos esta clase para generar los tests porque necesitamos suplantar dependencias con MOCKS al tener más de 1 dependencia la clase RootController
     */
    public class AuthorizationServiceMock : IAuthorizationService
    {
        public AuthorizationResult Resultado { get; set; }  
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(Resultado);
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(Resultado);
        }
    }
}

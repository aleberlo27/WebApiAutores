using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiAutores.Controllers.V1;
using WebAPIAutores.Tests.Mocks;

namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass] //MOCKS
    public class RootControllerTests
    {
        [TestMethod]
        public async Task  SiUsuarioEsAdmin_Obtenemos4Links()
        {
            //PREPARACION (creamos 2 mock de las dependencias de la clase que hemos querido hacer el test: AuthorizationServiceMock y URLHelperMock)
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success(); //Mediante el mock de AuthorizationServiceMock decimos que el usuario es admin
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            //EJECUCION
            var resultado = await rootController.Get();

            //VERIFICACION (Ponemos un 4 porque es el número de URLs que esperamos dentro del método que implementa RootController)
            Assert.AreEqual(4, resultado.Value.Count());  
        }

        [TestMethod]
        public async Task SiUsuarioNOEsAdmin_Obtenemos4Links()
        {
            //PREPARACION (creamos 2 mock de las dependencias de la clase que hemos querido hacer el test: AuthorizationServiceMock y URLHelperMock)
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();//Mediante el mock de AuthorizationServiceMock decimos que el usuario NO es admin
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            //EJECUCION (cogemos el resultado que nos retorna rootcontroller)
            var resultado = await rootController.Get();

            //VERIFICACION (Ponemos un 2 porque es el número de URLs que esperamos dentro del método que implementa RootController)
            Assert.AreEqual(2, resultado.Value.Count());  
        }
    }
}

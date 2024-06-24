using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;


namespace WebAPIAutores.Tests.PruebasUnitarias
{
    [TestClass] //PRUEBAS UNITARIAS
    public class PrimeraLetraMayusculaAttributeTests
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            //PREPARACION
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            var valor = "alejandra";
            var valContext = new ValidationContext(new { Nombre = valor });

            //EJECUCIÓN
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //VERIFICACIÓN (assert es una clase que me permite hacer verificaciones, si la verificacion no es satisfactoria arroja un error)
            Assert.AreEqual("La primera letra debe ser mayúscula.", resultado.ErrorMessage);
        }

        
        [TestMethod]
        public void ValorNulo_NoDevuelveError()
        {
            //PREPARACION
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            //EJECUCIÓN
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //VERIFICACIÓN (assert es una clase que me permite hacer verificaciones, si la verificacion no es satisfactoria arroja un error)
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError()
        {
            //PREPARACION
            var primeraLetraMayuscula = new PrimeraLetraMayusculaAttribute();
            string valor = "Alejandra";
            var valContext = new ValidationContext(new { Nombre = valor });

            //EJECUCIÓN
            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            //VERIFICACIÓN (assert es una clase que me permite hacer verificaciones, si la verificacion no es satisfactoria arroja un error)
            Assert.IsNull(resultado);  
        }

    }
}
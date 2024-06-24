using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones
{
    //Tiene solamente una dependencia, el ValidationAttribute que para hacer el test unitario no necesitamos saber qué hace esa clase
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula.");
            }
            return ValidationResult.Success;
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    //CON REGLAS DE ATRIBUTOS
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del autor es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres. ")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
         
        /*
         
            //Rango de edad
            [Range(18, 105)]
            //NotMapped quiere decir que no nos actualiza en la tabla este campo
            [NotMapped]
            public int Edad {  get; set; }

            //Este atributo te valida si es un número válido para la tarjeta de crédito que metas por pantalla
            [CreditCard]
            [NotMapped]
            public string TarjetaDeCredito { get; set; }

            //Este atributo te valida si es válida la url que has metido por pantalla
            [Url]
            [NotMapped]
            public string Url { get; set; }

            public int Menor { get; set; }  
            public int Mayor { get; set; }

        */
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] {nameof(Nombre)});
                }

            }
            /*
            if (Menor > Mayor)
            {
                yield return new ValidationResult("Este valor no puede ser más grande que el campo mayor.", new string[] { nameof(Nombre) });
            }
            */
        }
    }
}

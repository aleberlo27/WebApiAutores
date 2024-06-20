using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El nombre del autor es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres. ")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
    }
}

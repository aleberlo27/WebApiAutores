using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
   
    public class Autor 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del autor es requerido.")]
        [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres. ")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}

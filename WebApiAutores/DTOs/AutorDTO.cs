
namespace WebApiAutores.DTOs
{
    /*
     *  Si heredamos esta clase de Recurso podremos agregar los enlaces de HATEOAS desde el controlador 
     */
    public class AutorDTO : Recurso
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
    }
}


namespace WebApiAutores.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        /*
         * Incluimos una lista de comentarios para hacer un join con la tabla 
         * En el caso de que no queramos incluirla no es necesario que contenga 
         * esta lista de comentarios. 
         * 
         * public List<ComentarioDTO> Comentarios { get; set; }
         * 
         */
    }
}

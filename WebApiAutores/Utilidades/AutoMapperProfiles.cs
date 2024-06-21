using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{   
    /*
     * Para poder usar el automapper tenemos que descargarnos la librería: AutoMapper.Extensions.Microsoft.DependencyInjection
     * también a parte de descargarnos la librería tenemos que realizarle el servicio en Startup.
     */
    public class AutoMapperProfiles : Profile
    {
        /*
         * Utilizamos esta clase para pasar de un objeto a otro, para mapearlo automáticamente
         * Si en un objeto tenemos muchos atributos o clases, el automapper nos lo pasa de uno a otro
         * sin necesidad de ir poniendo campo a campo de un objeto a otro.
         */
        public AutoMapperProfiles()
        {
            //Mapeado de Autore a los DTOs
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>().ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

            //Mapeado de Libro a los DTOs
            CreateMap<LibroCreacionDTO, Libro>().ForMember(Libro => Libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>().ReverseMap();
            CreateMap<Libro, LibroDTOConAutores>().ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            //Mapeado de Comentario a los DTOs
            CreateMap<ComentarioCreacionDTO, Comentario>(); 
            CreateMap<Comentario, ComentarioDTO>();
        }


        private List<LibroDTO> MapAutorDTOLibros (Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if (autor.AutoresLibros == null)
            {
                return resultado;
            }

            foreach(var autorlibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorlibro.LibroId,
                    Titulo = autorlibro.Libro.Titulo
                });
            }
            
            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if (libro.AutoresLibros == null)
            {
                return resultado;
            }
            foreach(var autorlibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorlibro.AutorId,
                    Nombre = autorlibro.Autor.Nombre
                });
            }
            return resultado;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if (libroCreacionDTO.AutoresIds == null)
            {
                return resultado;
            }

            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro { AutorId = autorId });
            }

            return resultado;
        }
    }
}

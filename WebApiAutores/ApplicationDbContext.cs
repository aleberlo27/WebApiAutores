using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        /*
         * Con este método creamos una tabla (DbSet) en SQLServer en la base de datos a partir 
         * de la clase Autor que se va a llamar autores.
         */
        public DbSet<Autor> Autores { get; set; } 
        
        public DbSet<Libro> Libros { get; set; }
    }
}

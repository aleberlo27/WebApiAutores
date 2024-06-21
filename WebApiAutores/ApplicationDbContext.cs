using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

namespace WebApiAutores
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*
             * Para crear la clave primaria de la tabla AutorLibro que sea los dos ids de autor y libro 
             * hay que hacer un builder en este método para que la Key (llave primaria) sea AutorId y LibroId.
             */
            modelBuilder.Entity<AutorLibro>().HasKey(al => new {al.AutorId, al.LibroId});
        }

        /*
         * Con este método creamos una tabla (DbSet) en SQLServer en la base de datos a partir 
         * de la clase Autor que se va a llamar autores.
         */
        public DbSet<Autor> Autores { get; set; } 
        
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set;}

    }
}

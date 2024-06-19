using Microsoft.EntityFrameworkCore.Migrations;



namespace WebApiAutores.Migrations
{

    /*
     * Cuando en la terminal de Nu-Get (administrador de paquetes) hemos puesto Add-Migration Initial
     * nos crea una tabla como la que vemos ahí: nombre de la tabla Autores y tiene como campos el Id
     * y el nombre, como primary key usamos el id del autor 
     

    public partial class Inicial : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Autores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autores", x => x.Id);
                });
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Autores");
        }
    }
    */
}

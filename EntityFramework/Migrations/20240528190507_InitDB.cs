using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class InitDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CargaHoraria = table.Column<float>(type: "real", nullable: false),
                    Comisiones = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaID = table.Column<int>(type: "int", nullable: false),
                    AlumnoID = table.Column<int>(type: "int", nullable: true),
                    NotaFinal = table.Column<int>(type: "int", nullable: false),
                    Promocionable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reportes_Alumnos_AlumnoID",
                        column: x => x.AlumnoID,
                        principalTable: "Alumnos",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Reportes_Materias_MateriaID",
                        column: x => x.MateriaID,
                        principalTable: "Materias",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_AlumnoID",
                table: "Reportes",
                column: "AlumnoID");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_MateriaID",
                table: "Reportes",
                column: "MateriaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Materias");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class ComponentesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tolerancias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Componente = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Prueba = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Max = table.Column<double>(type: "float", nullable: false),
                    Min = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tolerancias", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tolerancias");
        }
    }
}

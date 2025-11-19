using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class AddComponenteRelationToTolerancias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Componente",
                table: "Tolerancias");

            migrationBuilder.AddColumn<int>(
                name: "ComponenteId",
                table: "Tolerancias",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tolerancias_ComponenteId",
                table: "Tolerancias",
                column: "ComponenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tolerancias_Componentes_ComponenteId",
                table: "Tolerancias",
                column: "ComponenteId",
                principalTable: "Componentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tolerancias_Componentes_ComponenteId",
                table: "Tolerancias");

            migrationBuilder.DropIndex(
                name: "IX_Tolerancias_ComponenteId",
                table: "Tolerancias");

            migrationBuilder.DropColumn(
                name: "ComponenteId",
                table: "Tolerancias");

            migrationBuilder.AddColumn<string>(
                name: "Componente",
                table: "Tolerancias",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }
    }
}

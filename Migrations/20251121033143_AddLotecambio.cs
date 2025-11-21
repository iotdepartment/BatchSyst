using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class AddLotecambio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Componentes_ComponenteId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_Folio",
                table: "Batches");

            migrationBuilder.AddColumn<string>(
                name: "RegistroId",
                table: "Batches",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_Folio",
                table: "Batches",
                column: "Folio");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_RegistroId",
                table: "Batches",
                column: "RegistroId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Componentes_ComponenteId",
                table: "Batches",
                column: "ComponenteId",
                principalTable: "Componentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Componentes_ComponenteId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_Folio",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_RegistroId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "RegistroId",
                table: "Batches");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_Folio",
                table: "Batches",
                column: "Folio",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Componentes_ComponenteId",
                table: "Batches",
                column: "ComponenteId",
                principalTable: "Componentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

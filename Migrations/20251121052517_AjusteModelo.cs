using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class AjusteModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultadosPrueba_Batches_BatchId",
                table: "ResultadosPrueba");

            migrationBuilder.DropColumn(
                name: "ExpDays",
                table: "Componentes");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "ResultadosPrueba",
                newName: "LoteId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultadosPrueba_BatchId",
                table: "ResultadosPrueba",
                newName: "IX_ResultadosPrueba_LoteId");

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "ResultadosPrueba",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<bool>(
                name: "EsValido",
                table: "ResultadosPrueba",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultadosPrueba_Batches_LoteId",
                table: "ResultadosPrueba",
                column: "LoteId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResultadosPrueba_Batches_LoteId",
                table: "ResultadosPrueba");

            migrationBuilder.DropColumn(
                name: "EsValido",
                table: "ResultadosPrueba");

            migrationBuilder.RenameColumn(
                name: "LoteId",
                table: "ResultadosPrueba",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_ResultadosPrueba_LoteId",
                table: "ResultadosPrueba",
                newName: "IX_ResultadosPrueba_BatchId");

            migrationBuilder.AlterColumn<float>(
                name: "Valor",
                table: "ResultadosPrueba",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "ExpDays",
                table: "Componentes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ResultadosPrueba_Batches_BatchId",
                table: "ResultadosPrueba",
                column: "BatchId",
                principalTable: "Batches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

           
        }
    }
}

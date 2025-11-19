using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class AddLoteRPrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ComponenteId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Componentes_ComponenteId",
                        column: x => x.ComponenteId,
                        principalTable: "Componentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResultadosPrueba",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    ToleranciaId = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadosPrueba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadosPrueba_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResultadosPrueba_Tolerancias_ToleranciaId",
                        column: x => x.ToleranciaId,
                        principalTable: "Tolerancias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_ComponenteId",
                table: "Batches",
                column: "ComponenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_Folio",
                table: "Batches",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPrueba_BatchId",
                table: "ResultadosPrueba",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadosPrueba_ToleranciaId",
                table: "ResultadosPrueba",
                column: "ToleranciaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultadosPrueba");

            migrationBuilder.DropTable(
                name: "Batches");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class addUserBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Batches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_UsuarioId",
                table: "Batches",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_AspNetUsers_UsuarioId",
                table: "Batches",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_AspNetUsers_UsuarioId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_UsuarioId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Batches");
        }
    }
}

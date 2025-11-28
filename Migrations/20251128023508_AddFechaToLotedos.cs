using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Batch.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaToLotedos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Batches",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()"); // ⚡ valor automático
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Batches");
        }
    }
}

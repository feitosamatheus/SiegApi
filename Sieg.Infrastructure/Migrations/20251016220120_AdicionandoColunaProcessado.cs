using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoColunaProcessado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processado",
                table: "Documentos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processado",
                table: "Documentos");
        }
    }
}

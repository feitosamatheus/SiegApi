using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoFiltroNoIndexHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos");

            migrationBuilder.CreateIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos",
                column: "XmlHash",
                unique: true,
                filter: "[Excluido] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos");

            migrationBuilder.CreateIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos",
                column: "XmlHash",
                unique: true);
        }
    }
}

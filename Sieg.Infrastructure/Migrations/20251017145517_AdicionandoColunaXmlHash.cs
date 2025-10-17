using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoColunaXmlHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documento_NomeOriginalArquivo",
                table: "Documentos");

            migrationBuilder.AddColumn<string>(
                name: "XmlHash",
                table: "Documentos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos",
                column: "XmlHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Documento_XmlHash",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "XmlHash",
                table: "Documentos");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_NomeOriginalArquivo",
                table: "Documentos",
                column: "NomeOriginalArquivo");
        }
    }
}

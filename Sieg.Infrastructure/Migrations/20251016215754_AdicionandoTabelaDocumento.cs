using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoTabelaDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaminhoXml",
                table: "DocumentosFiscais");

            migrationBuilder.DropColumn(
                name: "NomeOriginalArquivo",
                table: "DocumentosFiscais");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentoId",
                table: "DocumentosFiscais",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeOriginalArquivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CaminhoXml = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tamanho = table.Column<long>(type: "bigint", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    DataExclusao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosFiscais_DocumentoId",
                table: "DocumentosFiscais",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documento_NomeOriginalArquivo",
                table: "Documentos",
                column: "NomeOriginalArquivo");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentosFiscais_Documentos_DocumentoId",
                table: "DocumentosFiscais",
                column: "DocumentoId",
                principalTable: "Documentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentosFiscais_Documentos_DocumentoId",
                table: "DocumentosFiscais");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_DocumentosFiscais_DocumentoId",
                table: "DocumentosFiscais");

            migrationBuilder.DropColumn(
                name: "DocumentoId",
                table: "DocumentosFiscais");

            migrationBuilder.AddColumn<string>(
                name: "CaminhoXml",
                table: "DocumentosFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NomeOriginalArquivo",
                table: "DocumentosFiscais",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

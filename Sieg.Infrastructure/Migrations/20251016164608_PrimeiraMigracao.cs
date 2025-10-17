using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrimeiraMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentosFiscais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDocumento = table.Column<int>(type: "int", nullable: false),
                    CnpjEmitente = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DataEmissao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UfEmitente = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CaminhoXml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomeOriginalArquivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    DataExclusao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosFiscais", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosFiscais");
        }
    }
}

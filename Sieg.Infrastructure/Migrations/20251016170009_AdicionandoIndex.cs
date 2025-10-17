using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sieg.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UfEmitente",
                table: "DocumentosFiscais",
                type: "char(2)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "CnpjEmitente",
                table: "DocumentosFiscais",
                type: "varchar(14)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14);

            migrationBuilder.CreateIndex(
                name: "IX_DocFiscal_Cnpj_DataEmissao",
                table: "DocumentosFiscais",
                columns: new[] { "CnpjEmitente", "DataEmissao" });

            migrationBuilder.CreateIndex(
                name: "IX_DocFiscal_DataEmissao",
                table: "DocumentosFiscais",
                column: "DataEmissao");

            migrationBuilder.CreateIndex(
                name: "IX_DocFiscal_Uf_DataEmissao",
                table: "DocumentosFiscais",
                columns: new[] { "UfEmitente", "DataEmissao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocFiscal_Cnpj_DataEmissao",
                table: "DocumentosFiscais");

            migrationBuilder.DropIndex(
                name: "IX_DocFiscal_DataEmissao",
                table: "DocumentosFiscais");

            migrationBuilder.DropIndex(
                name: "IX_DocFiscal_Uf_DataEmissao",
                table: "DocumentosFiscais");

            migrationBuilder.AlterColumn<string>(
                name: "UfEmitente",
                table: "DocumentosFiscais",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldUnicode: false);

            migrationBuilder.AlterColumn<string>(
                name: "CnpjEmitente",
                table: "DocumentosFiscais",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(14)",
                oldUnicode: false);
        }
    }
}

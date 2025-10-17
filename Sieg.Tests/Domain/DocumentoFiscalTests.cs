using FluentAssertions;
using Sieg.Domain.Entities;
using Sieg.Domain.Enums;
using Sieg.Domain.Exceptions;
using Sieg.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Tests.Domain;

[TestFixture]
public class DocumentoFiscalTests
{
    private readonly Cnpj _cnpjValido = new("22831673000126");
    private readonly Uf _ufValida = new("SP");

    [Test]
    public void Deve_Atualizar_DocumentoFiscal_Com_Sucesso()
    {
        // Arrange
        var documentoFiscal = new DocumentoFiscal();

        var tipo = ETipoDocumentoFiscal.NFe;
        var dataEmissao = DateTimeOffset.UtcNow.AddDays(-1);
        var valorTotal = 1500.50m;

        // Act
        documentoFiscal.AtualizarDocumento(tipo, _cnpjValido, dataEmissao, _ufValida, valorTotal);

        // Assert
        documentoFiscal.TipoDocumento.Should().Be(tipo);
        documentoFiscal.CnpjEmitente.Should().Be(_cnpjValido);
        documentoFiscal.DataEmissao.Should().Be(dataEmissao);
        documentoFiscal.UfEmitente.Should().Be(_ufValida);
        documentoFiscal.ValorTotal.Should().Be(valorTotal);
    }

    [Test]
    public void Deve_Lancar_Excecao_Quando_TipoDocumento_For_Invalido()
    {
        // Arrange
        var documentoFiscal = new DocumentoFiscal();

        // Act
        Action act = () => documentoFiscal.AtualizarDocumento(
            (ETipoDocumentoFiscal)999, 
            _cnpjValido,
            DateTimeOffset.UtcNow.AddDays(-1),
            _ufValida,
            1000);

        // Assert
        act.Should()
            .Throw<DominioException>()
            .WithMessage("*Tipo de documento inválido*");
    }

    [Test]
    public void Deve_Lancar_Excecao_Quando_CnpjEmitente_For_Nulo()
    {
        var documentoFiscal = new DocumentoFiscal();

        Action act = () => documentoFiscal.AtualizarDocumento(
            ETipoDocumentoFiscal.NFe,
            null!,
            DateTimeOffset.UtcNow.AddDays(-1),
            _ufValida,
            1000);

        act.Should()
            .Throw<DominioException>()
            .WithMessage("*CNPJ do emitente não pode ser nulo*");
    }

    [Test]
    public void Deve_Lancar_Excecao_Quando_UfEmitente_For_Nula()
    {
        var documentoFiscal = new DocumentoFiscal();

        Action act = () => documentoFiscal.AtualizarDocumento(
            ETipoDocumentoFiscal.NFe,
            _cnpjValido,
            DateTimeOffset.UtcNow.AddDays(-1),
            null!,
            1000);

        act.Should()
            .Throw<DominioException>()
            .WithMessage("*UF do emitente não pode ser nula*");
    }

    [Test]
    public void Deve_Lancar_Excecao_Quando_DataEmissao_For_Futura()
    {
        var documentoFiscal = new DocumentoFiscal();

        Action act = () => documentoFiscal.AtualizarDocumento(
            ETipoDocumentoFiscal.NFe,
            _cnpjValido,
            DateTimeOffset.UtcNow.AddDays(1),
            _ufValida,
            1000);

        act.Should()
            .Throw<DominioException>()
            .WithMessage("*Data de emissão não pode ser futura*");
    }

    [Test]
    public void Deve_Lancar_Excecao_Quando_ValorTotal_For_Menor_Ou_Igual_A_Zero()
    {
        var documentoFiscal = new DocumentoFiscal();

        Action act = () => documentoFiscal.AtualizarDocumento(
            ETipoDocumentoFiscal.NFe,
            _cnpjValido,
            DateTimeOffset.UtcNow.AddDays(-1),
            _ufValida,
            0);

        act.Should()
            .Throw<DominioException>()
            .WithMessage("*Valor total deve ser maior que zero*");
    }
}

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using Moq;
using NUnit.Framework;
using Sieg.Api.Domain.Interfaces.Services;
using Sieg.Application.Dtos;
using Sieg.Application.UseCases.Documentos.SalvarDocumento;
using Sieg.Domain.Entities;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.Services;
using Sieg.Domain.Interfaces.UnitOfWork;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sieg.Application.Tests.UseCases.Documentos.SalvarDocumento;

[TestFixture]
public class SalvarDocumentoHandlerTests
{
    private Mock<IDocumentoRepository> _documentoRepositoryMock;
    private Mock<IArmazenamentoService> _armazenamentoServiceMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<SalvarDocumentoHandler>> _loggerMock;
    private Mock<IBrokerService> _brokerMock;

    private SalvarDocumentoHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _documentoRepositoryMock = new Mock<IDocumentoRepository>();
        _armazenamentoServiceMock = new Mock<IArmazenamentoService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SalvarDocumentoHandler>>();
        _brokerMock = new Mock<IBrokerService>();

        _handler = new SalvarDocumentoHandler(
            _documentoRepositoryMock.Object,
            _armazenamentoServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _brokerMock.Object
        );
    }

    private SalvarDocumentoCommand CriarComandoValido()
    {
        var xmlConteudo = "<xml><nota>123</nota></xml>";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlConteudo));

        var arquivo = new ArquivoUploadDTO(
            NomeArquivo: "nota.xml",
            TipoConteudo: "application/xml",
            Conteudo: stream,
            Tamanho: stream.Length);

        return new SalvarDocumentoCommand(arquivo);
    }

    [Test]
    public async Task Deve_Salvar_Documento_Com_Sucesso()
    {
        // Arrange
        var command = CriarComandoValido();
        _armazenamentoServiceMock.Setup(x => x.SalvarAsync(It.IsAny<MemoryStream>())).ReturnsAsync("/docs/nota.xml");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeTrue();
        result.Mensagem.Should().Be("Documento salvo com sucesso!");
        result.Documento.Should().NotBeNull();

        _documentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Documento>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _brokerMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Deve_Retornar_Falha_Quando_Xml_Malformado()
    {
        // Arrange
        var conteudoInvalido = new MemoryStream(Encoding.UTF8.GetBytes("<xml><nota>"));
        var arquivo = new ArquivoUploadDTO(
            NomeArquivo: "nota.xml",
            TipoConteudo: "application/xml",
            Conteudo: conteudoInvalido,
            Tamanho: conteudoInvalido.Length);

        var command = new SalvarDocumentoCommand(arquivo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Mensagem.Should().Contain("malformado");

        _documentoRepositoryMock.Verify(x => x.AdicionarAsync(It.IsAny<Documento>(), It.IsAny<CancellationToken>()), Times.Never);
        _brokerMock.Verify(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Deve_Retornar_Falha_Quando_Repositorio_Lancar_DominioException()
    {
        // Arrange
        var command = CriarComandoValido();

        _armazenamentoServiceMock
            .Setup(x => x.SalvarAsync(It.IsAny<MemoryStream>()))
            .ReturnsAsync("/docs/nota.xml");

        _documentoRepositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<Documento>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DominioException("Erro de regra de negócio"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Mensagem.Should().Be("Erro de regra de negócio");

        _loggerMock.VerifyLog(LogLevel.Warning, Times.Once());
    }

    [Test]
    public async Task Deve_Retornar_Falha_Quando_Ocorrer_Erro_Desconhecido()
    {
        // Arrange
        var command = CriarComandoValido();

        _armazenamentoServiceMock
            .Setup(x => x.SalvarAsync(It.IsAny<MemoryStream>()))
            .ThrowsAsync(new Exception("Erro inesperado"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Mensagem.Should().Contain("Falha ao processar o documento");

        _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
    }
}

public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> mock, LogLevel level, Times times)
    {
        mock.Verify(x => x.Log(
            level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
        ), times);
    }
}

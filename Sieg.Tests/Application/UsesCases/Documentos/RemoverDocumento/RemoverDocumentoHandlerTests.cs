using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieg.Application.UseCases.Documentos.RemoverDocumento;
using Sieg.Domain.Entities;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Tests.Application.UsesCases.Documentos.RemoverDocumento;

[TestFixture]
public class RemoverDocumentoHandlerTests
{
    private Mock<IDocumentoRepository> _documentoRepositoryMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<ILogger<RemoverDocumentoHandler>> _loggerMock;
    private RemoverDocumentoHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _documentoRepositoryMock = new Mock<IDocumentoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RemoverDocumentoHandler>>();

        _handler = new RemoverDocumentoHandler(
            _documentoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task Deve_Remover_Documento_Com_Sucesso()
    {
        // Arrange
        var documento = new Documento("nota.xml", "/docs/nota.xml", 512, "<xml></xml>");
        var command = new RemoverDocumentoCommand(documento.Id);

        _documentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(documento.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(documento);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeTrue();
        result.Mensagem.Should().Be("Documento removido com sucesso");

        _documentoRepositoryMock.Verify(x => x.Atualizar(documento), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Deve_Retornar_Falha_Quando_Documento_Nao_Encontrado()
    {
        // Arrange
        var command = new RemoverDocumentoCommand(Guid.NewGuid());

        _documentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Documento?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Mensagem.Should().Be("Documento não encontrado.");

        _documentoRepositoryMock.Verify(x => x.Atualizar(It.IsAny<Documento>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Deve_Retornar_Falha_Quando_Ocorrer_Erro_Desconhecido()
    {
        // Arrange
        var command = new RemoverDocumentoCommand(Guid.NewGuid());

        _documentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Erro inesperado"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Sucesso.Should().BeFalse();
        result.Mensagem.Should().Contain("Falha ao remover o documento");

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

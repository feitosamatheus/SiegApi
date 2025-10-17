using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Domain.Entities;
using Sieg.Domain.Interfaces.Repositories;

namespace Sieg.Application.UseCases.Documentos.ObterDocumentoPorId;

public sealed class ObterDocumentoPorIdHandler : IRequestHandler<ObterDocumentoPorIdCommand, ObterDocumentoPorIdResult>
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILogger<ObterDocumentoPorIdHandler> _logger;

    public ObterDocumentoPorIdHandler(IDocumentoRepository documentoRepository, ILogger<ObterDocumentoPorIdHandler> logger)
    {
        _documentoRepository = documentoRepository;
        _logger = logger;
    }

    public async Task<ObterDocumentoPorIdResult> Handle(ObterDocumentoPorIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documento = await _documentoRepository.ObterPorIdAsync(request.DocumentId, cancellationToken);
            if (documento == null)
                return new ObterDocumentoPorIdResult(false, "Documento não encontrado");

            var documentoDto = new DocumentoDTO(
                documento.Id,
                documento.NomeOriginalArquivo,
                documento.CaminhoXml,
                documento.Tamanho,
                documento.DataCriacao,
                documento.XmlHash
            );

            return new ObterDocumentoPorIdResult(true, "Documento encontrado com sucesso", documentoDto);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Erro inesperado ao consultar o documento {DocumentoId}: {Mensagem}", request.DocumentId, ex.Message);
            return new ObterDocumentoPorIdResult(false, "Falha ao consultar o documento. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

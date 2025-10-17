using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Domain.Entities;
using Sieg.Domain.Interfaces.Repositories;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterDocumentoFiscalPorId;

public sealed class ObterDocumentoFiscalPorIdHandler : IRequestHandler<ObterDocumentoFiscalPorIdCommand, ObterDocumentoFiscalPorIdResult>
{
    private readonly IDocumentoFiscalRepository _documentoFiscalRepository;
    private readonly ILogger<ObterDocumentoFiscalPorIdHandler> _logger;

    public ObterDocumentoFiscalPorIdHandler(IDocumentoFiscalRepository documentoFiscalRepository, ILogger<ObterDocumentoFiscalPorIdHandler> logger)
    {
        _documentoFiscalRepository = documentoFiscalRepository;
        _logger = logger;
    }

    public async Task<ObterDocumentoFiscalPorIdResult> Handle(ObterDocumentoFiscalPorIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documentoFiscal = await _documentoFiscalRepository.ObterPorIdAsync(request.DocumentoFiscalId, cancellationToken);
            if (documentoFiscal == null)
                return new ObterDocumentoFiscalPorIdResult(false, "Documento Fiscal não encontrado");

            var documentoFiscalDto = new DocumentoFiscalDTO(
                documentoFiscal.Id,
                documentoFiscal.DocumentoId,
                documentoFiscal.TipoDocumento,
                documentoFiscal.CnpjEmitente.Value,
                documentoFiscal.DataEmissao,
                documentoFiscal.UfEmitente.Value,
                documentoFiscal.ValorTotal
            );

            return new ObterDocumentoFiscalPorIdResult(true, "Documento Fiscal encontrado com sucesso", documentoFiscalDto);
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "Erro inesperado ao consultar o documento fiscal. Id: {DocumentoFiscalId} - Mensagem: {Mensagem}", request.DocumentoFiscalId, ex.Message);
            return new ObterDocumentoFiscalPorIdResult(false, "Falha ao consultar o documento fiscal. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

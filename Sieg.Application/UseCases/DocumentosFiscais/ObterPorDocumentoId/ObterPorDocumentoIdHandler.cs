using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Application.UseCases.DocumentosFiscais.ObterTodosDocumentosFiscais;
using Sieg.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterPorDocumentoId;

public sealed class ObterPorDocumentoIdHandler : IRequestHandler<ObterPorDocumentoIdCommand, ObterPorDocumentoIdResult>
{
    private readonly IDocumentoFiscalRepository _documentoFiscalRepository;
    private readonly ILogger<ObterPorDocumentoIdHandler> _logger;


    public ObterPorDocumentoIdHandler(IDocumentoFiscalRepository documentoFiscalRepository, ILogger<ObterPorDocumentoIdHandler> logger)
    {
        _documentoFiscalRepository = documentoFiscalRepository;
        _logger = logger;
    }

    public async Task<ObterPorDocumentoIdResult> Handle(ObterPorDocumentoIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documentoFiscal = await _documentoFiscalRepository.ObterPorDocumentoIdAsync(request.DocumentoId, cancellationToken);

            if (documentoFiscal == null)
                return new ObterPorDocumentoIdResult(false, "Documento fiscal não encontrado para o DocumentoId fornecido.");

            var documentoFiscalDto = new DocumentoFiscalDTO(
                documentoFiscal.Id, 
                documentoFiscal.DocumentoId, 
                documentoFiscal.TipoDocumento, 
                documentoFiscal.CnpjEmitente.Value ?? string.Empty,
                documentoFiscal.DataEmissao, 
                documentoFiscal.UfEmitente.Value ?? string.Empty, 
                documentoFiscal.ValorTotal);

            return new ObterPorDocumentoIdResult(true, "Documento fiscal obtido com sucesso.", documentoFiscalDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao obter o documento fiscal para o DocumentoId fornecido. DocumentoId: {DocumentoId} - Mensagem: {Mensagem}", request.DocumentoId, ex.Message);
            return new ObterPorDocumentoIdResult(false, "Falha ao obter o documento fiscal para o DocumentoId fornecido. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

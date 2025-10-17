using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.UnitOfWork;

namespace Sieg.Application.UseCases.DocumentosFiscais.RemoverDocumentoFiscal;

public sealed class RemoverDocumentoFiscalHandler : IRequestHandler<RemoverDocumentoFiscalCommand, RemoverDocumentoFiscalResult>
{
    private readonly IDocumentoFiscalRepository _documentoFiscalRepository;
    private readonly ILogger<RemoverDocumentoFiscalResult> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverDocumentoFiscalHandler(IDocumentoFiscalRepository documentoFiscalRepository, IUnitOfWork unitOfWork, ILogger<RemoverDocumentoFiscalResult> logger)
    {
        _documentoFiscalRepository = documentoFiscalRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RemoverDocumentoFiscalResult> Handle(RemoverDocumentoFiscalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documento = await _documentoFiscalRepository.ObterPorIdAsync(request.DocumentoFiscalId, cancellationToken);
            if (documento == null)
                return new RemoverDocumentoFiscalResult(false, "Documento fiscal não encontrado.");

            documento.Excluir();
            _documentoFiscalRepository.Atualizar(documento);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RemoverDocumentoFiscalResult(true, "Documento fiscal removido com sucesso");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover o documento fiscal. Id: {DocumentoFiscalId} - Mensagem: {Mensagem}", request.DocumentoFiscalId, ex.Message);
            return new RemoverDocumentoFiscalResult(false, "Falha ao remover o documento fiscal. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

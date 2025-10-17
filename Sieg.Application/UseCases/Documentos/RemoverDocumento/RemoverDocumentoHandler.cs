using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.UnitOfWork;

namespace Sieg.Application.UseCases.Documentos.RemoverDocumento;

public sealed class RemoverDocumentoHandler : IRequestHandler<RemoverDocumentoCommand, RemoverDocumentoResult>
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILogger<RemoverDocumentoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RemoverDocumentoHandler(IDocumentoRepository documentoRepository, IUnitOfWork unitOfWork, ILogger<RemoverDocumentoHandler> logger)
    {
        _documentoRepository = documentoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<RemoverDocumentoResult> Handle(RemoverDocumentoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documento = await _documentoRepository.ObterPorIdAsync(request.Id, cancellationToken);
            if (documento == null)
                return new RemoverDocumentoResult(false, "Documento não encontrado.");

            documento.Excluir();
            _documentoRepository.Atualizar(documento);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RemoverDocumentoResult(true, "Documento removido com sucesso");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao remover o documento {DocumentoId}: {Mensagem}", request.Id, ex.Message);
            return new RemoverDocumentoResult(false, "Falha ao remover o documento. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

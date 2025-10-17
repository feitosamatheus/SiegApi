using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Domain.Entities;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.UnitOfWork;
using Sieg.Domain.ValueObjects;

namespace Sieg.Application.UseCases.Documentos.AtualizarDocumento;

public sealed class AtualizarDocumentoHandler : IRequestHandler<AtualizarDocumentoCommand, AtualizarDocumentoResult>
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILogger<AtualizarDocumentoHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarDocumentoHandler(IDocumentoRepository documentoRepository, ILogger<AtualizarDocumentoHandler> logger, IUnitOfWork unitOfWork)
    {
        _documentoRepository = documentoRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtualizarDocumentoResult> Handle(AtualizarDocumentoCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documento = await _documentoRepository.ObterPorIdAsync(request.Dto.Id, cancellationToken);
            if (documento == null)
                return new AtualizarDocumentoResult(false, "Documento não encontrado");

            documento.AtualizarDocumento(request.Dto.NomeArquivo, request.Dto.CaminhoXml);

            _documentoRepository.Atualizar(documento);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var documentoDto = new DocumentoDTO(
                documento.Id,
                documento.NomeOriginalArquivo,
                documento.CaminhoXml,
                documento.Tamanho,
                documento.DataCriacao,
                documento.XmlHash
            );

            return new AtualizarDocumentoResult(true, "Documento atualizado com sucesso", documentoDto);
        }
        catch (DominioException ex)
        {
            _logger.LogWarning(ex, "Violação de regra de negócio na atualização documento {DocumentoId}: {Mensagem}", request.Dto.Id, ex.Message);
            return new AtualizarDocumentoResult(false, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar o documento {DocumentoId}: {Mensagem}", request.Dto.Id, ex.Message);
            return new AtualizarDocumentoResult(false, "Falha ao atualizar o documento. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Domain.Entities;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.UnitOfWork;
using Sieg.Domain.ValueObjects;

namespace Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;

public sealed class AtualizarDocumentoFiscalHandler : IRequestHandler<AtualizarDocumentoFiscalCommand, AtualizarDocumentoFiscalResult>
{
    private readonly IDocumentoFiscalRepository _documentoFiscalRepository;
    private readonly ILogger<AtualizarDocumentoFiscalResult> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AtualizarDocumentoFiscalHandler(IDocumentoFiscalRepository documentoFiscalRepository, ILogger<AtualizarDocumentoFiscalResult> logger, IUnitOfWork unitOfWork)
    {
        _documentoFiscalRepository = documentoFiscalRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<AtualizarDocumentoFiscalResult> Handle(AtualizarDocumentoFiscalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var documentoFiscal = await _documentoFiscalRepository.ObterPorIdAsync(request.Dto.DocumentoFiscalId, cancellationToken);
            if (documentoFiscal == null)
                return new AtualizarDocumentoFiscalResult(false, "Documento Fiscal não encontrado");

            var cnpjVo= new Cnpj(request.Dto.CnpjEmitente);
            var ufVo= new Uf(request.Dto.UfEmitente);

            documentoFiscal.AtualizarDocumento(request.Dto.TipoDocumento, cnpjVo, request.Dto.DataEmissao, ufVo, request.Dto.ValorTotal);

            _documentoFiscalRepository.Atualizar(documentoFiscal);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var documentoFiscalDto = new DocumentoFiscalDTO(
                documentoFiscal.Id,
                documentoFiscal.DocumentoId,
                documentoFiscal.TipoDocumento,
                documentoFiscal.CnpjEmitente.Value,
                documentoFiscal.DataEmissao,
                documentoFiscal.UfEmitente.Value,
                documentoFiscal.ValorTotal
            );

            return new AtualizarDocumentoFiscalResult(true, "Documento Fiscal atualizado com sucesso", documentoFiscalDto);
        }
        catch (DominioException ex)
        {
            _logger.LogWarning(ex, "Violação de regra de negócio na atualização do documento fiscal. Id: {DocumentoId} - Mensagem: {Mensagem}", request.Dto.DocumentoFiscalId, ex.Message);
            return new AtualizarDocumentoFiscalResult(false, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar o documento fiscal. Id: {DocumentoFiscalId} - Mensagem: {Mensagem}", request.Dto.DocumentoFiscalId, ex.Message);
            return new AtualizarDocumentoFiscalResult(false, "Falha ao atualizar o documento discal. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

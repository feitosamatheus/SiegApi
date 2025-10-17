using Sieg.Api.Application.Dtos;
using Sieg.Application.Dtos;
using Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;
using Sieg.Application.UseCases.DocumentosFiscais.ObterDocumentoFiscalPorId;
using Sieg.Application.UseCases.DocumentosFiscais.ObterPorDocumentoId;
using Sieg.Application.UseCases.DocumentosFiscais.ObterTodosDocumentosFiscais;
using Sieg.Application.UseCases.DocumentosFiscais.RemoverDocumentoFiscal;

namespace Sieg.Application.Interfaces;

public interface IDocumentoFiscalService
{
    Task<ObterDocumentoFiscalPorIdResult> ObterDocumentoFiscalPorIdAsync(Guid documentoFiscalId, CancellationToken cancellation);
    Task<ObterTodosDocumentosFiscaisResult> ObterTodosDocumentosFiscaisAsync(FiltroDocumentoFiscalDTO dto, CancellationToken cancellationToken);
    Task<AtualizarDocumentoFiscalResult> AtualizarDocumentoFiscalAsync(DocumentoFiscalAtualizarDTO dto, CancellationToken cancellationToken);
    Task<RemoverDocumentoFiscalResult> RemoverDocumentoFiscalAsync(Guid documentoFiscalId, CancellationToken cancellationToken);
    Task<ObterPorDocumentoIdResult> ObterDocumentoFiscalPorDocumentoIdAsync(Guid documentoId, CancellationToken cancellationToken);
}

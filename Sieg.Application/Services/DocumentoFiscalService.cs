using MediatR;
using Sieg.Api.Application.Dtos;
using Sieg.Application.Dtos;
using Sieg.Application.Interfaces;
using Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;
using Sieg.Application.UseCases.DocumentosFiscais.ObterDocumentoFiscalPorId;
using Sieg.Application.UseCases.DocumentosFiscais.ObterPorDocumentoId;
using Sieg.Application.UseCases.DocumentosFiscais.ObterTodosDocumentosFiscais;
using Sieg.Application.UseCases.DocumentosFiscais.RemoverDocumentoFiscal;

namespace Sieg.Application.Services;

public sealed class DocumentoFiscalService : IDocumentoFiscalService
{
    private readonly IMediator _mediator;

    public DocumentoFiscalService(IMediator mediator)
        => _mediator = mediator;

    public async Task<AtualizarDocumentoFiscalResult> AtualizarDocumentoFiscalAsync(DocumentoFiscalAtualizarDTO dto, CancellationToken cancellationToken)
        => await _mediator.Send(new AtualizarDocumentoFiscalCommand(dto), cancellationToken);

    public async Task<RemoverDocumentoFiscalResult> RemoverDocumentoFiscalAsync(Guid documentoFiscalId, CancellationToken cancellationToken)
        => await _mediator.Send(new RemoverDocumentoFiscalCommand(documentoFiscalId), cancellationToken);

    public async Task<ObterDocumentoFiscalPorIdResult> ObterDocumentoFiscalPorIdAsync(Guid documentoFiscalId, CancellationToken cancellation)
        => await _mediator.Send(new ObterDocumentoFiscalPorIdCommand(documentoFiscalId), cancellation);

    public async Task<ObterTodosDocumentosFiscaisResult> ObterTodosDocumentosFiscaisAsync(FiltroDocumentoFiscalDTO dto, CancellationToken cancellationToken)
        => await _mediator.Send(new ObterTodosDocumentosFiscaisCommand(dto), cancellationToken);

    public async Task<ObterPorDocumentoIdResult> ObterDocumentoFiscalPorDocumentoIdAsync(Guid documentoId, CancellationToken cancellationToken)
        => await _mediator.Send(new ObterPorDocumentoIdCommand(documentoId), cancellationToken);
}

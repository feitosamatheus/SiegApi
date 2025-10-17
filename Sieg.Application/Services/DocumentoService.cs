using MediatR;
using Sieg.Application.Dtos;
using Sieg.Application.Interfaces;
using Sieg.Application.UseCases.Documentos.AtualizarDocumento;
using Sieg.Application.UseCases.Documentos.ObterDocumentoPorId;
using Sieg.Application.UseCases.Documentos.ObterTodosDocumentos;
using Sieg.Application.UseCases.Documentos.RemoverDocumento;
using Sieg.Application.UseCases.Documentos.SalvarDocumento;
using Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;
using Sieg.Application.UseCases.DocumentosFiscais.ObterDocumentoFiscalPorId;
using Sieg.Application.UseCases.DocumentosFiscais.RemoverDocumentoFiscal;

namespace Sieg.Application.Services;

public class DocumentoService : IDocumentoService
{
    private readonly IMediator _mediator;

    public DocumentoService(IMediator mediator)
        => _mediator = mediator;

    public async Task<SalvarDocumentoResult> SalvarDocumentoAsync(ArquivoUploadDTO dto, CancellationToken cancellationToken)
        => await _mediator.Send(new SalvarDocumentoCommand(dto), cancellationToken);

    public async Task<AtualizarDocumentoResult> AtualizarDocumentoAsync(DocumentoUpdateDTO dto, CancellationToken cancellationToken)
        => await _mediator.Send(new AtualizarDocumentoCommand(dto), cancellationToken);

    public async Task<RemoverDocumentoResult> RemoverDocumentoAsync(Guid documentoId, CancellationToken cancellationToken)
        => await _mediator.Send(new RemoverDocumentoCommand(documentoId), cancellationToken);

    public async Task<ObterDocumentoPorIdResult> ObterDocumentoPorIdAsync(Guid documentoId, CancellationToken cancellationToken)
        => await _mediator.Send(new ObterDocumentoPorIdCommand(documentoId), cancellationToken);

    public async Task<ObterTodosDocumentosResult> ObterTodosDocumentosAsync(FiltroDocumentoDTO filtroDto, CancellationToken cancellationToken)
        => await _mediator.Send(new ObterTodosDocumentosCommand(filtroDto), cancellationToken);

}

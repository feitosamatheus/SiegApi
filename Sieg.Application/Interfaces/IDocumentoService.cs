using Sieg.Application.Dtos;
using Sieg.Application.UseCases.Documentos.AtualizarDocumento;
using Sieg.Application.UseCases.Documentos.ObterDocumentoPorId;
using Sieg.Application.UseCases.Documentos.ObterTodosDocumentos;
using Sieg.Application.UseCases.Documentos.RemoverDocumento;
using Sieg.Application.UseCases.Documentos.SalvarDocumento;

namespace Sieg.Application.Interfaces;

public interface IDocumentoService
{
    Task<SalvarDocumentoResult> SalvarDocumentoAsync(ArquivoUploadDTO dto, CancellationToken cancellationToken);
    Task<AtualizarDocumentoResult> AtualizarDocumentoAsync(DocumentoUpdateDTO documentoUpdateDto, CancellationToken cancellationToken);
    Task<RemoverDocumentoResult> RemoverDocumentoAsync(Guid documentoId, CancellationToken cancellationToken);
    Task<ObterTodosDocumentosResult> ObterTodosDocumentosAsync(FiltroDocumentoDTO filtroDto, CancellationToken cancellationToken);
    Task<ObterDocumentoPorIdResult> ObterDocumentoPorIdAsync(Guid documentoId, CancellationToken cancellationToken);
}

using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.ValueObjects;

namespace Sieg.Application.UseCases.Documentos.ObterTodosDocumentos;

public sealed class ObterTodosDocumentosHandler : IRequestHandler<ObterTodosDocumentosCommand, ObterTodosDocumentosResult>
{
    private readonly IDocumentoRepository _documentoRepository;
    private readonly ILogger<ObterTodosDocumentosHandler> _logger;

    public ObterTodosDocumentosHandler(IDocumentoRepository documentoRepository, ILogger<ObterTodosDocumentosHandler> logger)
    {
        _documentoRepository = documentoRepository;
        _logger = logger;
    }

    public async Task<ObterTodosDocumentosResult> Handle(ObterTodosDocumentosCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var (Itens, TotalPaginas, TotalRegistros, PaginaAtual, TamanhoPagina) = await _documentoRepository.ObterTodosAsync(request.FiltroDocumentoDTO.Pagina, request.FiltroDocumentoDTO.Tamanho, request.FiltroDocumentoDTO.Nome, request.FiltroDocumentoDTO.DataCriacao, cancellationToken);

            var itemsDto = Itens.Select(d => new DocumentoDTO(
                d.Id,
                d.NomeOriginalArquivo,
                d.CaminhoXml,
                d.Tamanho,
                d.DataCriacao,
                d.XmlHash
            ));

            var pagedDto = new DocumentoPagedDTO(
                Itens: itemsDto,
                TotalPaginas: TotalPaginas,
                TotalRegistros: TotalRegistros,
                PaginaAtual: PaginaAtual,
                TamanhoPagina: TamanhoPagina
            );

            return new ObterTodosDocumentosResult(true, "Documentos carregados com sucesso.", pagedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao filtrar os documentos: {Mensagem}", ex.Message);
            return new ObterTodosDocumentosResult(false, "Falha ao filtrar os documentos. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

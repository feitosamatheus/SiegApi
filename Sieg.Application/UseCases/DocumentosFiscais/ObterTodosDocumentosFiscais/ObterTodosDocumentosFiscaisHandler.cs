using MediatR;
using Microsoft.Extensions.Logging;
using Sieg.Application.Dtos;
using Sieg.Application.UseCases.Documentos.ObterTodosDocumentos;
using Sieg.Domain.Exceptions;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.ValueObjects;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterTodosDocumentosFiscais;

public sealed class ObterTodosDocumentosFiscaisHandler : IRequestHandler<ObterTodosDocumentosFiscaisCommand, ObterTodosDocumentosFiscaisResult>
{
    private readonly IDocumentoFiscalRepository _documentoFiscalRepository;
    private readonly ILogger<ObterTodosDocumentosFiscaisHandler> _logger;

    public ObterTodosDocumentosFiscaisHandler(IDocumentoFiscalRepository documentoFiscalRepository, ILogger<ObterTodosDocumentosFiscaisHandler> logger)
    {
        _documentoFiscalRepository = documentoFiscalRepository;
        _logger = logger;
    }

    public async Task<ObterTodosDocumentosFiscaisResult> Handle(ObterTodosDocumentosFiscaisCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var filtro = new DocumentoFiscalFiltro(
                CnpjEmitente: request.FiltroDocumentoFiscalDTO.CnpjEmitente,
                UfEmitente: request.FiltroDocumentoFiscalDTO.UfEmitente,
                DataInicio: request.FiltroDocumentoFiscalDTO.DataInicio,
                DataFim: request.FiltroDocumentoFiscalDTO.DataFim
            );

            var (Itens, TotalPaginas, TotalRegistros, PaginaAtual, TamanhoPagina) = await _documentoFiscalRepository.ObterTodosAsync(filtro, request.FiltroDocumentoFiscalDTO.PaginaAtual, request.FiltroDocumentoFiscalDTO.TamanhoPagina, cancellationToken);

            var itemsDto = Itens.Select(d => new DocumentoFiscalDTO(
                d.Id,
                d.DocumentoId,
                d.TipoDocumento,
                d.CnpjEmitente != null ? d.CnpjEmitente.Value : string.Empty,
                d.DataEmissao,
                d.UfEmitente != null ? d.UfEmitente.Value : string.Empty,
                d.ValorTotal
            ));

            var pagedDto = new DocumentoFiscalPagedDTO(
                Itens: itemsDto,
                TotalPaginas: TotalPaginas,
                TotalRegistros: TotalRegistros,
                PaginaAtual: PaginaAtual,
                TamanhoPagina: TamanhoPagina
            );

            return new ObterTodosDocumentosFiscaisResult(true, "Documentos fiscais carregados com sucesso.", pagedDto);
        }
        catch (DominioException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio ao filtrar os documentos: {Mensagem}", ex.Message);

            var pagedDtoVazio = new DocumentoFiscalPagedDTO(
                Itens: new List<DocumentoFiscalDTO>(),
                TotalPaginas: 0,
                TotalRegistros: 0,
                PaginaAtual: 1,
                TamanhoPagina: 10);

            return new ObterTodosDocumentosFiscaisResult(true, "Nenhum documento fiscal encontrado.", pagedDtoVazio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao filtrar documentos fiscais. Mensagem: {Mensagem}", ex.Message);
            return new ObterTodosDocumentosFiscaisResult(false, "Falha ao filtrar documentos fiscais. Por favor, tente novamente ou contate o suporte.");
        }
    }
}

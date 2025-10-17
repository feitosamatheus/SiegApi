using Microsoft.AspNetCore.Mvc;
using Sieg.Api.Application.Dtos;
using Sieg.API.Requests;
using Sieg.Application.Dtos;
using Sieg.Application.Interfaces;
using System.Threading;

namespace Sieg.API.Controllers;

[Route("api/documentos-fiscais")]
[ApiController]
public class DocumentosFiscaisController : ControllerBase
{
    private readonly IDocumentoFiscalService _documentoFiscalService;
    public DocumentosFiscaisController(IDocumentoFiscalService documentoFiscalService)
        => _documentoFiscalService = documentoFiscalService;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterDocumentoFiscalPorId(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _documentoFiscalService.ObterDocumentoFiscalPorIdAsync(id, cancellationToken);
        if (!resultado.Success)
        {
            if (resultado.Message.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = resultado.Message });

            return StatusCode(500, new { message = resultado.Message });
        }

        return Ok(resultado.DocumentoFiscalDTO);
    }

    [HttpGet]
    public async Task<IActionResult> ObterDocumentosFiscais([FromQuery] FiltroDocumentoFiscalRequest request, CancellationToken cancellationToken)
    {
        var filtroDto = new FiltroDocumentoFiscalDTO(request.Pagina, request.Tamanho, request.CnpjEmitente, request.UfEmitente, request.DataInicio, request.DataFim);

        var resultado = await _documentoFiscalService.ObterTodosDocumentosFiscaisAsync(filtroDto, cancellationToken);

        if (!resultado.Success)
        {
            return StatusCode(500, new { message = resultado.Message });
        }

        return Ok(resultado.DocumentoFiscalPagedDTO);
    }

    [HttpGet("por-documento/{documentoId:guid}")]
    public async Task<IActionResult> ObterDocumentoFiscalPorDocumentoId(Guid documentoId, CancellationToken cancellationToken)
    {
        var resultado = await _documentoFiscalService.ObterDocumentoFiscalPorDocumentoIdAsync(documentoId, cancellationToken);

        if (!resultado.Sucesso)
        {
            if (resultado.Mensagem.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { message = resultado.Mensagem });

            return StatusCode(500, new { message = resultado.Mensagem }); 
        }

        return Ok(resultado.DocumentoFiscal);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarDocumentoFiscal(Guid id, [FromBody] DocumentoFiscalRequest request, CancellationToken cancellationToken)
    {
        var documentoDto = new DocumentoFiscalAtualizarDTO(id, request.TipoDocumento, request.CnpjEmitente, request.DataEmissao, request.UfEmitente, request.ValorTotal);

        var resultado = await _documentoFiscalService.AtualizarDocumentoFiscalAsync(documentoDto, cancellationToken);

        if (!resultado.Sucesso)
        {
            if (resultado.Mensagem.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { mensagem = resultado.Mensagem });
            
            if (resultado.Mensagem.Contains("falha", StringComparison.OrdinalIgnoreCase))
                return StatusCode(500, new { mensagem = resultado.Mensagem });

            return BadRequest(new { mensagem = resultado.Mensagem });
        }

        return Ok(resultado.DocumentoFiscalDTO);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoverDocumentoFiscal(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _documentoFiscalService.RemoverDocumentoFiscalAsync(id, cancellationToken);

        if (!resultado.Sucesso)
        {
            if (resultado.Mensagem.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { mensagem = resultado.Mensagem });

            if (resultado.Mensagem.Contains("falha", StringComparison.OrdinalIgnoreCase))
                return StatusCode(500, new { mensagem = resultado.Mensagem });

            return BadRequest(new { mensagem = resultado.Mensagem });
        }

        return NoContent();
    }
}

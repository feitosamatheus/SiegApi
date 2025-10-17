using Microsoft.AspNetCore.Mvc;
using Sieg.API.Requests;
using Sieg.Application.Dtos;
using Sieg.Application.Interfaces;
using Sieg.Application.Services;
using System.Threading;

namespace Sieg.API.Controllers;

[ApiController]
[Route("api/documentos")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoService _documentoService;
    public DocumentosController(IDocumentoService documentoService)
        => _documentoService = documentoService;

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocumento([FromForm] ArquivoUploadRequest request, CancellationToken cancellationToken)
    {
        var arquivo = request.Arquivo;

        if (arquivo == null || Path.GetExtension(arquivo.FileName).ToLower() != ".xml")
            return BadRequest("Arquivo inválido. Apenas XML é permitido.");

        using var memoria = new MemoryStream();
        await arquivo.CopyToAsync(memoria, cancellationToken);
        memoria.Position = 0;

        var dto = new ArquivoUploadDTO(arquivo.FileName, arquivo.ContentType, memoria, arquivo.Length);

        var resultado = await _documentoService.SalvarDocumentoAsync(dto, cancellationToken);

        if (!resultado.Sucesso)
            return BadRequest(new { message = resultado.Mensagem });

        return CreatedAtAction(nameof(ObterDocumentoPorId), new { id = resultado.Documento?.Id }, resultado.Documento);
    }

    [HttpGet]
    public async Task<IActionResult> ObterDocumentos([FromQuery] FiltroDocumentoRequest request, CancellationToken cancellationToken)
    {
        var filtroDto = new FiltroDocumentoDTO(request.Pagina, request.Tamanho, request.Nome, request.DataCriacao);

        var resultado = await _documentoService.ObterTodosDocumentosAsync(filtroDto, cancellationToken);

        if (!resultado.Success)
        {
            return StatusCode(500, new { message = resultado.Message });
        }

        return Ok(resultado.DocumentoPagedDTO);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterDocumentoPorId(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _documentoService.ObterDocumentoPorIdAsync(id, cancellationToken);
        if (!resultado.Success)
        {
            if (resultado.Message.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = resultado.Message });

            return StatusCode(500, new { message = resultado.Message });
        }

        return Ok(resultado.DocumentoDTO);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> AtualizarDocumento(Guid id, [FromBody] DocumentoUpdateRequest request, CancellationToken cancellationToken)
    {
        var documentoUpdateDto = new DocumentoUpdateDTO(id, request.NomeArquivo, request.CaminhoXml);

        var resultado = await _documentoService.AtualizarDocumentoAsync(documentoUpdateDto, cancellationToken);

        if (!resultado.Sucesso)
        {
            if (resultado.Mensagem.Contains("encontrado", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { mensagem = resultado.Mensagem });

            if (resultado.Mensagem.Contains("falha", StringComparison.OrdinalIgnoreCase))
                return StatusCode(500, new { mensagem = resultado.Mensagem });

            return BadRequest(new { mensagem = resultado.Mensagem });
        }

        return Ok(resultado.DocumentoDTO);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoverDocumento(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _documentoService.RemoverDocumentoAsync(id, cancellationToken);

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


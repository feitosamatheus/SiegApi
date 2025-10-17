using Sieg.Application.Dtos;
using Sieg.Domain.Entities;

namespace Sieg.Application.UseCases.Documentos.ObterTodosDocumentos;

public sealed record ObterTodosDocumentosResult(bool Success, string Message, DocumentoPagedDTO? DocumentoPagedDTO = null);

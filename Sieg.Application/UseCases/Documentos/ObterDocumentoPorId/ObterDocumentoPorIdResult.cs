using Sieg.Application.Dtos;
using Sieg.Domain.Entities;

namespace Sieg.Application.UseCases.Documentos.ObterDocumentoPorId;

public sealed record ObterDocumentoPorIdResult(bool Success, string Message, DocumentoDTO? DocumentoDTO = null);

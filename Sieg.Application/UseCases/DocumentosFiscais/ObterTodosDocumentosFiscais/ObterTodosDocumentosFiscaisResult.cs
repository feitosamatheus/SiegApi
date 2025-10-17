using Sieg.Application.Dtos;
using Sieg.Domain.Entities;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterTodosDocumentosFiscais;

public sealed record ObterTodosDocumentosFiscaisResult(bool Success, string Message, DocumentoFiscalPagedDTO? DocumentoFiscalPagedDTO = null);

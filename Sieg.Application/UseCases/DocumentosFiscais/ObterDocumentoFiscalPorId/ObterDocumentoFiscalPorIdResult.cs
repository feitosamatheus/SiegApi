using Sieg.Application.Dtos;
using Sieg.Domain.Entities;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterDocumentoFiscalPorId;

public sealed record ObterDocumentoFiscalPorIdResult(bool Success, string Message, DocumentoFiscalDTO? DocumentoFiscalDTO = null);

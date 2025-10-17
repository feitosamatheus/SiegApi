using Sieg.Application.Dtos;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterPorDocumentoId;

public sealed record ObterPorDocumentoIdResult(bool Sucesso, string Mensagem, DocumentoFiscalDTO? DocumentoFiscal = null);

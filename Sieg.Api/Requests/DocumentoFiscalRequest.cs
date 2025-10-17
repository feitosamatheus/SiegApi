using Sieg.Domain.Enums;

namespace Sieg.API.Requests;

public sealed record DocumentoFiscalRequest(
    ETipoDocumentoFiscal TipoDocumento,
    string CnpjEmitente,
    DateTimeOffset DataEmissao,
    string UfEmitente,
    decimal ValorTotal);

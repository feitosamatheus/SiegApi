namespace Sieg.API.Requests;

public record FiltroDocumentoFiscalRequest(
    int Pagina = 1,
    int Tamanho = 10,
    string? CnpjEmitente = null,
    string? UfEmitente = null,
    DateTime? DataInicio = null,
    DateTime? DataFim = null);
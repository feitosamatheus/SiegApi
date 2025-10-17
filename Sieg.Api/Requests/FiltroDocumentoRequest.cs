namespace Sieg.API.Requests;

public record FiltroDocumentoRequest(
    int Pagina = 1, 
    int Tamanho = 10, 
    string? Nome = null, 
    DateTime? DataCriacao = null);

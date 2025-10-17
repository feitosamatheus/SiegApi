using Sieg.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Domain.Interfaces.Repositories;

public interface IDocumentoRepository
{
    Task AdicionarAsync(Documento documento, CancellationToken cancellationToken);
    void Atualizar(Documento documento);
    Task<Documento?> ObterPorIdAsync(Guid documentId, CancellationToken cancellationToken);
    Task<(IEnumerable<Documento> Itens, int TotalPaginas, int TotalRegistros, int PaginaAtual, int TamanhoPagina)> ObterTodosAsync(int pagina, int tamanho, string nome, DateTime? dataCriacao, CancellationToken cancellationToken);
}

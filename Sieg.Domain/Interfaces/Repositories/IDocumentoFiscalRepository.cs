using Sieg.Domain.Entities;
using Sieg.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Domain.Interfaces.Repositories;

public interface IDocumentoFiscalRepository
{
    Task AdicionarAsync(DocumentoFiscal documentoFiscal, CancellationToken cancellationToken);
    Task<DocumentoFiscal?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<DocumentoFiscal?> ObterPorDocumentoIdAsync(Guid documentoId, CancellationToken cancellationToken);
    Task<(IEnumerable<DocumentoFiscal> Itens, int TotalPaginas, int TotalRegistros, int PaginaAtual, int TamanhoPagina)> ObterTodosAsync(DocumentoFiscalFiltro filtro, int pagina, int tamanhoPagina, CancellationToken cancellationToken);
    void Atualizar(DocumentoFiscal documentoFiscal);
}

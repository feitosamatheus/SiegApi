using Microsoft.EntityFrameworkCore;
using Sieg.Domain.Entities;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.ValueObjects;
using Sieg.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sieg.Infrastructure.Repositories;

public sealed class DocumentoFiscalRepository : IDocumentoFiscalRepository
{
    private readonly DatabaseContext _context;

    public DocumentoFiscalRepository(DatabaseContext context) => _context = context;

    public async Task AdicionarAsync(DocumentoFiscal documentoFiscal, CancellationToken cancellationToken)
        => await _context.DocumentosFiscais.AddAsync(documentoFiscal, cancellationToken);

    public async Task<DocumentoFiscal?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.DocumentosFiscais.FirstOrDefaultAsync(df => df.Id == id, cancellationToken);

    public async Task<DocumentoFiscal?> ObterPorDocumentoIdAsync(Guid documentoId, CancellationToken cancellationToken)
        => await _context.DocumentosFiscais.FirstOrDefaultAsync(df => df.DocumentoId == documentoId, cancellationToken);

    public async Task<(IEnumerable<DocumentoFiscal> Itens, int TotalPaginas, int TotalRegistros, int PaginaAtual, int TamanhoPagina)> ObterTodosAsync(DocumentoFiscalFiltro filtro, int pagina, int tamanhoPagina, CancellationToken cancellationToken)
    {
        var query = _context.DocumentosFiscais.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.CnpjEmitente))
        {
            var cnpj = new Cnpj(filtro.CnpjEmitente);
            query = query.Where(d => d.CnpjEmitente == cnpj);
        }

        if (!string.IsNullOrWhiteSpace(filtro.UfEmitente))
        {
            var Uf = new Uf(filtro.UfEmitente);
            query = query.Where(d => d.UfEmitente == Uf);
        }

        if (filtro.DataInicio.HasValue)
        {
            var dataInicioInclusiva = filtro.DataInicio.Value.Date;
            query = query.Where(d => d.DataEmissao >= dataInicioInclusiva);
        }

        if (filtro.DataFim.HasValue)
        {
            var dataFimInclusiva = filtro.DataFim.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(d => d.DataEmissao <= dataFimInclusiva);
        }

        var totalRegistros = await query.CountAsync(cancellationToken);
        int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanhoPagina);

        var itens = await query
            .OrderByDescending(d => d.DataEmissao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return (Itens: itens, TotalPaginas: totalPaginas, TotalRegistros: totalRegistros, PaginaAtual: pagina, TamanhoPagina: tamanhoPagina);
    }

    public void Atualizar(DocumentoFiscal documentoFiscal)
        => _context.DocumentosFiscais.Update(documentoFiscal);
}

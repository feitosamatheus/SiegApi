using Microsoft.EntityFrameworkCore;
using Sieg.Domain.Entities;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Infrastructure.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Infrastructure.Repositories;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly DatabaseContext _context;

    public DocumentoRepository(DatabaseContext context) => _context = context;

    public async Task AdicionarAsync(Documento documento, CancellationToken cancellationToken)
        => await _context.Documentos.AddAsync(documento, cancellationToken);

    public void Atualizar(Documento documento)
        => _context.Documentos.Update(documento);

    public async Task<Documento?> ObterPorIdAsync(Guid documentoId, CancellationToken cancellationToken)
        => await _context.Documentos.FirstOrDefaultAsync(df => df.Id == documentoId, cancellationToken);

    public async Task<(IEnumerable<Documento> Itens, int TotalPaginas, int TotalRegistros, int PaginaAtual, int TamanhoPagina)> ObterTodosAsync(int pagina, int tamanho, string nome, DateTime? dataCriacao, CancellationToken cancellationToken)
    {
        var query = _context.Documentos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(d => d.NomeOriginalArquivo.Contains(nome));

        if (dataCriacao.HasValue)
            query = query.Where(d => d.DataCriacao.Date == dataCriacao.Value.Date);

        var totalRegistros = await query.CountAsync(cancellationToken);
        int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanho);

        var itens = await query
            .OrderByDescending(d => d.DataCriacao)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return (Itens: itens, TotalPaginas: totalPaginas, TotalRegistros: totalRegistros, PaginaAtual: pagina, TamanhoPagina: tamanho);
    }
}

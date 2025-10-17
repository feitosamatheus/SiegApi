using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record DocumentoPagedDTO(IEnumerable<DocumentoDTO> Itens, int TotalPaginas, int TotalRegistros, int PaginaAtual, int TamanhoPagina);

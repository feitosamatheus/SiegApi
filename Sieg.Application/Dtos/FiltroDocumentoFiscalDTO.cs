using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record FiltroDocumentoFiscalDTO(
    int PaginaAtual = 1,
    int TamanhoPagina = 10,
    string? CnpjEmitente = null,
    string? UfEmitente = null,
    DateTime? DataInicio = null,
    DateTime? DataFim = null);

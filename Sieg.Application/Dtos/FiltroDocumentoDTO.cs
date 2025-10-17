using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record FiltroDocumentoDTO(
    int Pagina = 1,
    int Tamanho = 10,
    string? Nome = null,
    DateTime? DataCriacao = null);



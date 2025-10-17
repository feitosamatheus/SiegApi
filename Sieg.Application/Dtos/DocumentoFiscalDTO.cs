using Sieg.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record DocumentoFiscalDTO(
    Guid DocumentoFiscalId,
    Guid DocumentoId,
    ETipoDocumentoFiscal TipoDocumento,
    string CnpjEmitente,
    DateTimeOffset DataEmissao,
    string UfEmitente,
    decimal ValorTotal);

using Sieg.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Api.Application.Dtos;

public sealed record DocumentoFiscalAtualizarDTO(
    Guid DocumentoFiscalId,
    ETipoDocumentoFiscal TipoDocumento,
    string CnpjEmitente,
    DateTimeOffset DataEmissao,
    string UfEmitente,
    decimal ValorTotal);
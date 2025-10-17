using Sieg.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;

public sealed record AtualizarDocumentoFiscalResult(bool Sucesso, string Mensagem, DocumentoFiscalDTO? DocumentoFiscalDTO = null);

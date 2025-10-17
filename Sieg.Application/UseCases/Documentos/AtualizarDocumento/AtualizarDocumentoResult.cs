using Sieg.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.Documentos.AtualizarDocumento;

public sealed record AtualizarDocumentoResult(bool Sucesso, string Mensagem, DocumentoDTO? DocumentoDTO = null);

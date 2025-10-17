using Sieg.Application.Dtos;
using Sieg.Domain.Entities;

namespace Sieg.Application.UseCases.Documentos.SalvarDocumento;

public sealed record SalvarDocumentoResult(bool Sucesso, string Mensagem, DocumentoDTO? Documento = null);

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.Documentos.RemoverDocumento;

public sealed record RemoverDocumentoCommand(Guid Id) : IRequest<RemoverDocumentoResult>;

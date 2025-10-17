using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.Documentos.ObterDocumentoPorId;

public sealed record ObterDocumentoPorIdCommand(Guid DocumentId) : IRequest<ObterDocumentoPorIdResult>;

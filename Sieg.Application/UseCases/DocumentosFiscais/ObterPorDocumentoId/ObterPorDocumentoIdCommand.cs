using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.DocumentosFiscais.ObterPorDocumentoId;

public sealed record ObterPorDocumentoIdCommand(Guid DocumentoId) : IRequest<ObterPorDocumentoIdResult>;

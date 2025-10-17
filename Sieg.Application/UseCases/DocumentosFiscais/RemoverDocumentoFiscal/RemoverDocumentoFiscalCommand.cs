using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.DocumentosFiscais.RemoverDocumentoFiscal;

public sealed record RemoverDocumentoFiscalCommand(Guid DocumentoFiscalId) : IRequest<RemoverDocumentoFiscalResult>;

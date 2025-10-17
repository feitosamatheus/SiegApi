using MediatR;
using Sieg.Api.Application.Dtos;
using Sieg.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.UseCases.DocumentosFiscais.AtualizarDocumentoFiscal;

public sealed record AtualizarDocumentoFiscalCommand(DocumentoFiscalAtualizarDTO Dto) : IRequest<AtualizarDocumentoFiscalResult>;

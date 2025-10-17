using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record DocumentoUpdateDTO(Guid Id, string NomeArquivo, string CaminhoXml);

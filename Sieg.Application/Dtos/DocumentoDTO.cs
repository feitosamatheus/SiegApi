using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public record DocumentoDTO(Guid Id, string NomeOriginalArquivo, string CaminhoXml, long Tamanho, DateTimeOffset DataCriacao, string xmlHash);

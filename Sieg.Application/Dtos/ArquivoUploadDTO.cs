using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Application.Dtos;

public sealed record ArquivoUploadDTO(string NomeArquivo, string TipoConteudo, Stream Conteudo, long Tamanho);



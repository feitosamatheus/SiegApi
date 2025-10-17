using Sieg.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Api.Domain.Exceptions;

public class RegistroDuplicadoException : PersonalizadaException
{
    public RegistroDuplicadoException() : base() { }
    public RegistroDuplicadoException(string mensagem) : base(mensagem) { }
    public RegistroDuplicadoException(string mensagem, Exception excecao) : base(mensagem, excecao) { }
}

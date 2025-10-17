using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sieg.Api.Domain.Interfaces.Services;

public interface IBrokerService
{
    Task PublishAsync(string json, CancellationToken cancellationToken = default);
}

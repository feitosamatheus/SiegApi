using MassTransit;
using MassTransit.Transports;
using Newtonsoft.Json.Linq;
using Sieg.Api.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sieg.Api.Infrastructure.Services;

public class BrokerService : IBrokerService
{
    private readonly IBus _bus;

    public BrokerService(IBus bus)
        => _bus = bus;
    
    public async Task PublishAsync(string json, CancellationToken cancellationToken = default)
    {
        await _bus.Publish(JObject.Parse(json), ctx =>
        {
            ctx.SetRoutingKey("documento.criado");   
            ctx.Headers.Set("event-name", "documento.criado");
        }, cancellationToken);
    }
}
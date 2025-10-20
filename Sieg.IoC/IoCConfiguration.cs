using Amazon.S3;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Sieg.Api.Domain.Interfaces.Services;
using Sieg.Api.Infrastructure.Services;
using Sieg.Application.Interfaces;
using Sieg.Application.Services;
using Sieg.Application.UseCases.Documentos.SalvarDocumento;
using Sieg.Domain.Interfaces.Repositories;
using Sieg.Domain.Interfaces.Services;
using Sieg.Domain.Interfaces.UnitOfWork;
using Sieg.Infrastructure.Contexts;
using Sieg.Infrastructure.Repositories;
using Sieg.Infrastructure.Services;
using Sieg.Infrastructure.UnitOfWork;
using System.Security.Authentication;
using Newtonsoft.Json.Linq;

namespace Sieg.IoC;

public static class IoCConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection")));


        services.AddMassTransit(x =>
        {
            var rabbitHost = configuration["RabbitMQ:Host"] ?? "b-2e244e53-f0ed-463a-a217-7bca0f99c319.mq.us-east-2.on.aws";
            var rabbitPort = int.Parse(configuration["RabbitMQ:Port"] ?? "5671");
            var rabbitUser = configuration["RabbitMQ:Username"] ?? "sieg-broker-desafio";
            var rabbitPass = configuration["RabbitMQ:Password"] ?? "SiegDesafio123!";

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var rabbitUri = $"amqps://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}/";

                cfg.Host(new Uri(rabbitUri), h =>
                {
                    h.UseSsl(s => s.Protocol = System.Security.Authentication.SslProtocols.Tls12);
                });

                cfg.UseNewtonsoftJsonSerializer();

                cfg.Message<Newtonsoft.Json.Linq.JObject>(m => m.SetEntityName("events"));
                cfg.Publish<Newtonsoft.Json.Linq.JObject>(p => p.ExchangeType = RabbitMQ.Client.ExchangeType.Topic);
            });
        });

        services.AddSingleton<IBrokerService, BrokerService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var assembly = AppDomain.CurrentDomain.Load("Sieg.Api.Application");
        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDocumentoFiscalService, DocumentoFiscalService>();
        services.AddScoped<IDocumentoService, DocumentoService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDocumentoFiscalRepository, DocumentoFiscalRepository>();
        services.AddScoped<IDocumentoRepository, DocumentoRepository>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBrokerService, BrokerService>();

        var awsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
        {
            Credentials = new Amazon.Runtime.BasicAWSCredentials(
                configuration["AWS:AccessKey"],
                configuration["AWS:SecretKey"]
            ),
            Region = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
        };

        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();

        var bucketName = configuration["AWS:BucketName"];
        services.AddSingleton<IArmazenamentoService>(sp =>
            new ArmazenamentoService(sp.GetRequiredService<IAmazonS3>(), bucketName)
        );
        
        return services;
    }
}

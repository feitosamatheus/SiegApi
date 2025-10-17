using Amazon.S3;
using Amazon.S3.Transfer;
using MassTransit.Caching.Internals;
using Microsoft.Extensions.Hosting;
using Sieg.Domain.Interfaces.Services;

namespace Sieg.Infrastructure.Services;

public sealed class ArmazenamentoService : IArmazenamentoService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public ArmazenamentoService(IAmazonS3 s3Client, string bucketName)
    {
        _s3Client = s3Client;
        _bucketName = bucketName;
    }

    public async Task<string> SalvarAsync(MemoryStream arquivo)
    {
        var nomeArquivo = $"{Guid.NewGuid()}.xml";

        arquivo.Position = 0;
        var key = $"{nomeArquivo}";

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(arquivo, _bucketName, key);

        return key; 
    }
}

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Giovanni.Task1.Models;

namespace Giovanni.Task1.Services;

public interface IPayloadBlobRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task SaveAsync(string payload, string blobName, CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task<string> GetAsync(string blobName, CancellationToken cancellationToken = default);
}

public class PayloadBlobRepository: IPayloadBlobRepository
{
    private readonly ILogger<PayloadBlobRepository> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _blobContainerName;

    public PayloadBlobRepository(
        ILogger<PayloadBlobRepository> logger,
        IOptions<FunctionSettings> options,
        BlobServiceClient blobServiceClient)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options.Value.BlobContainerName);
        ArgumentNullException.ThrowIfNull(blobServiceClient);
        
        _logger = logger;
        _blobContainerName = options.Value.BlobContainerName;
        _blobServiceClient = blobServiceClient;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(string payload, string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
        await blobClient.UploadAsync(memoryStream, overwrite: false, cancellationToken: cancellationToken);

        _logger.LogDebug("Payload saved to Blob Storage as '{blobName}'.", blobName);
    }

    /// <inheritdoc/>
    public async Task<string> GetAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);

        if (await containerClient.ExistsAsync(cancellationToken))
        {
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync(cancellationToken))
            {
                var downloadInfo = await blobClient.DownloadAsync(cancellationToken);
                using var streamReader = new StreamReader(downloadInfo.Value.Content);
                var payload = await streamReader.ReadToEndAsync();

                _logger.LogDebug("Payload retrieved from Blob Storage as '{blobName}'.", blobName);

                return payload;
            }

            _logger.LogDebug("Blob '{blobName}' not found in Blob Storage.", blobName);
        }

        _logger.LogDebug("Blob container '{blobContainerName}' does not exist.", _blobContainerName);

        return string.Empty;
    }
}
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Giovanni.Task1.Services;

public interface IPublicApiFetchService
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<(string, HttpStatusCode)> FetchAsync(string relativeUri, CancellationToken cancellationToken = default);
}

public class PublicApiFetchService: IPublicApiFetchService
{
    private readonly ILogger<PublicApiFetchService> _logger;
    private readonly HttpClient _httpClient;

    public PublicApiFetchService(
        ILogger<PublicApiFetchService> logger,
        HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(httpClient);

        _logger = logger;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<(string,HttpStatusCode)> FetchAsync(string relativeUri, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(relativeUri, cancellationToken);
        
        return (await response.Content.ReadAsStringAsync(cancellationToken), response.StatusCode);
    }
}
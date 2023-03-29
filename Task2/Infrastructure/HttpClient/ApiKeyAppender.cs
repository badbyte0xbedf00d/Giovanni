using Giovanni.Task2.Infrastructure.Models;
using Microsoft.Extensions.Options;

namespace Giovanni.Task2.Infrastructure.HttpClient;

public class ApiKeyAppender: DelegatingHandler
{
    private readonly string _apiKey;
    private readonly ILogger<ApiKeyAppender> _logger;

    public ApiKeyAppender(IOptions<WeatherApiSettings> options, ILogger<ApiKeyAppender> logger)
    {
        ArgumentNullException.ThrowIfNull(options.Value.ApiKey);
        ArgumentNullException.ThrowIfNull(logger);

        _apiKey = options.Value.ApiKey;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsApiKeyAppended(request.RequestUri!))
        {
            request.RequestUri = new Uri(request.RequestUri + $"&key={_apiKey}");
            _logger.LogDebug("Appended API key to request URI: {requestUri}", request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsApiKeyAppended(Uri requestUri) =>
        requestUri.Query.Split('?', '&').Any(x => x.StartsWith("appid="));
    
}
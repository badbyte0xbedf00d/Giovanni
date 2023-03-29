using Giovanni.Task2.Domain.Models.Weather;
using Giovanni.Task2.Domain.Models.WeatherApi;

namespace Giovanni.Task2.Services;

public interface IWeatherFetchService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cityName"></param>
    /// <param name="countryName"></param>
    /// <returns></returns>
    Task<WeatherInfo> FetchWeatherAsync(string cityName, string countryName, CancellationToken cancellationToken = default);
}

public class WeatherFetchService: IWeatherFetchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherFetchService> _logger;

    public WeatherFetchService(
        HttpClient httpClient,
        ILogger<WeatherFetchService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<WeatherInfo> FetchWeatherAsync(string cityName, string countryName, CancellationToken cancellationToken = default)
    {
        var requestUri = new Uri($"/v1/current.json?q={cityName},{countryName}", UriKind.Relative);

        return await _httpClient.GetFromJsonAsync<WeatherInfo>(requestUri, cancellationToken: cancellationToken);
    }
}
using System.Text.Json;
using Giovanni.Task2.Infrastructure.Models;
using Giovanni.Task2.Repositories;
using Microsoft.Extensions.Options;

namespace Giovanni.Task2.Services;

public interface IWeatherBackgroundService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DoWork(CancellationToken cancellationToken = default);
}

public class WeatherBackgroundService: IWeatherBackgroundService
{
    private readonly ILogger<WeatherBackgroundService> _logger;
    private readonly IWeatherFetchService _weatherFetchService;
    private readonly IWeatherRepository _weatherRepository;
    private readonly WeatherPosition[] _weatherPositions;

    public WeatherBackgroundService(
        ILogger<WeatherBackgroundService> logger,
        IOptions<WeatherSettings> weatherSettings,
        IWeatherFetchService weatherFetchService,
        IWeatherRepository weatherRepository)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(weatherSettings.Value.WeatherPositions);
        ArgumentNullException.ThrowIfNull(weatherFetchService);
        ArgumentNullException.ThrowIfNull(weatherRepository);
     
        _logger = logger;
        _weatherPositions = weatherSettings.Value.WeatherPositions.ToArray();
        _weatherFetchService = weatherFetchService;
        _weatherRepository = weatherRepository;
    }

    public async Task DoWork(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var weatherPosition in _weatherPositions)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var weatherDto = await _weatherFetchService.FetchWeatherAsync(weatherPosition.City, weatherPosition.Country, cancellationToken);

                await _weatherRepository.SaveAsync(weatherDto, cancellationToken);
                await Task.Delay(2500, cancellationToken);
            }
            
            await Task.Delay(60000, cancellationToken);
        }
    }
}
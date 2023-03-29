using Giovanni.Task2.Services;

namespace Giovanni.Task2.Infrastructure.BackgroundServices;

public class WeatherBackgroundWorker: BackgroundService
{
    private readonly ILogger<WeatherBackgroundWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WeatherBackgroundWorker(
        ILogger<WeatherBackgroundWorker> logger,
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service running.");

        await DoWork(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        return base.StopAsync(cancellationToken);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is working.");

        using var scope = _serviceProvider.CreateScope();
        var weatherBackgroundService =
            scope.ServiceProvider
                .GetRequiredService<IWeatherBackgroundService>();

        await weatherBackgroundService.DoWork(cancellationToken);
    }
}
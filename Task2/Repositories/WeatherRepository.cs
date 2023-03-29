using Giovanni.Task2.Domain.Models.Weather;
using Giovanni.Task2.Domain.Models.WeatherApi;
using Giovanni.Task2.Infrastructure;
using Giovanni.Task2.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Giovanni.Task2.Repositories;

public interface IWeatherRepository
{
    public Task SaveAsync(WeatherInfo weatherInfo, CancellationToken cancellationToken = default);
    public Task<IQueryable<Weather>> GetLastCountriesWeatherAsync(CancellationToken cancellationToken = default);
}
public class WeatherRepository: IWeatherRepository
{
    private readonly ILogger<WeatherRepository> _logger;
    private readonly IClock _clock;
    private readonly GiovanniDbContext _dbContext;

    public WeatherRepository(
        ILogger<WeatherRepository> logger,
        IClock clock,
        GiovanniDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(dbContext);

        _logger = logger;
        _clock = clock;
        _dbContext = dbContext;
    }

    public async Task SaveAsync(WeatherInfo weatherInfo, CancellationToken cancellationToken = default)
    {
        var countryName = weatherInfo.Location.Country;
        var cityName = weatherInfo.Location.Name;

        var country = await _dbContext.Countries.Include(c => c.Cities).ThenInclude(c => c.Weathers).FirstOrDefaultAsync(country =>
            country.Name == countryName, cancellationToken);

        if(country is null)
        {
            country = new Country
            {
                Name = countryName,
                Cities = new List<City>()
            };

            _dbContext.Countries.Add(country);
            
        }

        if (country.Cities.All(c => c.Name != cityName))
        {
            country.Cities.Add(new City()
            {
                Name = cityName, 
                Longitude = weatherInfo.Location.Lon,
                Latitude = weatherInfo.Location.Lat,
                Weathers = new List<Weather>()
            });
        }

        var city = country.Cities.First(c => c.Name == cityName);

        city.Weathers.Add(new Weather()
        {
            Timestamp = _clock.GetCurrentInstant().InUtc().ToDateTimeUtc(),
            Description = weatherInfo.Current.Condition.Text,
            Clouds = weatherInfo.Current.Cloud,
            Temperature = weatherInfo.Current.Temp_C,
            WindDeg = weatherInfo.Current.Wind_Degree,
            WindSpeed = weatherInfo.Current.Wind_Kph
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IQueryable<Weather>> GetLastCountriesWeatherAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_dbContext.Weathers.Include(w => w.City).ThenInclude(c => c.Country)
            .GroupBy(w => w.CityId)
            .Select(g => g.OrderByDescending(w => w.Timestamp).FirstOrDefault()).AsNoTracking());
    }
}
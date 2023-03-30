using AutoMapper;
using Giovanni.Task2.Domain.Models.OuputDto;
using Giovanni.Task2.Domain.Models.Weather;
using Giovanni.Task2.Repositories;
using Giovanni.Task2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Giovanni.Task2.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMapper _mapper;
        private readonly IWeatherRepository _weatherRepository;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IMapper mapper,
            IWeatherRepository weatherRepository)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentNullException.ThrowIfNull(weatherRepository);

            _logger = logger;
            _mapper = mapper;
            _weatherRepository = weatherRepository;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> GetWeatherForecast()
        {
            var lastWeathers = await _weatherRepository.GetLastCountriesWeatherAsync();
            var lastWeathersList = lastWeathers.ToList();
            var cities = lastWeathersList.Select(w => w.City);
            var countries = cities.Select(c => c.Country);

            return Ok(_mapper.Map<IEnumerable<CountryOutputDto>>(countries));
        }

        [HttpGet(Name = "GetWeatherByTimeRange")]
        public async Task<IActionResult> GetWeatherByTimeRange([FromQuery]string country, [FromQuery] string city, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var lastWeathers = await _weatherRepository.GetCityWeatherByDateTimeRange(country, city, from, to);

            if (!lastWeathers.Any())
                return NotFound();

            var lastWeathersList = lastWeathers.ToList();

            var chunkSize = (int)Math.Ceiling((double)lastWeathersList.Count / 10);
            var chunks = lastWeathersList.Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();

            var avg = chunks.Select(w => (AvgTemp: w.Average(weather => weather.Temperature),
                AvgWind: w.Average(weather => weather.WindSpeed), Timestamp: (w[w.Count / 2].Timestamp))).ToList();

            var result = avg.Select(a => new CountryCityWeatherDescriptionOutputDto()
            {
                AvgTemperature = a.AvgTemp, 
                AvgWind = a.AvgWind, 
                AvgUpdateDateTime = a.Timestamp,
                CityName = lastWeathersList.First().City.Name, 
                CountryName = lastWeathersList.First().City.Country.Name
            });

            return Ok(result);
        }
    }
}
using AutoMapper;
using Giovanni.Task2.Domain.Models.OuputDto;
using Giovanni.Task2.Domain.Models.Weather;
using Giovanni.Task2.Repositories;
using Giovanni.Task2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Giovanni.Task2.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> Get()
        {
            var lastWeathers = await _weatherRepository.GetLastCountriesWeatherAsync();
            var lastWeathersList = lastWeathers.ToList();
            var cities = lastWeathersList.Select(w => w.City);
            var countries = cities.Select(c => c.Country);

            return Ok(_mapper.Map<IEnumerable<CountryOutputDto>>(countries));
        }
    }
}
using System.Text.Json.Serialization;

namespace Giovanni.Task2.Domain.Models.Weather;

public class WeatherRootDto
{
    [JsonPropertyName("coord")]
    public WeatherCoordinationDto Coordination { get; set; }
    [JsonPropertyName("weather")]
    public IEnumerable<WeatherDescriptionDto> WeatherDescriptions { get; set; }
    public WeatherMainStatsDto Main { get; set; }
    public WeatherWindDto Wind { get; set; }
    public WeatherCloudsDto Clouds { get; set; }
    [JsonPropertyName("sys")]
    public WeatherSysDto System { get; set; }
    [JsonPropertyName("name")]
    public string City { get; set; }
}
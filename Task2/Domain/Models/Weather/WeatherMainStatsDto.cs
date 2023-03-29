using System.Text.Json.Serialization;

namespace Giovanni.Task2.Domain.Models.Weather;

public class WeatherMainStatsDto
{
    [JsonPropertyName("temp_min")]
    public double MinTemperature { get; set; }
    [JsonPropertyName("temp_max")]
    public double MaxTemperature { get; set; }
}
using System.Text.Json.Serialization;

namespace Giovanni.Task2.Domain.Models.Weather;

public class WeatherCoordinationDto
{
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}
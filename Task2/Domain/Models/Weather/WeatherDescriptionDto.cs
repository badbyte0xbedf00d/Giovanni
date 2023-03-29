using System.Text.Json.Serialization;

namespace Giovanni.Task2.Domain.Models.Weather;

public class WeatherDescriptionDto
{
    [JsonPropertyName("main")]
    public string Description { get; set; }
}
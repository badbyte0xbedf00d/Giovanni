namespace Giovanni.Task2.Domain.Models.OuputDto;

public record CountryOutputDto
{
    public string CountryName { get; init; }
    public IEnumerable<CityOutputDto> Cities { get; init; }

}

public record CityOutputDto
{
    public string CityName { get; init; }
    public IEnumerable<WeatherDescriptionOutputDto> WeatherDescriptions { get; init; }
}

public record WeatherDescriptionOutputDto
{
    public string Description { get; init; }
    public double Temperature { get; init; }
    public double Wind { get; init; }
    public DateTime LastUpdateDateTime { get; init; }
}

public record CountryCityWeatherDescriptionOutputDto
{
    public string CountryName { get; init; }
    public string CityName { get; init; }
    public double AvgTemperature { get; init; }
    public double AvgWind { get; init; }
    public DateTime AvgUpdateDateTime { get; init; }
}
namespace Giovanni.Task2.Infrastructure.Entities;

public class Weather
{
    public int Id { get; set; }
    public int CityId { get; set; }
    public string Description { get; set; }
    public double Temperature { get; set; }
    public double Clouds { get; set; }
    public double WindSpeed { get; set; }
    public double WindDeg { get; set; }
    public DateTime Timestamp { get; set; }
    public City City { get; set; }
}
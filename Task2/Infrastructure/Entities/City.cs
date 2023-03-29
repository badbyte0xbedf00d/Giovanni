namespace Giovanni.Task2.Infrastructure.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public List<Weather> Weathers { get; set; }
}
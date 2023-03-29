using AutoMapper;
using Giovanni.Task2.Domain.Models.OuputDto;
using Giovanni.Task2.Infrastructure.Entities;

namespace Giovanni.Task2.AutoMapper;

public class EntitiesMappingProfile: Profile
{
    public EntitiesMappingProfile()
    {
        CreateMap<Country, CountryOutputDto>()
            .ForMember(dto => dto.CountryName, expression => expression.MapFrom(country => country.Name))
            .ForMember(dto => dto.Cities,
                expression => expression.MapFrom(country => country.Cities));
        CreateMap<City, CityOutputDto>()
            .ForMember(dto => dto.CityName, expression => expression.MapFrom(city => city.Name))
            .ForMember(dto => dto.WeatherDescriptions, expression => expression.MapFrom(city => city.Weathers));
        CreateMap<Weather, WeatherDescriptionOutputDto>()
            .ForMember(dto => dto.Description, expression => expression.MapFrom(weather => weather.Description))
            .ForMember(dto => dto.Wind, expression => expression.MapFrom(weather => weather.WindSpeed))
            .ForMember(dto => dto.Temperature, expression => expression.MapFrom(weather => weather.Temperature))
            .ForMember(dto => dto.LastUpdateDateTime, expression => expression.MapFrom(weather => weather.Timestamp));
    }
}
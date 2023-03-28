using System;
using AutoMapper;
using Giovanni.Task1.Extensions;
using Giovanni.Task1.Models;

namespace Giovanni.Task1.AutoMapper
{
    public class EntitiesMappingProfile: Profile
    {
        public EntitiesMappingProfile()
        {
            CreateMap<LogEntity, LogOutputDto>()
                .ForMember(dto => dto.Status, expression => expression.MapFrom(entity => ((System.Net.HttpStatusCode)Convert.ToInt32(entity.PartitionKey)).IsSuccessStatusCode() ? "Success" : "Failure"))
                .ForMember(dto => dto.BlobName, expression => expression.MapFrom(entity => entity.BlobName))
                .ForMember(dto => dto.DateTimeUtc, expression => expression.MapFrom(entity => entity.LogDateTimeUtc))
                .ForMember(dto => dto.StatusCode, expression => expression.MapFrom(entity => Convert.ToInt32(entity.PartitionKey)));
        }
    }
}

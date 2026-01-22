using AutoMapper;
using Backend.Dto;
using Backend.Models;

namespace Backend.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<FileImport, ReturnFileImportDto>();
        CreateMap<ResultAggregate, ReturnResultAggregateDto>();
        CreateMap<ValueEntry, ReturnValueEntryDto>();
    }
}
using Backend.Dto;
using MediatR;

namespace Backend.MediatR.Queries;

public class GetLastTenFileValuesQuery : IRequest<List<ReturnValueEntryDto>>
{
    public string FileName { get; set; }
}
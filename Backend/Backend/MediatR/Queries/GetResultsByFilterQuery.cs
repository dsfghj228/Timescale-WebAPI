using Backend.Dto;
using MediatR;

namespace Backend.MediatR.Queries;

public class GetResultsByFilterQuery : IRequest<List<ReturnResultAggregateDto>>
{
    public ResultFilterDto Filter { get; set; }
}
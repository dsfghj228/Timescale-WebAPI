using AutoMapper;
using Backend.Dto;
using Backend.Interfaces;
using Backend.MediatR.Queries;
using MediatR;

namespace Backend.MediatR.Handlers;

public class GetResultsByFilterQueryHandler : IRequestHandler<GetResultsByFilterQuery, List<ReturnResultAggregateDto>>
{
    private readonly IFileRepository _fileRepository;
    private readonly IMapper _mapper;

    public GetResultsByFilterQueryHandler(IFileRepository fileRepository, IMapper mapper)
    {
        _fileRepository = fileRepository;
        _mapper = mapper;
    }
    
    public async Task<List<ReturnResultAggregateDto>> Handle(GetResultsByFilterQuery request, CancellationToken cancellationToken)
    {
        var results = await _fileRepository.GetResultsByFilterAsync(request.Filter);
        
        return _mapper.Map<List<ReturnResultAggregateDto>>(results);
    }
}
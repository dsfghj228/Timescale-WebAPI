using AutoMapper;
using Backend.Dto;
using Backend.Interfaces;
using Backend.MediatR.Queries;
using MediatR;

namespace Backend.MediatR.Handlers;

public class GetLastTenFileValuesQueryHandler : IRequestHandler<GetLastTenFileValuesQuery, List<ReturnValueEntryDto>>
{
    private readonly IMapper _mapper;
    private readonly IFileRepository _fileRepository;
    
    public GetLastTenFileValuesQueryHandler(IMapper mapper, IFileRepository fileRepository)
    {
        _mapper = mapper;
        _fileRepository = fileRepository;
    }
    
    public async Task<List<ReturnValueEntryDto>> Handle(GetLastTenFileValuesQuery request, CancellationToken cancellationToken)
    {
        var values = await _fileRepository.GetLastTenFileValuesAsync(request.FileName);
        return _mapper.Map<List<ReturnValueEntryDto>>(values);
    }
}
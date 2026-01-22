using AutoMapper;
using Backend.Dto;
using Backend.Interfaces;
using Backend.MediatR.Commands;
using MediatR;

namespace Backend.MediatR.Handlers;

public class SaveFileCommandHandler : IRequestHandler<SaveFileCommand, ReturnFileImportDto>
{
    private readonly IFileRepository _fileRepository;
    private readonly IMapper _mapper;
    
    public SaveFileCommandHandler(IFileRepository fileRepository, IMapper mapper)
    {
        _fileRepository = fileRepository;
        _mapper = mapper;
    }
    
    public async Task<ReturnFileImportDto> Handle(SaveFileCommand request, CancellationToken cancellationToken)
    {
        var file = await _fileRepository.SaveFileAsync(request.FileStream, request.FileName);
        
        return _mapper.Map<ReturnFileImportDto>(file);
    }
}
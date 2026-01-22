using Backend.Dto;
using Backend.Models;
using MediatR;

namespace Backend.MediatR.Commands;

public class SaveFileCommand : IRequest<ReturnFileImportDto>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = null!;
}
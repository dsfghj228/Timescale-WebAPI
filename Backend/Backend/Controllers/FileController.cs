using Backend.Dto;
using Backend.MediatR.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public FileController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
    {
        var command = new SaveFileCommand
        {
            FileStream = request.File.OpenReadStream(),
            FileName = request.File.FileName
        };
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
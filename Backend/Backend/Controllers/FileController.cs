using Backend.Dto;
using Backend.MediatR.Commands;
using Backend.MediatR.Queries;
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

    [HttpGet("{fileName}/values/latest")]
    public async Task<IActionResult> GetLatest([FromRoute] string fileName)
    {
        var query = new GetLastTenFileValuesQuery
        {
            FileName = fileName
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("results")]
    public async Task<IActionResult> GetResultsByFilter([FromQuery] ResultFilterDto filter)
    {
        var query = new GetResultsByFilterQuery
        {
            Filter = filter
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
using Backend.Dto;
using Backend.MediatR.Commands;
using Backend.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Контроллер для управления файлами и их данными.
/// </summary>
[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public FileController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Загрузка нового файла
    /// </summary>
    /// <param name="request">Файл</param>
    /// <response code="200">Файл успешно загружен</response>
    /// <response code="400">
    /// Возможные ошибки:
    /// - Invalid File Format
    /// - Invalid Date
    /// - Invalid ExecutionTime
    /// - Invalid Value
    /// - Invalid Row Count
    /// </response>
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
    
    /// <summary>
    /// Получение последних 10 значений файла
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <response code="200">Успешное получение значений файла</response>
    /// <response code="404">Файл не найден</response>
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
    
    /// <summary>
    /// Получение результатов по фильтру
    /// </summary>
    /// <param name="filter">Фильтры поиска</param>
    /// <response code="200">Успешное получение результатов</response>
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
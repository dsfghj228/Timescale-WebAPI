using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Backend.Data;
using Backend.Dto;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.IntegrationTests;

public class FileControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Guid _fileId = Guid.NewGuid();
    private const string FileName = "testfile.csv";
    
    public FileControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    public async Task InitializeAsync()
    {
        await SeedTestData();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        _context.Files.RemoveRange(_context.Files);
        await _context.SaveChangesAsync();
    }
    
    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var file = new FileImport
        {
            Id = _fileId,
            FileName = FileName,
            UploadedAt = DateTime.UtcNow
        };
        
        var values = new List<ValueEntry>
        {
            new ValueEntry
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                ExecutionTime = 1.5,
                Value = 10
            },
            new ValueEntry
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-5),
                ExecutionTime = 2.0,
                Value = 20
            },
            new ValueEntry
            {
                Timestamp = DateTime.UtcNow,
                ExecutionTime = 1.0,
                Value = 15
            }
        };

        foreach (var value in values)
        {
            value.FileImportId = _fileId;
        }

        _context.Files.Add(file);
        _context.Values.AddRange(values);

        var dates = values.Select(v => v.Timestamp).ToList();
        var result = new ResultAggregate
        {
            FileImportId = _fileId,
            FirstOperationDate = dates.Min(),
            TimeDeltaSeconds = (dates.Max() - dates.Min()).TotalSeconds,
            AverageExecutionTime = values.Average(v => v.ExecutionTime),
            AverageValue = values.Average(v => v.Value),
            MedianValue = new FileService().GetMedian(values.Select(v => v.Value).ToList()),
            MaxValue = values.Max(v => v.Value),
            MinValue = values.Min(v => v.Value)
        };

        _context.Results.Add(result);

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task UploadFile_ShouldReturnOk()
    {
        var fileContent = "Date;ExecutionTime;Value\n2024-01-01T10:00:00Z;1.5;10";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        var multipartContent = new MultipartFormDataContent();

        var fileContentPart = new StreamContent(fileStream);
        fileContentPart.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        multipartContent.Add(
            fileContentPart,
            "File",
            "uploadfile.csv"
        );

        var response = await _client.PostAsync("api/files/upload", multipartContent);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ReturnFileImportDto>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("uploadfile.csv", result.FileName);
        Assert.Equal(2, await context.Files.CountAsync());
    }
    
    [Fact]
    public async Task GetLatestFileValues_ShouldReturnOk()
    {
        var response = await _client.GetAsync($"api/files/{FileName}/values/latest");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ReturnValueEntryDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(15, result[0].Value);
        Assert.Equal(2.0, result[1].ExecutionTime);
    }
    
    [Fact]
    public async Task GetLatestFileValues_WrongFile_ShouldReturnNotFound()
    {
        var wrongFileName = "nonexistent.csv";
        var response = await _client.GetAsync($"api/files/{wrongFileName}/values/latest");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task GetResultsByFilters_ShouldReturnOk()
    {
        var response = await _client.GetAsync($"api/files/results?fileName={FileName}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ReturnResultAggregateDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(15, result[0].MedianValue);
    }
    
    [Fact]
    public async Task GetResultsByFilters_WrongFile_ShouldReturnEmptyArray()
    {
        var fileName = "nonexistent.csv";
        
        var response = await _client.GetAsync($"api/files/results?fileName={fileName}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<ReturnResultAggregateDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal([], result);
    }
}
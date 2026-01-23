using Back_Quiz.Exceptions;
using Backend.Data;
using Backend.Dto;
using Backend.Interfaces;
using Backend.Models;
using Backend.Repositories;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Backend.UnitTests;

public class FileRepositoryUnitTests
{
    private readonly IFileRepository _fileRepository;
    private readonly ApplicationDbContext _context;
    private readonly Guid _fileId = Guid.NewGuid();
    private const string FileName = "testfile.csv";
    
    public FileRepositoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new ApplicationDbContext(options);

        _fileRepository = new FileRepository(_context, new FileService());

        SeedTestData();
    }

    private async Task SeedTestData()
    {

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
    public async Task SaveFileAsync_ExistingFile_ReturnsFile()
    {
        var fileName = "test.csv";
        var fileContent = new List<string>
        {
            "Date;ExecutionTime;Value",
            "2024-01-01T10:00:00Z;1.5;10"
        };
        var fileText = string.Join("\n", fileContent);
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileText));
        
        var result = await _fileRepository.SaveFileAsync(stream, fileName);
        
        Assert.NotNull(result);
        Assert.Equal(fileName, result.FileName);
    }
    
    [Fact]
    public async Task SaveFileAsync_WrongFile_ShouldReturnException() 
    { 
        var fileName = "test.txt"; 
        var fileContent = new List<string>
        {
            "Date;ExecutionTime;Value",
            "2024-01-01T10:00:00Z;-1;10"
        };
        var fileText = string.Join("\n", fileContent);
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileText)); 
        
        var ex = await Assert.ThrowsAsync<CustomExceptions.InvalidExecutionTimeException>(
            () => _fileRepository.SaveFileAsync(stream, fileName));
        
        Assert.Contains("-1", ex.Message);
    }
    
    [Fact]
    public async Task GetLastTenFileValuesAsync_ExistingFile_ReturnsValues()
    {
        var result = await _fileRepository.GetLastTenFileValuesAsync(FileName);
        
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1.0, result[0].ExecutionTime);
        Assert.Equal(20, result[1].Value);
    }
    
    [Fact]
    public async Task GetLastTenFileValuesAsync_WrongFile_ShouldReturnException()
    {
        var wrongFileName = "nonexistent.csv";

        var ex = await Assert.ThrowsAsync<CustomExceptions.FileNotFoundException>(
            () => _fileRepository.GetLastTenFileValuesAsync(wrongFileName));
        
        Assert.Contains(wrongFileName, ex.Message);
    }
    
    [Fact]
    public async Task GetResultsByFilterAsync_ExistingFile_ReturnsResults()
    {
        var filter = new ResultFilterDto
        {
            FileName = FileName
        };
        
        var result = await _fileRepository.GetResultsByFilterAsync(filter);
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(FileName, result[0].FileImport.FileName);
    }
}
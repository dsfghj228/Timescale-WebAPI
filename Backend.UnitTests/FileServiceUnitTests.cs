using System.Globalization;
using Back_Quiz.Exceptions;
using Backend.Interfaces;
using Backend.Services;

namespace Backend.UnitTests;

public class FileServiceUnitTests
{
    private readonly IFileService _fileService = new FileService();

    [Fact]
    public void ValidateFile_ValidCsv_ReturnsValues()
    {
        var lines = new List<string>
        {
            "Date;ExecutionTime;Value",
            "2024-01-01T10:00:00Z;1.5;10"
        };
        var fileId = Guid.NewGuid();
        var expectedDate = DateTime.Parse(
            "2024-01-01T10:00:00Z",
            null,
            DateTimeStyles.AdjustToUniversal
        );

        
        var result = _fileService.ValidateFile(lines, fileId);
        
        Assert.Single(result);
        Assert.Equal(expectedDate, result[0].Timestamp);
        Assert.Equal(1.5, result[0].ExecutionTime);
        Assert.Equal(10, result[0].Value);
    }
    
    [Fact]
    public void ValidateFile_NegativeExecutionTime_ThrowsException()
    {
        var lines = new List<string>
        {
            "Date;ExecutionTime;Value",
            "2024-01-01T10:00:00Z;-1;10"
        };

        var ex = Assert.Throws<CustomExceptions.InvalidExecutionTimeException>(() =>
            _fileService.ValidateFile(lines, Guid.NewGuid()));

        Assert.Contains("-1", ex.Message);
    }
    
    [Fact]
    public void GetMedian_OddNumberOfElements_ReturnsCorrectMedian()
    {
        var numbers = new List<double> { 3, 1, 4, 1, 5 };
        
        var median = _fileService.GetMedian(numbers);
        
        Assert.Equal(3, median);
    }
    
    [Fact]
    public void GetMedian_EvenNumberOfElements_ReturnsAverageOfTwoMiddle()
    {
        var numbers = new List<double> { 1, 2, 3, 4 };

        var median = _fileService.GetMedian(numbers);

        Assert.Equal(2.5, median);
    }
}
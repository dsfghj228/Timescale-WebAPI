using Backend.Data;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    
    public FileRepository(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }
    
    public async Task<FileImport> SaveFileAsync(Stream file, string fileName)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var lines = new List<string>();
            using (var reader = new StreamReader(file))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
            
            var fileId = Guid.NewGuid();
            
            var values = _fileService.ValidateFile(lines, fileId);
            
            var existingFile = await _context.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FileName == fileName);
            if (existingFile != null)
            {
                _context.Values.RemoveRange(
                    _context.Values.Where(v => v.FileImportId == existingFile.Id)
                );

                _context.Results.RemoveRange(
                    _context.Results.Where(r => r.FileImportId == existingFile.Id)
                );

                _context.Files.Remove(existingFile);

                await _context.SaveChangesAsync();
            }
            
            var fileImport = new FileImport
            {
                Id = fileId,
                FileName = fileName,
                UploadedAt = DateTime.UtcNow
            };
            
            await _context.Files.AddAsync(fileImport);
            await _context.SaveChangesAsync();
            
            foreach (var value in values)
            {
                value.FileImportId = fileId;
            }

            await _context.Values.AddRangeAsync(values);
            
            var dates = values.Select(v => v.Timestamp).ToList();
            var executionTimes = values.Select(v => v.ExecutionTime).ToList();
            var valueNumbers = values.Select(v => v.Value).ToList();
            
            var result = new ResultAggregate
            {
                TimeDeltaSeconds = (dates.Max() - dates.Min()).TotalSeconds,
                FirstOperationDate = dates.Min(),
                AverageExecutionTime = executionTimes.Average(),
                AverageValue = valueNumbers.Average(),
                MedianValue = _fileService.GetMedian(valueNumbers),
                MaxValue = valueNumbers.Max(),
                MinValue = valueNumbers.Min(),
                FileImportId = fileId
            };
            
            await _context.Results.AddAsync(result);
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            fileImport.Values = values;
            fileImport.Result = result;
            
            return fileImport;
            
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ValueEntry>> GetLastTenFileValuesAsync(string fileName)
    {
        var fileExists = await _context.Files
            .AsNoTracking()
            .AnyAsync(f => f.FileName == fileName);

        if (!fileExists)
            throw new Exception($"File '{fileName}' not found");
        
        var values = await _context.Values
            .Where(v => v.FileImport.FileName == fileName)
            .AsNoTracking()
            .OrderByDescending(v => v.Timestamp)
            .Take(10)
            .ToListAsync();
        
        return values;
    }
}
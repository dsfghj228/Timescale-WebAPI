using Backend.Data;
using Backend.Dto;
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

    public async Task<List<ResultAggregate>> GetResultsByFilterAsync(ResultFilterDto filter)
    {
        IQueryable<ResultAggregate> query = _context.Results
            .Include(r => r.FileImport);

        if (!string.IsNullOrEmpty(filter.FileName))
        {
            query = query.Where(q => q.FileImport.FileName == filter.FileName);
        }
        
        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(r =>
                r.FirstOperationDate >= filter.StartDateFrom.Value);
        }

        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(r =>
                r.FirstOperationDate <= filter.StartDateTo.Value);
        }

        if (filter.AvgValueFrom.HasValue)
        {
            query = query.Where(r =>
                r.AverageValue >= filter.AvgValueFrom.Value);
        }

        if (filter.AvgValueTo.HasValue)
        {
            query = query.Where(r =>
                r.AverageValue <= filter.AvgValueTo.Value);
        }

        if (filter.AvgExecutionTimeFrom.HasValue)
        {
            query = query.Where(r =>
                r.AverageExecutionTime >= filter.AvgExecutionTimeFrom.Value);
        }

        if (filter.AvgExecutionTimeTo.HasValue)
        {
            query = query.Where(r =>
                r.AverageExecutionTime <= filter.AvgExecutionTimeTo.Value);
        }

        return await query
            .OrderByDescending(r => r.FirstOperationDate)
            .ToListAsync();
    }
}
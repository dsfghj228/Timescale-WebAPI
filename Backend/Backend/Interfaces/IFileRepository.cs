using Backend.Models;

namespace Backend.Interfaces;

public interface IFileRepository
{
    Task<FileImport> SaveFileAsync(Stream file, string fileName);
    Task<List<ValueEntry>> GetLastTenFileValuesAsync(string fileName);
}
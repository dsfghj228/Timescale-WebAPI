using Backend.Models;

namespace Backend.Interfaces;

public interface IFileService
{
    List<ValueEntry> ValidateFile(List<string> lines, Guid fileId);
    double GetMedian(List<double> numbers);
}
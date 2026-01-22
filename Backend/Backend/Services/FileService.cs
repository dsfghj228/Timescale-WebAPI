using System.Globalization;
using Backend.Interfaces;
using Backend.Models;

namespace Backend.Services;

public class FileService : IFileService
{
    public List<ValueEntry> ValidateFile(List<string> lines, Guid fileId)
    {
        if(lines.Count < 2 || lines.Count > 10001)
            throw new Exception("Файл должен содержать от 1 до 10000 записей.");
        
        var dataLines = lines.Skip(1).ToList();
        var values = new List<ValueEntry>();

        foreach (var line in dataLines)
        {
            var parts = line.Split(';');
            if (parts.Length != 3)
                throw new Exception($"Неправильный формат строки: {line}");

            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var date))
                throw new Exception($"Неправильная дата: {parts[0]}");

            if (!double.TryParse(parts[1],NumberStyles.Float, CultureInfo.InvariantCulture, out var executionTime) || executionTime < 0)
                throw new Exception($"Неверное ExecutionTime: {parts[1]}");

            if (!double.TryParse(parts[2],NumberStyles.Float, CultureInfo.InvariantCulture, out var valueNumber) || valueNumber < 0)
                throw new Exception($"Неверное Value: {parts[2]}");

            if (date < new DateTime(2000, 1, 1) || date > DateTime.UtcNow)
                throw new Exception($"Дата вне допустимого диапазона: {parts[0]}");

            values.Add(new ValueEntry
            {
                Timestamp = date,
                ExecutionTime = executionTime,
                Value = valueNumber,
                FileImportId = fileId
            });
        }
        
        return values;
    }
    
    public double GetMedian(List<double> numbers)
    {
        var sorted = numbers.OrderBy(n => n).ToList();
        int count = sorted.Count;
        if (count % 2 == 0)
        {
            return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
        }
        else
        {
            return sorted[count / 2];
        }
    }
}
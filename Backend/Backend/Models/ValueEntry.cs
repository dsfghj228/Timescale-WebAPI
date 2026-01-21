namespace Backend.Models;

public class ValueEntry
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; } //<Время начала ГГГГ-ММ-ДДTчч-мм-сс.ммммZ>;
    public double ExecutionTime { get; set; } //<Время выполнения в секундах>;
    public double Value { get; set; } //<Показатель в виде числа с плавающей запятой>
    
    public Guid FileImportId { get; set; }
    public FileImport FileImport { get; set; } 
}
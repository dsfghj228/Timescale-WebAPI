namespace Backend.Models;

public class ResultAggregate
{
    public Guid Id { get; set; }
    public double TimeDeltaSeconds { get; set; } // дельта времени Date в секундах (максимальное Date – минимальное Date)
    public DateTime FirstOperationDate { get; set; } // минимальное дата и время, как момент запуска первой операции (Date)
    public double AverageExecutionTime { get; set; } // среднее время выполнения (ExecutionTime)
    public double AverageValue { get; set; } // среднее значение по показателям (Value)
    public double MedianValue { get; set; } // медина по показателям (Value)
    public double MaxValue { get; set; } // максимальное значение показателя (Value)
    public double MinValue { get; set; } // минимальное значение показателя
    
    public Guid FileImportId { get; set; }
    public FileImport FileImport { get; set; }
}
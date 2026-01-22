namespace Backend.Dto;

public class ReturnResultAggregateDto
{
    public Guid Id { get; set; }
    public double TimeDeltaSeconds { get; set; } 
    public DateTime FirstOperationDate { get; set; } 
    public double AverageExecutionTime { get; set; } 
    public double AverageValue { get; set; }
    public double MedianValue { get; set; }
    public double MaxValue { get; set; }
    public double MinValue { get; set; }
}
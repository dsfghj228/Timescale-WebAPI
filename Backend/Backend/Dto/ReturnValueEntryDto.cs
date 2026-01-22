namespace Backend.Dto;

public class ReturnValueEntryDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double ExecutionTime { get; set; }
    public double Value { get; set; }
}
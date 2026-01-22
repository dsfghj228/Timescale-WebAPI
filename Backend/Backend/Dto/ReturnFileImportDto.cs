namespace Backend.Dto;

public class ReturnFileImportDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public ICollection<ReturnValueEntryDto> Values { get; set; }
    public ReturnResultAggregateDto Result { get; set; } 
}
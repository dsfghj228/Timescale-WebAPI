namespace Backend.Models;

public class FileImport
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public ICollection<ValueEntry> Values { get; set; } = new List<ValueEntry>();
    public ResultAggregate Result { get; set; } = new ResultAggregate();
}
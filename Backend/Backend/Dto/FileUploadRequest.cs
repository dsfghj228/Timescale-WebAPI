namespace Backend.Dto;

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
}
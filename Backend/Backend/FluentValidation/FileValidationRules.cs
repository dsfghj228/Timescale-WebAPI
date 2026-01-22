namespace Backend.Services;

public static class FileValidationRules
{
    public static bool CanRead(Stream stream)
    {
        return stream != null && stream.CanRead;
    }

    public static bool HasCsvExtension(string fileName)
    {
        return !string.IsNullOrWhiteSpace(fileName)
               && Path.GetExtension(fileName)
                   .Equals(".csv", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsValidFileName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var invalidChars = Path.GetInvalidFileNameChars();
        return !fileName.Any(c => invalidChars.Contains(c));
    }
}
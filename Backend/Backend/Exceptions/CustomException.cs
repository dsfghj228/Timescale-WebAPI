using System.Net;

namespace Back_Quiz.Exceptions;

public abstract class CustomExceptions: Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Type { get; }
    public string Title { get; }

    protected CustomExceptions(
        HttpStatusCode statusCode,
        string type,
        string title,
        string message) : base(message)
    {
        StatusCode = statusCode;
        Type = type;
        Title = title;
    }

    public class InvalidFileFormatException(string line) : CustomExceptions(HttpStatusCode.BadRequest,
        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        "Invalid File Format",
        $"Строка имеет неправильный формат: {line}");

    public class InvalidDateException(string date) : CustomExceptions(HttpStatusCode.BadRequest,
        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        "Invalid Date",
        $"Дата вне допустимого диапазона или неверный формат: {date}");

    public class InvalidExecutionTimeException(string executionTime) : CustomExceptions(HttpStatusCode.BadRequest,
        "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        "Invalid ExecutionTime",
        $"Время выполнения не может быть отрицательным или неверного формата: {executionTime}");

    public class InvalidValueException(string value) : CustomExceptions(HttpStatusCode.BadRequest,
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Invalid Value",
            $"Значение показателя не может быть отрицательным или неверного формата: {value}");

    public class InvalidFileRowCountException(int count) : CustomExceptions(HttpStatusCode.BadRequest,
            "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            "Invalid Row Count",
            $"Файл должен содержать от 1 до 10000 записей. Текущий: {count - 1}");
    
    public class FileNotFoundException(string fileName) : CustomExceptions(HttpStatusCode.NotFound,
            "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            "File Not Found",
            $"Файл с именем '{fileName}' не найден в базе данных.");
}
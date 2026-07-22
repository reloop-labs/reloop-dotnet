namespace Reloop.Exceptions;

/** Thrown for non-2xx HTTP responses and network failures. */
public class ReloopApiException : Exception
{
    public int Status { get; }
    public string StatusText { get; }
    public ApiErrorBody Body { get; }

    public ReloopApiException(int status, string statusText, ApiErrorBody? body)
        : base(BuildMessage(status, statusText, body))
    {
        Status = status;
        StatusText = statusText;
        Body = body ?? new ApiErrorBody();
    }

    public ReloopApiException(string message, Exception innerException)
        : base(message, innerException)
    {
        Status = 0;
        StatusText = "Network Error";
        Body = new ApiErrorBody { Message = message };
    }

    private static string BuildMessage(int status, string statusText, ApiErrorBody? body)
    {
        if (body?.Message is { Length: > 0 } msg)
        {
            return msg;
        }

        if (status == 0)
        {
            return "Reloop network error: " + statusText;
        }

        return "Reloop API Error: " + status + " " + statusText;
    }
}

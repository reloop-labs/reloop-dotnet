namespace Reloop.Exceptions;

/** Thrown when client arguments are invalid (no HTTP call is made). */
public class ReloopValidationException : Exception
{
    public string? Field { get; }

    public ReloopValidationException(string message)
        : this(message, null)
    {
    }

    public ReloopValidationException(string message, string? field)
        : base(message)
    {
        Field = field;
    }
}

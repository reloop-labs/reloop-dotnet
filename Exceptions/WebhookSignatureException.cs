namespace Reloop.Exceptions;

/** Thrown when webhook signature verification fails. */
public class WebhookSignatureException : Exception
{
    public WebhookSignatureException(string message)
        : base(message)
    {
    }
}

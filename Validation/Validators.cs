using Reloop.Exceptions;

namespace Reloop.Validation;

/** Shared client-side field validators (no HTTP). */
public static class Validators
{
    public static string RequireNonEmptyString(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ReloopValidationException(
                field + " is required and must be a non-empty string.", field);
        }

        return value!.Trim();
    }

    public static string RequireMailString(string? value, string field)
    {
        return RequireNonEmptyString(value, field);
    }

    public static object RequireRecipient(object? value, string field)
    {
        if (value is string s)
        {
            return RequireMailString(s, field);
        }

        if (value is IEnumerable<string> collection)
        {
            var list = collection.ToList();
            if (list.Count == 0)
            {
                throw new ReloopValidationException(
                    field + " must contain at least one address.", field);
            }

            return list.Select(entry => RequireMailString(entry, field)).ToList();
        }

        if (value is string[] arr)
        {
            return RequireRecipient(arr.ToList(), field);
        }

        throw new ReloopValidationException(
            field + " is required and must be a string or string array.", field);
    }

    public static string RequireApiKeyName(string? name, string field)
    {
        var trimmed = RequireNonEmptyString(name, field);
        if (trimmed.Length > 255)
        {
            throw new ReloopValidationException(
                "API key " + field + " must be at most 255 characters.", field);
        }

        return trimmed;
    }

    public static string RequireApiKeyId(string? id, string field)
    {
        return RequireNonEmptyString(id, field);
    }

    public static void RequirePage(int page, string field)
    {
        if (page < 1)
        {
            throw new ReloopValidationException(
                "list " + field + " must be an integer >= 1.", field);
        }
    }

    public static void RequireLimit(int limit, int min, int max, string field)
    {
        if (limit < min || limit > max)
        {
            throw new ReloopValidationException(
                "list " + field + " must be an integer between " + min + " and " + max + ".",
                field);
        }
    }

    public static string RequireMailboxId(string? id, string field)
    {
        try
        {
            return RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Mailbox " + field + " is required and must be a non-empty string.", field);
        }
    }

    public static string RequireMessageId(string? id, string field)
    {
        try
        {
            return RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Message " + field + " is required and must be a non-empty string.", field);
        }
    }

    public static string RequireThreadId(string? id, string field)
    {
        try
        {
            return RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Thread " + field + " is required and must be a non-empty string.", field);
        }
    }

    public static string RequireInboxAttachmentId(string? id, string field)
    {
        return RequireNonEmptyString(id, field);
    }

    public static void RequireInboxLimit(int limit, string field)
    {
        RequireLimit(limit, 1, 200, field);
    }

    public static void RequireInboxOffset(int offset, string field)
    {
        if (offset < 0)
        {
            throw new ReloopValidationException(
                "list " + field + " must be an integer >= 0.", field);
        }
    }

    public static List<string> RequireInboxIdArray(IList<string>? ids, string field, int max)
    {
        if (ids == null || ids.Count == 0)
        {
            throw new ReloopValidationException(
                field + " is required and must be a non-empty array.", field);
        }

        if (ids.Count > max)
        {
            throw new ReloopValidationException(
                field + " must contain at most " + max + " items.", field);
        }

        var output = new List<string>(ids.Count);
        for (var i = 0; i < ids.Count; i++)
        {
            try
            {
                output.Add(RequireNonEmptyString(ids[i], field));
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    field + "[" + i + "] must be a non-empty string.", field);
            }
        }

        return output;
    }

    public static void RequireMailboxStatus(string? status, string field)
    {
        if (status != "active" && status != "disabled")
        {
            throw new ReloopValidationException(
                field + " must be \"active\" or \"disabled\".", field);
        }
    }

    public static void RequireThreadStatus(string? status, string field)
    {
        if (status != "active" && status != "archived" && status != "closed" && status != "trash")
        {
            throw new ReloopValidationException(
                field + " must be \"active\", \"archived\", \"closed\", or \"trash\".", field);
        }
    }

    public static void RequireThreadFilter(string? filter, string field)
    {
        if (filter != "primary" && filter != "alerts" && filter != "person" && filter != "tag")
        {
            throw new ReloopValidationException(
                field + " must be \"primary\", \"alerts\", \"person\", or \"tag\".", field);
        }
    }

    public static void RequireThreadBatchAction(string? action)
    {
        if (action == null)
        {
            throw new ReloopValidationException(
                "batch action must be a valid thread batch action.", "action");
        }

        switch (action)
        {
            case "archive":
            case "trash":
            case "restore":
            case "star":
            case "unstar":
            case "read":
            case "unread":
            case "important":
            case "unimportant":
            case "spam":
            case "unspam":
            case "pin":
            case "unpin":
                return;
            default:
                throw new ReloopValidationException(
                    "batch action must be a valid thread batch action.", "action");
        }
    }

    public static void RequireComposeBody(string? text, string? html)
    {
        if (text == null && html == null)
        {
            throw new ReloopValidationException(
                "at least one of text or html is required.", "params");
        }
    }

    public static void RequireFiniteNumber(double value, string field)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ReloopValidationException(
                field + " must be a number when provided.", field);
        }
    }
}

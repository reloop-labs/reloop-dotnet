using System.Text.Json.Serialization;

namespace Reloop.Exceptions;

/** Decoded Reloop API error payload. */
public class ApiErrorBody
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("why")]
    public string? Why { get; set; }

    private string? _fix;
    private string? _tip;

    /** How to resolve the error. The API returns this as "fix". */
    [JsonPropertyName("fix")]
    public string? Fix
    {
        get => _fix ?? _tip;
        set => _fix = value;
    }

    /** Deprecated: use <see cref="Fix"/>. Mirrors Fix; also accepts a legacy "tip" field. */
    [JsonPropertyName("tip")]
    public string? Tip
    {
        get => _fix ?? _tip;
        set => _tip = value;
    }

    [JsonPropertyName("link")]
    public string? Link { get; set; }
}

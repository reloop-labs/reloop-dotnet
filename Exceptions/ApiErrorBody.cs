using System.Text.Json.Serialization;

namespace Reloop.Exceptions;

/** Decoded Reloop API error payload. */
public class ApiErrorBody
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("why")]
    public string? Why { get; set; }

    [JsonPropertyName("tip")]
    public string? Tip { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }
}

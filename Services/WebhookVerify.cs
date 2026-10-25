using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Reloop.Exceptions;
using static Reloop.Models.WebhookModels;

namespace Reloop.Services;

/** HMAC-SHA256 webhook signature verification. */
public static class WebhookVerify
{
    /** Header Reloop signs every delivery with: t=&lt;unix&gt;,v1=&lt;hex&gt;. */
    public const string WebhookSignatureHeader = "Reloop-Signature";
    public const string WebhookTimestampHeader = "Reloop-Timestamp";
    public const string WebhookIdHeader = "Reloop-Id";

    /** Header name used before 2.0.1. Still accepted as a fallback. */
    private const string LegacyWebhookSignatureHeader = "x-webhook-signature";
    public const int DefaultToleranceSeconds = 300;

    public static WebhookEvent Verify(VerifyWebhookParams parameters)
    {
        if (parameters == null || string.IsNullOrEmpty(parameters.Secret))
        {
            throw new WebhookSignatureException("Webhook secret is required");
        }

        if (parameters.Payload == null)
        {
            throw new WebhookSignatureException("Webhook payload is required");
        }

        var tolerance = parameters.Tolerance ?? DefaultToleranceSeconds;
        if (tolerance < 0)
        {
            throw new WebhookSignatureException("Webhook signature tolerance must be non-negative");
        }

        var signatureHeader = GetHeader(parameters.Headers, WebhookSignatureHeader);
        if (string.IsNullOrEmpty(signatureHeader))
        {
            signatureHeader = GetHeader(parameters.Headers, LegacyWebhookSignatureHeader);
        }
        if (string.IsNullOrEmpty(signatureHeader))
        {
            throw new WebhookSignatureException("Missing " + WebhookSignatureHeader + " header");
        }

        var parsed = ParseSignatureHeader(signatureHeader);
        VerifyTimestamp(parsed.Timestamp, tolerance);

        var expected = ComputeExpectedSignature(parameters.Secret, parsed.Timestamp, parameters.Payload);
        var valid = parsed.Signatures.Any(sig => VerifySignature(expected, sig));
        if (!valid)
        {
            throw new WebhookSignatureException("Webhook signature verification failed");
        }

        return ParseWebhookEvent(parameters.Payload);
    }

    public static WebhookEvent ConstructEvent(byte[] payload, string? signature, string secret, int? tolerance = null)
    {
        var parameters = new VerifyWebhookParams
        {
            Payload = payload,
            Secret = secret,
            Tolerance = tolerance,
            Headers = string.IsNullOrEmpty(signature)
                ? new Dictionary<string, string>()
                : new Dictionary<string, string> { [WebhookSignatureHeader] = signature },
        };

        return Verify(parameters);
    }

    internal static string ComputeExpectedSignature(string secret, string timestamp, byte[] payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var prefix = Encoding.UTF8.GetBytes(timestamp + ".");
        var combined = new byte[prefix.Length + payload.Length];
        Buffer.BlockCopy(prefix, 0, combined, 0, prefix.Length);
        Buffer.BlockCopy(payload, 0, combined, prefix.Length, payload.Length);
        return BytesToHex(hmac.ComputeHash(combined));
    }

    private static string? GetHeader(Dictionary<string, string>? headers, string name)
    {
        if (headers == null)
        {
            return null;
        }

        foreach (var entry in headers)
        {
            if (entry.Key != null
                && entry.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value;
            }
        }

        return null;
    }

    private sealed class ParsedSignature
    {
        public ParsedSignature(string timestamp, string[] signatures)
        {
            Timestamp = timestamp;
            Signatures = signatures;
        }

        public string Timestamp { get; }
        public string[] Signatures { get; }
    }

    private static ParsedSignature ParseSignatureHeader(string header)
    {
        string? timestamp = null;
        var signatures = new List<string>();

        foreach (var element in header.Split(','))
        {
            var trimmed = element.Trim();
            var eqIndex = trimmed.IndexOf('=');
            if (eqIndex == -1)
            {
                continue;
            }

            var prefix = trimmed[..eqIndex];
            var value = trimmed[(eqIndex + 1)..];
            if (prefix == "t")
            {
                timestamp = value;
            }
            else if (prefix == "v1")
            {
                signatures.Add(value);
            }
        }

        if (string.IsNullOrEmpty(timestamp) || signatures.Count == 0)
        {
            throw new WebhookSignatureException(
                "Invalid " + WebhookSignatureHeader + " header: expected t= and v1= values");
        }

        return new ParsedSignature(timestamp, signatures.ToArray());
    }

    private static bool VerifySignature(string expected, string received)
    {
        try
        {
            var expectedBuf = HexToBytes(expected);
            var receivedBuf = HexToBytes(received);
            if (expectedBuf.Length != receivedBuf.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < expectedBuf.Length; i++)
            {
                diff |= expectedBuf[i] ^ receivedBuf[i];
            }

            return diff == 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static void VerifyTimestamp(string timestamp, int tolerance)
    {
        if (!double.TryParse(timestamp, NumberStyles.Float, CultureInfo.InvariantCulture, out var ts)
            || double.IsNaN(ts)
            || double.IsInfinity(ts))
        {
            throw new WebhookSignatureException("Invalid timestamp in " + WebhookSignatureHeader + " header");
        }

        var age = Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (long)Math.Floor(ts));
        if (age > tolerance)
        {
            throw new WebhookSignatureException(
                "Timestamp outside tolerance: allowed drift is " + tolerance + " seconds");
        }
    }

    private static WebhookEvent ParseWebhookEvent(byte[] raw)
    {
        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(raw);
        }
        catch (JsonException)
        {
            throw new WebhookSignatureException("Webhook payload is not valid JSON");
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new WebhookSignatureException("Webhook payload must be a JSON object");
            }

            if (!root.TryGetProperty("id", out var idNode)
                || idNode.ValueKind != JsonValueKind.String
                || string.IsNullOrEmpty(idNode.GetString()))
            {
                throw new WebhookSignatureException("Webhook payload missing required field: id");
            }

            // Production envelope: { id, type, created_at, data }.
            // Legacy envelope (pre-2.0.1 fixtures): { id, event, payload, timestamp }.
            var type = GetString(root, "type") ?? GetString(root, "event");
            if (string.IsNullOrEmpty(type))
            {
                throw new WebhookSignatureException("Webhook payload missing required field: type");
            }

            JsonElement dataNode;
            if (!(root.TryGetProperty("data", out dataNode) && dataNode.ValueKind == JsonValueKind.Object)
                && !(root.TryGetProperty("payload", out dataNode) && dataNode.ValueKind == JsonValueKind.Object))
            {
                throw new WebhookSignatureException("Webhook payload missing required field: data");
            }

            string createdAt;
            double unixSeconds;
            var createdAtRaw = GetString(root, "created_at");
            if (!string.IsNullOrEmpty(createdAtRaw))
            {
                if (!DateTimeOffset.TryParse(createdAtRaw, CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal, out var parsedAt))
                {
                    throw new WebhookSignatureException("Webhook payload has an invalid created_at");
                }

                createdAt = createdAtRaw;
                unixSeconds = parsedAt.ToUnixTimeMilliseconds() / 1000.0;
            }
            else if (root.TryGetProperty("timestamp", out var timestampNode)
                     && timestampNode.ValueKind == JsonValueKind.Number
                     && timestampNode.TryGetDouble(out var ts)
                     && !double.IsNaN(ts)
                     && !double.IsInfinity(ts))
            {
                unixSeconds = ts;
                createdAt = DateTimeOffset.FromUnixTimeMilliseconds((long)(ts * 1000))
                    .UtcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
            }
            else
            {
                throw new WebhookSignatureException("Webhook payload missing required field: created_at");
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(dataNode.GetRawText())
                ?? new Dictionary<string, object>();

            return new WebhookEvent
            {
                Id = idNode.GetString(),
                Type = type,
                CreatedAt = createdAt,
                Data = data,
                Event = type,
                Payload = data,
                Timestamp = unixSeconds,
            };
        }
    }

    private static string? GetString(JsonElement root, string name)
    {
        return root.TryGetProperty(name, out var node) && node.ValueKind == JsonValueKind.String
            ? node.GetString()
            : null;
    }

    private static string BytesToHex(byte[] bytes)
    {
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        }

        return builder.ToString();
    }

    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
        {
            throw new FormatException("Invalid hex string.");
        }

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return bytes;
    }
}

using System.Collections.Generic;
using System.Text.Json;

namespace Reloop;

public static class RequestParameters
{
    private static readonly Dictionary<string, string> RequestKeyMap = new()
    {
        ["first_name"] = "firstName",
        ["last_name"] = "lastName",
        ["group_ids"] = "groupIds",
        ["group_id"] = "groupId",
        ["fallback_value"] = "fallbackValue",
        ["default_subscription"] = "defaultSubscription",
        ["channel_id"] = "channelId",
        ["property_name"] = "propertyName",
        ["property_type"] = "propertyType",
        ["contact_id"] = "contactId",
        ["rate_limit_enabled"] = "rateLimitEnabled",
        ["user_id"] = "userId",
    };

    public static Dictionary<string, object?> ForRequest(Dictionary<string, object?> parameters)
    {
        var normalized = new Dictionary<string, object?>();

        foreach (var kvp in parameters)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            if (key == "unsubscribed")
            {
                if (!parameters.ContainsKey("status") && value is bool unsubscribed)
                {
                    normalized["status"] = unsubscribed ? "unsubscribed" : "subscribed";
                }
                continue;
            }

            var apiKey = RequestKeyMap.TryGetValue(key, out var mapped) ? mapped : ToCamelCase(key);
            normalized[apiKey] = NormalizeValue(value, isRequest: true);
        }

        return normalized;
    }

    public static Dictionary<string, object?> ForQuery(Dictionary<string, object?> options)
    {
        return ForRequest(options);
    }

    private static object? NormalizeValue(object? value, bool isRequest)
    {
        if (value is Dictionary<string, object?> mapValue)
        {
            return isRequest ? ForRequest(mapValue) : mapValue;
        }

        if (value is IEnumerable<Dictionary<string, object?>> listValue)
        {
            var converted = new List<object?>();
            foreach (var item in listValue)
            {
                converted.Add(NormalizeValue(item, isRequest));
            }
            return converted;
        }

        if (value is object?[] arrayValue)
        {
            var converted = new List<object?>();
            foreach (var item in arrayValue)
            {
                converted.Add(NormalizeValue(item, isRequest));
            }
            return converted.ToArray();
        }

        return value;
    }

    private static string ToCamelCase(string key)
    {
        if (RequestKeyMap.TryGetValue(key, out var mapped))
        {
            return mapped;
        }

        if (!key.Contains('_'))
        {
            return key;
        }

        var parts = key.Split('_');
        var result = parts[0];
        for (var index = 1; index < parts.Length; index++)
        {
            if (string.IsNullOrEmpty(parts[index]))
            {
                continue;
            }

            result += char.ToUpperInvariant(parts[index][0]) + parts[index][1..];
        }

        return result;
    }

    public static string BuildQuery(Dictionary<string, object?> values)
    {
        if (values.Count == 0)
        {
            return string.Empty;
        }

        var query = new List<string>();
        foreach (var kvp in values)
        {
            var key = kvp.Key;
            var value = kvp.Value;
            query.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString($"{value}")}");
        }

        return "?" + string.Join("&", query);
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reloop.Models
{
    public record User(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("image")] string? Image,
        [property: JsonPropertyName("email")] string Email
    );

    public record ApiKey(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("start")] string? Start,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("organizationId")] string OrganizationId,
        [property: JsonPropertyName("userId")] string UserId,
        [property: JsonPropertyName("refillInterval")] int? RefillInterval,
        [property: JsonPropertyName("refillAmount")] int? RefillAmount,
        [property: JsonPropertyName("lastRefillAt")] string? LastRefillAt,
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("rateLimitEnabled")] bool RateLimitEnabled,
        [property: JsonPropertyName("rateLimitTimeWindow")] int RateLimitTimeWindow,
        [property: JsonPropertyName("rateLimitMax")] int RateLimitMax,
        [property: JsonPropertyName("requestCount")] int RequestCount,
        [property: JsonPropertyName("remaining")] int? Remaining,
        [property: JsonPropertyName("lastRequest")] string? LastRequest,
        [property: JsonPropertyName("expiresAt")] string? ExpiresAt,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt,
        [property: JsonPropertyName("permissions")] string? Permissions,
        [property: JsonPropertyName("metadata")] string? Metadata,
        [property: JsonPropertyName("createdBy")] User? CreatedBy,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record ApiKeyWithKey(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt,
        [property: JsonPropertyName("permissions")] string? Permissions,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record ApiKeyListResponse(
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("apiKeys")] List<ApiKey> ApiKeys,
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("event")] string Event
    );

    public class ApiKeyListParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public bool? Enabled { get; set; }
        public string? UserId { get; set; }
        public string? Q { get; set; }
    }

    public record DeleteApiKeyResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record CreateApiKeyParams(
        [property: JsonPropertyName("name")] string Name
    );

    public record UpdateApiKeyParams(
        [property: JsonPropertyName("name")] string Name
    );
}

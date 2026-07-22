using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class ApiKeyModels
{
    public class User
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class ApiKey
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("start")]
        public string? Start { get; set; }

        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        [JsonPropertyName("refillInterval")]
        public int? RefillInterval { get; set; }

        [JsonPropertyName("refillAmount")]
        public int? RefillAmount { get; set; }

        [JsonPropertyName("lastRefillAt")]
        public string? LastRefillAt { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("rateLimitEnabled")]
        public bool RateLimitEnabled { get; set; }

        [JsonPropertyName("rateLimitTimeWindow")]
        public int RateLimitTimeWindow { get; set; }

        [JsonPropertyName("rateLimitMax")]
        public int RateLimitMax { get; set; }

        [JsonPropertyName("requestCount")]
        public int RequestCount { get; set; }

        [JsonPropertyName("remaining")]
        public int? Remaining { get; set; }

        [JsonPropertyName("lastRequest")]
        public string? LastRequest { get; set; }

        [JsonPropertyName("expiresAt")]
        public string? ExpiresAt { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("permissions")]
        public string? Permissions { get; set; }

        [JsonPropertyName("metadata")]
        public string? Metadata { get; set; }

        [JsonPropertyName("createdBy")]
        public User? CreatedBy { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ApiKeyWithKey
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("permissions")]
        public string? Permissions { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ApiKeyListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("apiKeys")]
        public List<ApiKey>? ApiKeys { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ApiKeyListParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public bool? Enabled { get; set; }
        public string? UserId { get; set; }
        public string? Q { get; set; }
    }

    public class DeleteApiKeyResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class CreateApiKeyParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public CreateApiKeyParams()
        {
        }

        public CreateApiKeyParams(string name)
        {
            Name = name;
        }
    }

    public class UpdateApiKeyParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public UpdateApiKeyParams()
        {
        }

        public UpdateApiKeyParams(string name)
        {
            Name = name;
        }
    }
}

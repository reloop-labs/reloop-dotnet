using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class WebhookModels
{
    public static class WebhookStatus
    {
        public const string Active = "active";
        public const string Paused = "paused";
        public const string Disabled = "disabled";
        public const string Failed = "failed";
    }

    public static class WebhookDeliveryStatus
    {
        public const string Pending = "pending";
        public const string Success = "success";
        public const string Failed = "failed";
        public const string Retrying = "retrying";
    }

    public class Webhook
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("secret")]
        public string? Secret { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("customHeaders")]
        public Dictionary<string, string>? CustomHeaders { get; set; }

        [JsonPropertyName("rateLimitEnabled")]
        public bool RateLimitEnabled { get; set; }

        [JsonPropertyName("maxRequestsPerMinute")]
        public int MaxRequestsPerMinute { get; set; }

        [JsonPropertyName("maxRetries")]
        public int MaxRetries { get; set; }

        [JsonPropertyName("retryBackoffMultiplier")]
        public double RetryBackoffMultiplier { get; set; }

        [JsonPropertyName("filteringOptions")]
        public Dictionary<string, object>? FilteringOptions { get; set; }

        [JsonPropertyName("lastTriggeredAt")]
        public string? LastTriggeredAt { get; set; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }

        [JsonPropertyName("failureCount")]
        public int FailureCount { get; set; }

        [JsonPropertyName("consecutiveFailures")]
        public int ConsecutiveFailures { get; set; }

        [JsonPropertyName("events")]
        public List<string>? Events { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class CreateWebhookParams
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("events")]
        public List<string>? Events { get; set; }

        public CreateWebhookParams()
        {
        }

        public CreateWebhookParams(string description, string url, List<string> events)
        {
            Description = description;
            Url = url;
            Events = events;
        }
    }

    public class UpdateWebhookParams
    {
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("secret")]
        public string? Secret { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("customHeaders")]
        public Dictionary<string, string>? CustomHeaders { get; set; }

        [JsonPropertyName("rateLimitEnabled")]
        public bool? RateLimitEnabled { get; set; }

        [JsonPropertyName("maxRequestsPerMinute")]
        public double? MaxRequestsPerMinute { get; set; }

        [JsonPropertyName("maxRetries")]
        public double? MaxRetries { get; set; }

        [JsonPropertyName("retryBackoffMultiplier")]
        public double? RetryBackoffMultiplier { get; set; }

        [JsonPropertyName("filteringOptions")]
        public Dictionary<string, object>? FilteringOptions { get; set; }
    }

    public class ListWebhooksParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Status { get; set; }
        public string? OrganizationId { get; set; }
        public string? UserId { get; set; }
    }

    public class WebhookListResponse
    {
        [JsonPropertyName("webhooks")]
        public List<Webhook>? Webhooks { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }

    public class DeleteWebhookResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class TriggerWebhookParams
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }

        [JsonPropertyName("payload")]
        public Dictionary<string, object>? Payload { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }

        public TriggerWebhookParams()
        {
        }

        public TriggerWebhookParams(string @event, Dictionary<string, object> payload)
        {
            Event = @event;
            Payload = payload;
        }
    }

    public class TriggerWebhookResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("jobId")]
        public string? JobId { get; set; }
    }

    public class WebhookDelivery
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("webhookId")]
        public string? WebhookId { get; set; }

        [JsonPropertyName("webhookEventId")]
        public string? WebhookEventId { get; set; }

        [JsonPropertyName("eventType")]
        public string? EventType { get; set; }

        [JsonPropertyName("eventData")]
        public Dictionary<string, object>? EventData { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("requestUrl")]
        public string? RequestUrl { get; set; }

        [JsonPropertyName("requestHeaders")]
        public Dictionary<string, string>? RequestHeaders { get; set; }

        [JsonPropertyName("requestBody")]
        public Dictionary<string, object>? RequestBody { get; set; }

        [JsonPropertyName("responseStatus")]
        public int? ResponseStatus { get; set; }

        [JsonPropertyName("responseBody")]
        public string? ResponseBody { get; set; }

        [JsonPropertyName("responseHeaders")]
        public Dictionary<string, string>? ResponseHeaders { get; set; }

        [JsonPropertyName("attemptNumber")]
        public int AttemptNumber { get; set; }

        [JsonPropertyName("maxAttempts")]
        public int MaxAttempts { get; set; }

        [JsonPropertyName("nextRetryAt")]
        public string? NextRetryAt { get; set; }

        [JsonPropertyName("lastAttemptAt")]
        public string? LastAttemptAt { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("errorDetails")]
        public Dictionary<string, object>? ErrorDetails { get; set; }

        [JsonPropertyName("completedAt")]
        public string? CompletedAt { get; set; }

        [JsonPropertyName("durationMs")]
        public int? DurationMs { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
    }

    public class ListWebhookDeliveriesParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Status { get; set; }
    }

    public class WebhookDeliveryListResponse
    {
        [JsonPropertyName("deliveries")]
        public List<WebhookDelivery>? Deliveries { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }

    public class RetryWebhookDeliveryResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class WebhookEvent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }

        [JsonPropertyName("payload")]
        public Dictionary<string, object>? Payload { get; set; }

        [JsonPropertyName("timestamp")]
        public double Timestamp { get; set; }
    }

    public class VerifyWebhookParams
    {
        public byte[]? Payload { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
        public string? Secret { get; set; }
        public int? Tolerance { get; set; }

        public VerifyWebhookParams()
        {
        }

        public VerifyWebhookParams(byte[] payload, Dictionary<string, string> headers, string secret)
        {
            Payload = payload;
            Headers = headers;
            Secret = secret;
        }
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.WebhookModels;

namespace Reloop.Services;

/** Manages webhooks and verifies signed payloads. */
public class WebhookService
{
    private const string WebhookV1 = "/api/webhook/v1";
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal WebhookService(ReloopClient client)
    {
        _client = client;
    }

    public static WebhookEvent ConstructEvent(byte[] payload, string? signature, string secret, int? tolerance = null)
    {
        return WebhookVerify.ConstructEvent(payload, signature, secret, tolerance);
    }

    public WebhookEvent Verify(VerifyWebhookParams parameters)
    {
        return WebhookVerify.Verify(parameters);
    }

    public Task<Webhook?> CreateAsync(CreateWebhookParams parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("create params are required and must be an object.", "params");
        }

        return _client.FetchAsync<Webhook>(HttpMethod.Post, WebhookV1 + "/", parameters);
    }

    public Task<WebhookListResponse?> ListAsync(ListWebhooksParams? parameters = null)
    {
        var query = BuildListWebhooksQuery(parameters);
        return _client.FetchAsync<WebhookListResponse>(HttpMethod.Get, WebhookV1, null, query);
    }

    public Task<Webhook?> GetAsync(string webhookId)
    {
        var id = RequireWebhookId(webhookId);
        return _client.FetchAsync<Webhook>(HttpMethod.Get, WebhookV1 + "/" + id);
    }

    public Task<Webhook?> UpdateAsync(string webhookId, UpdateWebhookParams parameters)
    {
        var id = RequireWebhookId(webhookId);
        var body = ValidateUpdateParams(parameters);
        return _client.FetchAsync<Webhook>(PatchMethod, WebhookV1 + "/" + id, body);
    }

    public Task<DeleteWebhookResponse?> DeleteAsync(string webhookId)
    {
        var id = RequireWebhookId(webhookId);
        return _client.FetchAsync<DeleteWebhookResponse>(HttpMethod.Delete, WebhookV1 + "/" + id);
    }

    public Task<Webhook?> PauseAsync(string webhookId)
    {
        return UpdateAsync(webhookId, new UpdateWebhookParams { Status = WebhookStatus.Paused });
    }

    public Task<Webhook?> EnableAsync(string webhookId)
    {
        return UpdateAsync(webhookId, new UpdateWebhookParams { Status = WebhookStatus.Active });
    }

    public Task<Webhook?> DisableAsync(string webhookId)
    {
        return UpdateAsync(webhookId, new UpdateWebhookParams { Status = WebhookStatus.Disabled });
    }

    public Task<TriggerWebhookResponse?> TriggerAsync(TriggerWebhookParams parameters)
    {
        var body = ValidateTriggerParams(parameters);
        return _client.FetchAsync<TriggerWebhookResponse>(HttpMethod.Post, WebhookV1 + "/trigger", body);
    }

    public Task<WebhookDeliveryListResponse?> ListDeliveriesAsync(
        string webhookId,
        ListWebhookDeliveriesParams? parameters = null)
    {
        var id = RequireWebhookId(webhookId);
        var query = BuildListDeliveriesQuery(parameters);
        return _client.FetchAsync<WebhookDeliveryListResponse>(
            HttpMethod.Get,
            WebhookV1 + "/" + id + "/deliveries",
            null,
            query);
    }

    public Task<RetryWebhookDeliveryResponse?> RetryDeliveryAsync(string deliveryId)
    {
        var id = RequireDeliveryId(deliveryId);
        return _client.FetchAsync<RetryWebhookDeliveryResponse>(
            HttpMethod.Post,
            "/api/webhook/deliveries/" + id + "/retry");
    }

    private static string RequireWebhookId(string? id)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, "webhookId");
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Webhook webhookId is required and must be a non-empty string.", "webhookId");
        }
    }

    private static string RequireDeliveryId(string? id)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, "deliveryId");
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Webhook delivery deliveryId is required and must be a non-empty string.", "deliveryId");
        }
    }

    private static void RequireWebhookStatus(string? status, string field)
    {
        if (status == null
            || (status != WebhookStatus.Active
                && status != WebhookStatus.Paused
                && status != WebhookStatus.Disabled
                && status != WebhookStatus.Failed))
        {
            throw new ReloopValidationException("update status must be a valid webhook status.", field);
        }
    }

    private static void RequireDeliveryStatus(string? status, string field)
    {
        if (status == null
            || (status != WebhookDeliveryStatus.Pending
                && status != WebhookDeliveryStatus.Success
                && status != WebhookDeliveryStatus.Failed
                && status != WebhookDeliveryStatus.Retrying))
        {
            throw new ReloopValidationException("listDeliveries status must be a valid delivery status.", field);
        }
    }

    private static UpdateWebhookParams ValidateUpdateParams(UpdateWebhookParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("update requires at least one field to change.", "params");
        }

        if (parameters.Description == null
            && parameters.Name == null
            && parameters.Url == null
            && parameters.Secret == null
            && parameters.Status == null
            && parameters.CustomHeaders == null
            && parameters.RateLimitEnabled == null
            && parameters.MaxRequestsPerMinute == null
            && parameters.MaxRetries == null
            && parameters.RetryBackoffMultiplier == null
            && parameters.FilteringOptions == null)
        {
            throw new ReloopValidationException("update requires at least one field to change.", "params");
        }

        if (parameters.Status != null)
        {
            RequireWebhookStatus(parameters.Status, "status");
        }

        if (parameters.MaxRequestsPerMinute.HasValue)
        {
            Validators.RequireFiniteNumber(parameters.MaxRequestsPerMinute.Value, "maxRequestsPerMinute");
        }

        if (parameters.MaxRetries.HasValue)
        {
            Validators.RequireFiniteNumber(parameters.MaxRetries.Value, "maxRetries");
        }

        if (parameters.RetryBackoffMultiplier.HasValue)
        {
            Validators.RequireFiniteNumber(parameters.RetryBackoffMultiplier.Value, "retryBackoffMultiplier");
        }

        return parameters;
    }

    private static TriggerWebhookParams ValidateTriggerParams(TriggerWebhookParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("trigger params are required and must be an object.", "params");
        }

        var eventName = Validators.RequireNonEmptyString(parameters.Event, "event");
        if (parameters.Payload == null)
        {
            throw new ReloopValidationException("trigger payload is required and must be an object.", "payload");
        }

        var body = new TriggerWebhookParams(eventName, parameters.Payload);
        if (parameters.OrganizationId != null)
        {
            body.OrganizationId = Validators.RequireNonEmptyString(parameters.OrganizationId, "organizationId");
        }

        if (parameters.UserId != null)
        {
            body.UserId = Validators.RequireNonEmptyString(parameters.UserId, "userId");
        }

        return body;
    }

    private static Dictionary<string, string?> BuildListWebhooksQuery(ListWebhooksParams? parameters)
    {
        var query = new Dictionary<string, string?>();
        if (parameters == null)
        {
            return query;
        }

        if (parameters.Page.HasValue)
        {
            Validators.RequirePage(parameters.Page.Value, "page");
            query["page"] = parameters.Page.Value.ToString();
        }

        if (parameters.Limit.HasValue)
        {
            Validators.RequireLimit(parameters.Limit.Value, 1, 100, "limit");
            query["limit"] = parameters.Limit.Value.ToString();
        }

        if (parameters.Status != null)
        {
            RequireWebhookStatus(parameters.Status, "status");
            query["status"] = parameters.Status;
        }

        if (parameters.OrganizationId != null)
        {
            query["organizationId"] = Validators.RequireNonEmptyString(parameters.OrganizationId, "organizationId");
        }

        if (parameters.UserId != null)
        {
            query["userId"] = Validators.RequireNonEmptyString(parameters.UserId, "userId");
        }

        return query;
    }

    private static Dictionary<string, string?> BuildListDeliveriesQuery(ListWebhookDeliveriesParams? parameters)
    {
        var query = new Dictionary<string, string?>();
        if (parameters == null)
        {
            return query;
        }

        if (parameters.Page.HasValue)
        {
            Validators.RequirePage(parameters.Page.Value, "page");
            query["page"] = parameters.Page.Value.ToString();
        }

        if (parameters.Limit.HasValue)
        {
            Validators.RequireLimit(parameters.Limit.Value, 1, 100, "limit");
            query["limit"] = parameters.Limit.Value.ToString();
        }

        if (!string.IsNullOrEmpty(parameters.Status))
        {
            RequireDeliveryStatus(parameters.Status, "status");
            query["status"] = parameters.Status;
        }

        return query;
    }
}

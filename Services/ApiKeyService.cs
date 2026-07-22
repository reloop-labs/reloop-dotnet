using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Models;
using Reloop.Validation;
using static Reloop.Models.ApiKeyModels;

namespace Reloop.Services;

/** Manages organization API keys. */
public class ApiKeyService
{
    private const string ApiKeyV1 = "/api/api-key/v1";

    private readonly ReloopClient _client;

    internal ApiKeyService(ReloopClient client)
    {
        _client = client;
    }

    public Task<ApiKeyWithKey?> CreateAsync(CreateApiKeyParams? parameters)
    {
        var name = Validators.RequireApiKeyName(parameters?.Name, "name");
        return _client.FetchAsync<ApiKeyWithKey>(
            HttpMethod.Post,
            ApiKeyV1 + "/",
            new Dictionary<string, string> { ["name"] = name });
    }

    public Task<ApiKeyListResponse?> ListAsync(ApiKeyListParams? parameters = null)
    {
        var query = new Dictionary<string, string?>();
        if (parameters != null)
        {
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

            if (parameters.Enabled.HasValue)
            {
                query["enabled"] = parameters.Enabled.Value.ToString().ToLowerInvariant();
            }

            if (parameters.UserId != null)
            {
                query["userId"] = parameters.UserId;
            }

            if (parameters.Q != null)
            {
                query["q"] = parameters.Q;
            }
        }

        return _client.FetchAsync<ApiKeyListResponse>(HttpMethod.Get, ApiKeyV1 + "/", null, query);
    }

    public Task<ApiKey?> GetAsync(string apiKeyId)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        return _client.FetchAsync<ApiKey>(HttpMethod.Get, ApiKeyV1 + "/" + id);
    }

    public Task<ApiKey?> UpdateAsync(string apiKeyId, UpdateApiKeyParams? parameters)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        var name = Validators.RequireApiKeyName(parameters?.Name, "name");
        return _client.FetchAsync<ApiKey>(
            new HttpMethod("PATCH"),
            ApiKeyV1 + "/" + id,
            new Dictionary<string, string> { ["name"] = name });
    }

    public Task<DeleteApiKeyResponse?> DeleteAsync(string apiKeyId)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        return _client.FetchAsync<DeleteApiKeyResponse>(HttpMethod.Delete, ApiKeyV1 + "/" + id);
    }

    public Task<ApiKeyWithKey?> RotateAsync(string apiKeyId)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        return _client.FetchAsync<ApiKeyWithKey>(
            HttpMethod.Post,
            ApiKeyV1 + "/rotate/" + id,
            new Dictionary<string, object>());
    }

    public Task<ApiKey?> EnableAsync(string apiKeyId)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        return _client.FetchAsync<ApiKey>(
            HttpMethod.Post,
            ApiKeyV1 + "/enable/" + id,
            new Dictionary<string, object>());
    }

    public Task<ApiKey?> DisableAsync(string apiKeyId)
    {
        var id = Validators.RequireApiKeyId(apiKeyId, "apiKeyId");
        return _client.FetchAsync<ApiKey>(
            HttpMethod.Post,
            ApiKeyV1 + "/disable/" + id,
            new Dictionary<string, object>());
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reloop.Services;

public class ContactChannelsService
{
    private readonly ReloopClient _client;

    internal ContactChannelsService(ReloopClient client)
    {
        _client = client;
    }

    public Task<JsonElement?> CreateAsync(Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            HttpMethod.Post,
            "/api/contacts/v1/channels/create",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> ListAsync(Dictionary<string, object?> options)
    {
        var query = RequestParameters.BuildQuery(RequestParameters.ForQuery(options));
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/v1/channels/list{query}");
    }

    public Task<JsonElement?> GetAsync(string channelId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/v1/channels/{channelId}");
    }

    public Task<JsonElement?> UpdateAsync(string channelId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            new HttpMethod("PATCH"),
            $"/api/contacts/v1/channels/{channelId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> DeleteAsync(string channelId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Delete, $"/api/contacts/v1/channels/{channelId}");
    }

    public Task<JsonElement?> AddContactAsync(string channelId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            HttpMethod.Post,
            $"/api/contacts/channel/{channelId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> UpdateSubscriptionAsync(string channelId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            new HttpMethod("PATCH"),
            $"/api/contacts/channel/{channelId}",
            RequestParameters.ForRequest(parameters));
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reloop.Services;

public class ContactGroupsService
{
    private readonly ReloopClient _client;

    internal ContactGroupsService(ReloopClient client)
    {
        _client = client;
    }

    public Task<JsonElement?> AddContactAsync(string groupId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement>(
            HttpMethod.Post,
            $"/api/contacts/group/{groupId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> RemoveContactAsync(string groupId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement>(
            HttpMethod.Delete,
            $"/api/contacts/group/{groupId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> ListContactsAsync(string groupId, Dictionary<string, object?> options)
    {
        var query = RequestParameters.BuildQuery(RequestParameters.ForQuery(options));
        return _client.FetchAsync<JsonElement>(
            HttpMethod.Get,
            $"/api/contacts/v1/groups/{groupId}/contacts{query}");
    }
}

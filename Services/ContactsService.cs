using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reloop.Services;

public class ContactsService
{
    private readonly ReloopClient _client;

    public ContactGroupsService Groups { get; }
    public ContactChannelsService Channels { get; }

    internal ContactsService(ReloopClient client)
    {
        _client = client;
        Groups = new ContactGroupsService(client);
        Channels = new ContactChannelsService(client);
    }

    public Task<JsonElement?> CreateAsync(Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            HttpMethod.Post,
            "/api/contacts/create",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> GetAsync(string contactId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/retrieve/{contactId}");
    }

    public Task<JsonElement?> ListAsync(Dictionary<string, object?> options)
    {
        if (options.TryGetValue("group_id", out var groupId) || options.TryGetValue("groupId", out groupId))
        {
            var filtered = new Dictionary<string, object?>(options);
            filtered.Remove("group_id");
            filtered.Remove("groupId");
            return Groups.ListContactsAsync($"{groupId}", filtered);
        }

        var query = RequestParameters.BuildQuery(RequestParameters.ForQuery(options));
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/list{query}");
    }

    public Task<JsonElement?> UpdateAsync(string contactId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            new HttpMethod("PATCH"),
            $"/api/contacts/{contactId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> DeleteAsync(string contactId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Delete, $"/api/contacts/{contactId}");
    }

    public Task<JsonElement?> CreatePropertyAsync(Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            HttpMethod.Post,
            "/api/contacts/v1/properties/create",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> ListPropertiesAsync(Dictionary<string, object?> options)
    {
        var query = RequestParameters.BuildQuery(RequestParameters.ForQuery(options));
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/v1/properties/list{query}");
    }

    public Task<JsonElement?> UpdatePropertyAsync(string propertyId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            new HttpMethod("PATCH"),
            $"/api/contacts/v1/properties/{propertyId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> DeletePropertyAsync(string propertyId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Delete, $"/api/contacts/v1/properties/{propertyId}");
    }

    public Task<JsonElement?> CreateGroupAsync(Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            HttpMethod.Post,
            "/api/contacts/v1/groups/create",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> ListGroupsAsync(Dictionary<string, object?> options)
    {
        var query = RequestParameters.BuildQuery(RequestParameters.ForQuery(options));
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/v1/groups/list{query}");
    }

    public Task<JsonElement?> GetGroupAsync(string groupId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Get, $"/api/contacts/v1/groups/{groupId}");
    }

    public Task<JsonElement?> UpdateGroupAsync(string groupId, Dictionary<string, object?> parameters)
    {
        return _client.FetchAsync<JsonElement?>(
            new HttpMethod("PATCH"),
            $"/api/contacts/v1/groups/{groupId}",
            RequestParameters.ForRequest(parameters));
    }

    public Task<JsonElement?> DeleteGroupAsync(string groupId)
    {
        return _client.FetchAsync<JsonElement?>(HttpMethod.Delete, $"/api/contacts/v1/groups/{groupId}");
    }
}

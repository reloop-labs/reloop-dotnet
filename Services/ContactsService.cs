using System.Net.Http;
using System.Text.RegularExpressions;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.ContactModels;

namespace Reloop.Services;

/** Manages contacts and nested property/group/channel services. */
public class ContactsService
{
    private const string ContactsBase = "/api/contacts";
    private static readonly Regex EmailPattern = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
    private static readonly Regex CreatePropertyKeyPattern = new(@"^[a-z0-9_]+$", RegexOptions.Compiled);
    private static readonly Regex UpdatePropertyKeyPattern = new(@"^[a-z_]+$", RegexOptions.Compiled);

    private readonly ReloopClient _client;

    public ContactPropertiesService Properties { get; }
    public ContactGroupsService Groups { get; }
    public ContactChannelsService Channels { get; }

    internal ContactsService(ReloopClient client)
    {
        _client = client;
        Properties = new ContactPropertiesService(client);
        Groups = new ContactGroupsService(client);
        Channels = new ContactChannelsService(client);
    }

    public Task<ContactResponse?> CreateAsync(CreateContactParams parameters)
    {
        var body = ValidateCreateParams(parameters);
        return _client.FetchAsync<ContactResponse>(HttpMethod.Post, ContactsBase + "/create", body);
    }

    public Task<Contact?> GetAsync(string id)
    {
        var contactId = RequireContactId(id, "id");
        return _client.FetchAsync<Contact>(HttpMethod.Get, ContactsBase + "/retrieve/" + contactId);
    }

    public Task<ContactListResponse?> ListAsync(ListContactsParams? parameters = null)
    {
        var query = ValidateListParams(parameters);
        return _client.FetchAsync<ContactListResponse>(HttpMethod.Get, ContactsBase + "/list", null, query);
    }

    public Task<ContactResponse?> UpdateAsync(string id, UpdateContactParams parameters)
    {
        var contactId = RequireContactId(id, "id");
        var body = ValidateUpdateParams(parameters);
        return _client.FetchAsync<ContactResponse>(new HttpMethod("PATCH"), ContactsBase + "/" + contactId, body);
    }

    public Task<DeleteContactResponse?> DeleteAsync(string id)
    {
        var contactId = RequireContactId(id, "id");
        return _client.FetchAsync<DeleteContactResponse>(HttpMethod.Delete, ContactsBase + "/" + contactId);
    }

    internal static string RequireContactId(string? id, string field)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Contact " + field + " is required and must be a non-empty string.", field);
        }
    }

    internal static string RequireContactEmail(string? email, string field)
    {
        if (email == null)
        {
            throw new ReloopValidationException(
                "Contact " + field + " is required and must be a string.", field);
        }

        var trimmed = email.Trim();
        if (trimmed.Length == 0)
        {
            throw new ReloopValidationException(
                "Contact " + field + " is required and must be a string.", field);
        }

        if (!EmailPattern.IsMatch(trimmed))
        {
            throw new ReloopValidationException(
                "Contact " + field + " must be a valid email address.", field);
        }

        return trimmed;
    }

    internal static void RequireContactStatus(string? status, string field)
    {
        if (status == null
            || (status != ContactStatus.Subscribed
                && status != ContactStatus.Unsubscribed
                && status != ContactStatus.Blocked))
        {
            throw new ReloopValidationException(
                field + " must be \"subscribed\", \"unsubscribed\", or \"blocked\".", field);
        }
    }

    private static Dictionary<string, object> ValidateProperties(
        Dictionary<string, object>? properties,
        Regex keyPattern,
        string field)
    {
        if (properties == null)
        {
            return new Dictionary<string, object>();
        }

        var output = new Dictionary<string, object>();
        foreach (var entry in properties)
        {
            if (!keyPattern.IsMatch(entry.Key))
            {
                throw new ReloopValidationException(
                    field + " keys must match " + keyPattern + ".", field);
            }

            if (entry.Value is not string and not double and not float and not int and not long and not decimal)
            {
                throw new ReloopValidationException(
                    field + "." + entry.Key + " must be a string or number.", field);
            }

            output[entry.Key] = entry.Value;
        }

        return output;
    }

    private static List<string>? ValidateGroupIds(List<string>? groupIds)
    {
        if (groupIds == null)
        {
            return null;
        }

        return groupIds.Select(id =>
        {
            try
            {
                return Validators.RequireNonEmptyString(id, "groupIds");
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    "groupIds must contain non-empty strings.", "groupIds");
            }
        }).ToList();
    }

    private static List<ContactChannelInput>? ValidateChannels(List<ContactChannelInput>? channels)
    {
        if (channels == null)
        {
            return null;
        }

        for (var i = 0; i < channels.Count; i++)
        {
            var entry = channels[i];
            if (entry == null)
            {
                throw new ReloopValidationException("channels[" + i + "] must be an object.", "channels");
            }

            try
            {
                Validators.RequireNonEmptyString(entry.ChannelId, "channels");
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    "channels[" + i + "].channelId must be a non-empty string.", "channels");
            }

            if (entry.Subscription == null
                || (entry.Subscription != ChannelSubscription.OptIn
                    && entry.Subscription != ChannelSubscription.OptOut))
            {
                throw new ReloopValidationException(
                    "channels[" + i + "].subscription must be \"opt_in\" or \"opt_out\".", "channels");
            }
        }

        return channels;
    }

    private static Dictionary<string, object> ValidateCreateParams(CreateContactParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("create params are required and must be an object.", "params");
        }

        var body = new Dictionary<string, object>
        {
            ["email"] = RequireContactEmail(parameters.Email, "email"),
        };

        if (parameters.FirstName != null)
        {
            body["firstName"] = parameters.FirstName;
        }

        if (parameters.LastName != null)
        {
            body["lastName"] = parameters.LastName;
        }

        if (parameters.Status != null)
        {
            RequireContactStatus(parameters.Status, "status");
            body["status"] = parameters.Status;
        }

        if (parameters.Properties != null)
        {
            var properties = ValidateProperties(parameters.Properties, CreatePropertyKeyPattern, "properties");
            if (properties.Count > 0)
            {
                body["properties"] = properties;
            }
        }

        var groupIds = ValidateGroupIds(parameters.GroupIds);
        if (groupIds != null)
        {
            body["groupIds"] = groupIds;
        }

        var channels = ValidateChannels(parameters.Channels);
        if (channels != null)
        {
            body["channels"] = channels;
        }

        return body;
    }

    private static Dictionary<string, object> ValidateUpdateParams(UpdateContactParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException(
                "update requires at least one of email, firstName, lastName, status, or properties.",
                "params");
        }

        var body = new Dictionary<string, object>();

        if (parameters.Email != null)
        {
            body["email"] = RequireContactEmail(parameters.Email, "email");
        }

        if (parameters.FirstName != null)
        {
            body["firstName"] = parameters.FirstName;
        }

        if (parameters.LastName != null)
        {
            body["lastName"] = parameters.LastName;
        }

        if (parameters.Status != null)
        {
            RequireContactStatus(parameters.Status, "status");
            body["status"] = parameters.Status;
        }

        if (parameters.Properties != null)
        {
            var properties = ValidateProperties(parameters.Properties, UpdatePropertyKeyPattern, "properties");
            if (properties.Count > 0)
            {
                body["properties"] = properties;
            }
        }

        if (body.Count == 0)
        {
            throw new ReloopValidationException(
                "update requires at least one of email, firstName, lastName, status, or properties.",
                "params");
        }

        return body;
    }

    private static Dictionary<string, string?> ValidateListParams(ListContactsParams? parameters)
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

        if (parameters.Search != null)
        {
            query["search"] = parameters.Search;
        }

        if (parameters.Status != null)
        {
            RequireContactStatus(parameters.Status, "status");
            query["status"] = parameters.Status;
        }

        return query;
    }
}

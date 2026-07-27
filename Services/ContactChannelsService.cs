using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.ContactModels;

namespace Reloop.Services;

/** Manages contact channels and channel subscriptions. */
public class ContactChannelsService
{
    private const string ChannelsBase = "/api/contacts/v1/channels";
    private const string ChannelMembership = "/api/contacts/channel";
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal ContactChannelsService(ReloopClient client)
    {
        _client = client;
    }

    public Task<ContactChannelResponse?> CreateAsync(CreateChannelParams parameters)
    {
        var body = ValidateCreateParams(parameters);
        return _client.FetchAsync<ContactChannelResponse>(HttpMethod.Post, ChannelsBase + "/create", body);
    }

    public Task<ChannelListResponse?> ListAsync(ListChannelsParams? parameters = null)
    {
        var query = ValidateListParams(parameters);
        return _client.FetchAsync<ChannelListResponse>(HttpMethod.Get, ChannelsBase + "/list", null, query);
    }

    public Task<ContactChannel?> GetAsync(string id)
    {
        var channelId = RequireChannelId(id, "id");
        return _client.FetchAsync<ContactChannel>(HttpMethod.Get, ChannelsBase + "/" + channelId);
    }

    public Task<ContactChannelResponse?> UpdateAsync(string id, UpdateChannelParams parameters)
    {
        var channelId = RequireChannelId(id, "id");
        var body = ValidateUpdateParams(parameters);
        return _client.FetchAsync<ContactChannelResponse>(PatchMethod, ChannelsBase + "/" + channelId, body);
    }

    public Task<DeleteChannelResponse?> DeleteAsync(string id)
    {
        var channelId = RequireChannelId(id, "id");
        return _client.FetchAsync<DeleteChannelResponse>(HttpMethod.Delete, ChannelsBase + "/" + channelId);
    }

    public Task<AddContactToChannelResponse?> AddContactAsync(string id, AddContactToChannelParams? parameters)
    {
        var channelId = RequireChannelId(id, "id");
        var body = ValidateAddContactParams(parameters);
        return _client.FetchAsync<AddContactToChannelResponse>(
            HttpMethod.Post,
            ChannelMembership + "/" + channelId,
            body);
    }

    public Task<UpdateContactChannelResponse?> UpdateSubscriptionAsync(
        string id,
        UpdateContactChannelParams? parameters)
    {
        var channelId = RequireChannelId(id, "id");
        var body = ValidateUpdateSubscriptionParams(parameters);
        return _client.FetchAsync<UpdateContactChannelResponse>(
            PatchMethod,
            ChannelMembership + "/" + channelId,
            body);
    }

    private static string RequireChannelId(string? id, string field)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Channel " + field + " is required and must be a non-empty string.", field);
        }
    }

    private static string RequireChannelName(string? name, string field)
    {
        if (name == null)
        {
            throw new ReloopValidationException(
                "Channel " + field + " is required and must be a string.", field);
        }

        var trimmed = name.Trim();
        if (trimmed.Length < 1)
        {
            throw new ReloopValidationException(
                "Channel " + field + " must be at least 1 character.", field);
        }

        if (trimmed.Length > 255)
        {
            throw new ReloopValidationException(
                "Channel " + field + " must be at most 255 characters.", field);
        }

        return trimmed;
    }

    private static void RequireOptionalDescription(string? description, string field)
    {
        if (description != null && description.Length > 1000)
        {
            throw new ReloopValidationException(
                "Channel " + field + " must be at most 1000 characters.", field);
        }
    }

    private static void RequireOptionalSubscription(string? subscription, string field)
    {
        if (subscription != null
            && subscription != ChannelSubscription.OptIn
            && subscription != ChannelSubscription.OptOut)
        {
            throw new ReloopValidationException(
                "Channel " + field + " must be \"opt_in\" or \"opt_out\".", field);
        }
    }

    private static string RequireSubscription(string? subscription, string field)
    {
        RequireOptionalSubscription(subscription, field);
        if (subscription == null)
        {
            throw new ReloopValidationException(
                "Channel " + field + " is required and must be \"opt_in\" or \"opt_out\".", field);
        }

        return subscription;
    }

    private static void RequireOptionalVisibility(string? visibility, string field)
    {
        if (visibility != null
            && visibility != ChannelVisibility.Private
            && visibility != ChannelVisibility.Public)
        {
            throw new ReloopValidationException(
                "Channel " + field + " must be \"private\" or \"public\".", field);
        }
    }

    private static Dictionary<string, object?> ValidateMembershipParams(string? contactId, string? email)
    {
        var body = new Dictionary<string, object?>();
        if (contactId != null)
        {
            try
            {
                body["contact_id"] = Validators.RequireNonEmptyString(contactId, "contact_id");
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    "contact_id must be a non-empty string when provided.", "contact_id");
            }
        }

        if (email != null)
        {
            try
            {
                body["email"] = Validators.RequireNonEmptyString(email, "email");
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    "email must be a non-empty string when provided.", "email");
            }
        }

        if (body.Count == 0)
        {
            throw new ReloopValidationException("Either contact_id or email is required.", "params");
        }

        return body;
    }

    private static Dictionary<string, object?> ValidateCreateParams(CreateChannelParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("create params are required and must be an object.", "params");
        }

        var body = new Dictionary<string, object?>
        {
            ["name"] = RequireChannelName(parameters.Name, "name"),
        };

        if (parameters.Description != null)
        {
            RequireOptionalDescription(parameters.Description, "description");
            body["description"] = parameters.Description;
        }

        if (parameters.DefaultSubscription != null)
        {
            RequireOptionalSubscription(parameters.DefaultSubscription, "defaultSubscription");
            body["defaultSubscription"] = parameters.DefaultSubscription;
        }

        if (parameters.Visibility != null)
        {
            RequireOptionalVisibility(parameters.Visibility, "visibility");
            body["visibility"] = parameters.Visibility;
        }

        return body;
    }

    private static Dictionary<string, object?> ValidateUpdateParams(UpdateChannelParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException(
                "update requires at least one of name, description, or visibility.", "params");
        }

        var body = new Dictionary<string, object?>();
        if (parameters.Name != null)
        {
            body["name"] = RequireChannelName(parameters.Name, "name");
        }

        if (parameters.DescriptionPresent)
        {
            body["description"] = parameters.DescriptionClear ? null : parameters.Description;
        }

        if (parameters.Visibility != null)
        {
            RequireOptionalVisibility(parameters.Visibility, "visibility");
            body["visibility"] = parameters.Visibility;
        }

        if (body.Count == 0)
        {
            throw new ReloopValidationException(
                "update requires at least one of name, description, or visibility.", "params");
        }

        return body;
    }

    private static Dictionary<string, object?> ValidateAddContactParams(AddContactToChannelParams? parameters)
    {
        var body = ValidateMembershipParams(parameters?.ContactId, parameters?.Email);
        if (parameters?.Subscription != null)
        {
            RequireOptionalSubscription(parameters.Subscription, "subscription");
            body["subscription"] = parameters.Subscription;
        }

        return body;
    }

    private static Dictionary<string, object?> ValidateUpdateSubscriptionParams(UpdateContactChannelParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("Either contact_id or email is required.", "params");
        }

        var body = ValidateMembershipParams(parameters.ContactId, parameters.Email);
        body["subscription"] = RequireSubscription(parameters.Subscription, "subscription");
        return body;
    }

    private static Dictionary<string, string?> ValidateListParams(ListChannelsParams? parameters)
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

        return query;
    }
}

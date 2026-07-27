using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.ContactModels;

namespace Reloop.Services;

/** Manages contact groups and group membership. */
public class ContactGroupsService
{
    private const string GroupsBase = "/api/contacts/v1/groups";
    private const string GroupMembership = "/api/contacts/group";
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal ContactGroupsService(ReloopClient client)
    {
        _client = client;
    }

    public Task<ContactGroupResponse?> CreateAsync(CreateGroupParams? parameters)
    {
        var name = RequireCreateGroupName(parameters?.Name, "name");
        return _client.FetchAsync<ContactGroupResponse>(
            HttpMethod.Post,
            GroupsBase + "/create",
            new Dictionary<string, string> { ["name"] = name });
    }

    public Task<GroupListResponse?> ListAsync(ListGroupsParams? parameters = null)
    {
        var query = ValidateListParams(parameters);
        return _client.FetchAsync<GroupListResponse>(HttpMethod.Get, GroupsBase + "/list", null, query);
    }

    public Task<ContactGroup?> GetAsync(string id)
    {
        var groupId = RequireGroupId(id, "id");
        return _client.FetchAsync<ContactGroup>(HttpMethod.Get, GroupsBase + "/" + groupId);
    }

    public Task<ContactGroupResponse?> UpdateAsync(string id, UpdateGroupParams? parameters)
    {
        var groupId = RequireGroupId(id, "id");
        var name = RequireUpdateGroupName(parameters?.Name, "name");
        return _client.FetchAsync<ContactGroupResponse>(
            PatchMethod,
            GroupsBase + "/" + groupId,
            new Dictionary<string, string> { ["name"] = name });
    }

    public Task<DeleteGroupResponse?> DeleteAsync(string id)
    {
        var groupId = RequireGroupId(id, "id");
        return _client.FetchAsync<DeleteGroupResponse>(HttpMethod.Delete, GroupsBase + "/" + groupId);
    }

    public Task<GroupContactListResponse?> ListContactsAsync(string id, ListGroupContactsParams? parameters = null)
    {
        var groupId = RequireGroupId(id, "id");
        var query = ValidateListContactsParams(parameters);
        return _client.FetchAsync<GroupContactListResponse>(
            HttpMethod.Get,
            GroupsBase + "/" + groupId + "/contacts",
            null,
            query);
    }

    public Task<AddContactToGroupResponse?> AddContactAsync(string id, GroupMembershipParams? parameters)
    {
        var groupId = RequireGroupId(id, "id");
        var body = ValidateMembershipParams(parameters);
        return _client.FetchAsync<AddContactToGroupResponse>(
            HttpMethod.Post,
            GroupMembership + "/" + groupId,
            body);
    }

    public Task<RemoveContactFromGroupResponse?> RemoveContactAsync(string id, GroupMembershipParams? parameters)
    {
        var groupId = RequireGroupId(id, "id");
        var body = ValidateMembershipParams(parameters);
        return _client.FetchAsync<RemoveContactFromGroupResponse>(
            HttpMethod.Delete,
            GroupMembership + "/" + groupId,
            body);
    }

    private static string RequireGroupId(string? id, string field)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Group " + field + " is required and must be a non-empty string.", field);
        }
    }

    private static string RequireCreateGroupName(string? name, string field)
    {
        if (name == null)
        {
            throw new ReloopValidationException(
                "Group " + field + " is required and must be a string.", field);
        }

        var trimmed = name.Trim();
        if (trimmed.Length < 1)
        {
            throw new ReloopValidationException(
                "Group " + field + " must be at least 1 character.", field);
        }

        if (trimmed.Length > 50)
        {
            throw new ReloopValidationException(
                "Group " + field + " must be at most 50 characters.", field);
        }

        return trimmed;
    }

    private static string RequireUpdateGroupName(string? name, string field)
    {
        if (name == null)
        {
            throw new ReloopValidationException(
                "Group " + field + " is required and must be a string.", field);
        }

        var trimmed = name.Trim();
        if (trimmed.Length < 1)
        {
            throw new ReloopValidationException(
                "Group " + field + " must be at least 1 character.", field);
        }

        if (trimmed.Length > 255)
        {
            throw new ReloopValidationException(
                "Group " + field + " must be at most 255 characters.", field);
        }

        return trimmed;
    }

    private static void RequireListContactStatus(string? status, string field)
    {
        if (status == null
            || (status != ContactStatus.Subscribed
                && status != ContactStatus.Unsubscribed
                && status != ContactStatus.Blocked))
        {
            throw new ReloopValidationException(
                "list " + field + " must be \"subscribed\", \"unsubscribed\", or \"blocked\".", field);
        }
    }

    private static Dictionary<string, object?> ValidateMembershipParams(GroupMembershipParams? parameters)
    {
        var body = new Dictionary<string, object?>();
        if (parameters?.ContactId != null)
        {
            try
            {
                body["contact_id"] = Validators.RequireNonEmptyString(parameters.ContactId, "contact_id");
            }
            catch (ReloopValidationException)
            {
                throw new ReloopValidationException(
                    "contact_id must be a non-empty string when provided.", "contact_id");
            }
        }

        if (parameters?.Email != null)
        {
            try
            {
                body["email"] = Validators.RequireNonEmptyString(parameters.Email, "email");
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

    private static Dictionary<string, string?> ValidateListParams(ListGroupsParams? parameters)
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

        return query;
    }

    private static Dictionary<string, string?> ValidateListContactsParams(ListGroupContactsParams? parameters)
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
            RequireListContactStatus(parameters.Status, "status");
            query["status"] = parameters.Status;
        }

        return query;
    }
}

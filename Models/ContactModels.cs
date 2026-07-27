using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class ContactModels
{
    public static class ContactStatus
    {
        public const string Subscribed = "subscribed";
        public const string Unsubscribed = "unsubscribed";
        public const string Blocked = "blocked";
    }

    public static class ChannelSubscription
    {
        public const string OptIn = "opt_in";
        public const string OptOut = "opt_out";
    }

    public static class ChannelVisibility
    {
        public const string Private = "private";
        public const string Public = "public";
    }

    public static class PropertyType
    {
        public const string String = "string";
        public const string Number = "number";
    }

    public class ContactChannelInput
    {
        [JsonPropertyName("channelId")]
        public string? ChannelId { get; set; }

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }

        public ContactChannelInput()
        {
        }

        public ContactChannelInput(string channelId, string subscription)
        {
            ChannelId = channelId;
            Subscription = subscription;
        }
    }

    public class ContactGroupRef
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class ContactChannelRef
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }
    }

    public class Contact
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, object>? Properties { get; set; }

        [JsonPropertyName("groups")]
        public List<ContactGroupRef>? Groups { get; set; }

        [JsonPropertyName("channels")]
        public List<ContactChannelRef>? Channels { get; set; }

        [JsonPropertyName("suppressionReason")]
        public string? SuppressionReason { get; set; }

        [JsonPropertyName("suppressedAt")]
        public string? SuppressedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class ContactResponse : Contact
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class CreateContactParams
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, object>? Properties { get; set; }

        [JsonPropertyName("groupIds")]
        public List<string>? GroupIds { get; set; }

        [JsonPropertyName("channels")]
        public List<ContactChannelInput>? Channels { get; set; }
    }

    public class UpdateContactParams
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, object>? Properties { get; set; }
    }

    public class ListContactsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Search { get; set; }
        public string? Status { get; set; }
    }

    public class ContactListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("totalContacts")]
        public int TotalContacts { get; set; }

        [JsonPropertyName("subscribedContacts")]
        public int SubscribedContacts { get; set; }

        [JsonPropertyName("unsubscribedContacts")]
        public int UnsubscribedContacts { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class DeleteContactResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactProperty
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("propertyName")]
        public string? PropertyName { get; set; }

        [JsonPropertyName("propertyType")]
        public string? PropertyType { get; set; }

        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class ContactPropertyResponse : ContactProperty
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactPropertyListItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("propertyName")]
        public string? PropertyName { get; set; }

        [JsonPropertyName("propertyType")]
        public string? PropertyType { get; set; }

        [JsonPropertyName("defaultValue")]
        public string? DefaultValue { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class CreatePropertyParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("fallbackValue")]
        public string? FallbackValue { get; set; }
    }

    public class UpdatePropertyParams
    {
        [JsonPropertyName("fallbackValue")]
        public string? FallbackValue { get; set; }
    }

    public class ListPropertiesParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Search { get; set; }
        public string? Type { get; set; }
    }

    public class PropertyListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("properties")]
        public List<ContactPropertyListItem>? Properties { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class DeletePropertyResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactGroup
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class ContactGroupResponse : ContactGroup
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactGroupListItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class CreateGroupParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public CreateGroupParams()
        {
        }

        public CreateGroupParams(string name)
        {
            Name = name;
        }
    }

    public class UpdateGroupParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public UpdateGroupParams()
        {
        }

        public UpdateGroupParams(string name)
        {
            Name = name;
        }
    }

    public class ListGroupsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Search { get; set; }
    }

    public class GroupListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("groups")]
        public List<ContactGroupListItem>? Groups { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class DeleteGroupResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ListGroupContactsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Search { get; set; }
        public string? Status { get; set; }
    }

    public class GroupContactItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, object>? Properties { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class GroupContactListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("group")]
        public ContactGroupRef? Group { get; set; }

        [JsonPropertyName("contacts")]
        public List<GroupContactItem>? Contacts { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class GroupMembershipParams
    {
        [JsonPropertyName("contact_id")]
        public string? ContactId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }
    }

    public class AddContactToGroupResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class RemoveContactFromGroupResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactChannel
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("defaultSubscription")]
        public string? DefaultSubscription { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class ContactChannelResponse : ContactChannel
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class ContactChannelListItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("defaultSubscription")]
        public string? DefaultSubscription { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("subscriberCount")]
        public int? SubscriberCount { get; set; }
    }

    public class CreateChannelParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("defaultSubscription")]
        public string? DefaultSubscription { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }
    }

    public class UpdateChannelParams
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }

        [JsonPropertyName("descriptionPresent")]
        public bool DescriptionPresent { get; set; }

        [JsonPropertyName("descriptionClear")]
        public bool DescriptionClear { get; set; }
    }

    public class ListChannelsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
    }

    public class ChannelListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("channels")]
        public List<ContactChannelListItem>? Channels { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class DeleteChannelResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class AddContactToChannelParams
    {
        [JsonPropertyName("contact_id")]
        public string? ContactId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }
    }

    public class UpdateContactChannelParams
    {
        [JsonPropertyName("contact_id")]
        public string? ContactId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("subscription")]
        public string? Subscription { get; set; }
    }

    public class AddContactToChannelResponse
    {
        [JsonPropertyName("contact")]
        public ContactResponse? Contact { get; set; }

        [JsonPropertyName("subscriptionId")]
        public string? SubscriptionId { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class UpdateContactChannelResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reloop.Models
{
    public record User(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("image")] string? Image,
        [property: JsonPropertyName("email")] string Email
    );

    public record ApiKey(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("start")] string? Start,
        [property: JsonPropertyName("prefix")] string? Prefix,
        [property: JsonPropertyName("organizationId")] string OrganizationId,
        [property: JsonPropertyName("userId")] string UserId,
        [property: JsonPropertyName("refillInterval")] int? RefillInterval,
        [property: JsonPropertyName("refillAmount")] int? RefillAmount,
        [property: JsonPropertyName("lastRefillAt")] string? LastRefillAt,
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("rateLimitEnabled")] bool RateLimitEnabled,
        [property: JsonPropertyName("rateLimitTimeWindow")] int RateLimitTimeWindow,
        [property: JsonPropertyName("rateLimitMax")] int RateLimitMax,
        [property: JsonPropertyName("requestCount")] int RequestCount,
        [property: JsonPropertyName("remaining")] int? Remaining,
        [property: JsonPropertyName("lastRequest")] string? LastRequest,
        [property: JsonPropertyName("expiresAt")] string? ExpiresAt,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt,
        [property: JsonPropertyName("permissions")] string? Permissions,
        [property: JsonPropertyName("metadata")] string? Metadata,
        [property: JsonPropertyName("createdBy")] User? CreatedBy,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record ApiKeyWithKey(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("key")] string Key,
        [property: JsonPropertyName("enabled")] bool Enabled,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt,
        [property: JsonPropertyName("permissions")] string? Permissions,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record ApiKeyListResponse(
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("apiKeys")] List<ApiKey> ApiKeys,
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("event")] string Event
    );

    public class ApiKeyListParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public bool? Enabled { get; set; }
        public string? UserId { get; set; }
        public string? Q { get; set; }
    }

    public record DeleteApiKeyResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("message")] string Message,
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("event")] string Event
    );

    public record CreateApiKeyParams(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("enabled")] bool? Enabled = null,
        [property: JsonPropertyName("rateLimitEnabled")] bool? RateLimitEnabled = null
    );

    public record UpdateApiKeyParams(
        [property: JsonPropertyName("name")] string? Name = null,
        [property: JsonPropertyName("enabled")] bool? Enabled = null
    );

    public record DnsRecord(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("recordType")] string RecordType,
        [property: JsonPropertyName("recordTypeName")] string RecordTypeName,
        [property: JsonPropertyName("domain")] string Domain,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("value")] string Value,
        [property: JsonPropertyName("ttl")] string Ttl,
        [property: JsonPropertyName("priority")] int? Priority,
        [property: JsonPropertyName("verificationError")] string? VerificationError,
        [property: JsonPropertyName("purpose")] string? Purpose,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt
    );

    public record Domain(
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("domain")] string DomainName,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("userVerifiedDomain")] bool UserVerifiedDomain,
        [property: JsonPropertyName("systemVerified")] bool SystemVerified,
        [property: JsonPropertyName("customReturnPath")] string CustomReturnPath,
        [property: JsonPropertyName("trackingSubdomain")] string TrackingSubdomain,
        [property: JsonPropertyName("isClickTrackingEnabled")] bool IsClickTrackingEnabled,
        [property: JsonPropertyName("isOpenTrackingEnabled")] bool IsOpenTrackingEnabled,
        [property: JsonPropertyName("tls")] string Tls,
        [property: JsonPropertyName("isTrackingDomain")] bool IsTrackingDomain,
        [property: JsonPropertyName("isSendingEmailEnabled")] bool IsSendingEmailEnabled,
        [property: JsonPropertyName("isReceivingEmailEnabled")] bool IsReceivingEmailEnabled,
        [property: JsonPropertyName("verificationFailedReason")] string? VerificationFailedReason,
        [property: JsonPropertyName("dnsRecords")] List<DnsRecord> DnsRecords,
        [property: JsonPropertyName("lastVerifiedAt")] string? LastVerifiedAt,
        [property: JsonPropertyName("createdAt")] string CreatedAt,
        [property: JsonPropertyName("updatedAt")] string UpdatedAt,
        [property: JsonPropertyName("event")] string? Event
    );

    public record CreateDomainParams(
        [property: JsonPropertyName("domain")] string Domain,
        [property: JsonPropertyName("custom_return_path")] string? CustomReturnPath = null,
        [property: JsonPropertyName("tracking")] string? Tracking = null,
        [property: JsonPropertyName("click_tracking")] bool? ClickTracking = null,
        [property: JsonPropertyName("open_tracking")] bool? OpenTracking = null,
        [property: JsonPropertyName("tls")] string? Tls = null,
        [property: JsonPropertyName("sending_email")] bool? SendingEmail = null,
        [property: JsonPropertyName("receiving_email")] bool? ReceivingEmail = null
    );

    public record UpdateDomainParams(
        [property: JsonPropertyName("click_tracking")] bool? ClickTracking = null,
        [property: JsonPropertyName("open_tracking")] bool? OpenTracking = null,
        [property: JsonPropertyName("sending_email")] bool? SendingEmail = null,
        [property: JsonPropertyName("receiving_email")] bool? ReceivingEmail = null,
        [property: JsonPropertyName("tls")] string? Tls = null
    );

    public class ListDomainsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Q { get; set; }
        public string? Status { get; set; }
    }

    public record DomainListResponse(
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("domains")] List<Domain> Domains,
        [property: JsonPropertyName("total")] int Total,
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("limit")] int Limit,
        [property: JsonPropertyName("event")] string Event
    );

    public record DomainStatusResponse(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("event")] string? Event
    );

    public record ForwardDnsParams(
        [property: JsonPropertyName("email")] string Email
    );

    public record ForwardDnsResponse(
        [property: JsonPropertyName("success")] bool Success
    );

    public record DomainNameserversResponse(
        [property: JsonPropertyName("object")] string Object,
        [property: JsonPropertyName("domainId")] string DomainId,
        [property: JsonPropertyName("domain")] string Domain,
        [property: JsonPropertyName("nameservers")] List<string>? Nameservers,
        [property: JsonPropertyName("dnsProvider")] string? DnsProvider,
        [property: JsonPropertyName("event")] string Event
    );

    public record SendMailResponse(
        [property: JsonPropertyName("success")] bool Success,
        [property: JsonPropertyName("messageId")] string MessageId,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("timestamp")] string Timestamp,
        [property: JsonPropertyName("id")] string Id
    );
}

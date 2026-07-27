using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class DomainModels
{
    public static class DomainStatus
    {
        public const string Pending = "pending";
        public const string Verifying = "verifying";
        public const string Active = "active";
        public const string Suspended = "suspended";
        public const string Failed = "failed";
    }

    public static class DomainTlsMode
    {
        public const string Opportunistic = "opportunistic";
        public const string Enforced = "enforced";
    }

    public class DnsRecord
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("recordType")]
        public string? RecordType { get; set; }

        [JsonPropertyName("recordTypeName")]
        public string? RecordTypeName { get; set; }

        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("ttl")]
        public string? Ttl { get; set; }

        [JsonPropertyName("priority")]
        public int? Priority { get; set; }

        [JsonPropertyName("verificationError")]
        public string? VerificationError { get; set; }

        [JsonPropertyName("purpose")]
        public string? Purpose { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class Domain
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("domain")]
        public string? DomainName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("userVerifiedDomain")]
        public bool UserVerifiedDomain { get; set; }

        [JsonPropertyName("systemVerified")]
        public bool SystemVerified { get; set; }

        [JsonPropertyName("customReturnPath")]
        public string? CustomReturnPath { get; set; }

        [JsonPropertyName("trackingSubdomain")]
        public string? TrackingSubdomain { get; set; }

        [JsonPropertyName("isClickTrackingEnabled")]
        public bool IsClickTrackingEnabled { get; set; }

        [JsonPropertyName("isOpenTrackingEnabled")]
        public bool IsOpenTrackingEnabled { get; set; }

        [JsonPropertyName("tls")]
        public string? Tls { get; set; }

        [JsonPropertyName("isTrackingDomain")]
        public bool IsTrackingDomain { get; set; }

        [JsonPropertyName("isSendingEmailEnabled")]
        public bool IsSendingEmailEnabled { get; set; }

        [JsonPropertyName("isReceivingEmailEnabled")]
        public bool IsReceivingEmailEnabled { get; set; }

        [JsonPropertyName("verificationFailedReason")]
        public string? VerificationFailedReason { get; set; }

        [JsonPropertyName("dnsRecords")]
        public List<DnsRecord>? DnsRecords { get; set; }

        [JsonPropertyName("lastVerifiedAt")]
        public string? LastVerifiedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class CreateDomainParams
    {
        [JsonPropertyName("domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("click_tracking")]
        public bool? ClickTracking { get; set; }

        [JsonPropertyName("open_tracking")]
        public bool? OpenTracking { get; set; }

        [JsonPropertyName("tls")]
        public string? Tls { get; set; }

        [JsonPropertyName("sending_email")]
        public bool? SendingEmail { get; set; }

        [JsonPropertyName("receiving_email")]
        public bool? ReceivingEmail { get; set; }

        public CreateDomainParams()
        {
        }

        public CreateDomainParams(string domain)
        {
            Domain = domain;
        }
    }

    public class UpdateDomainParams
    {
        [JsonPropertyName("click_tracking")]
        public bool? ClickTracking { get; set; }

        [JsonPropertyName("open_tracking")]
        public bool? OpenTracking { get; set; }

        [JsonPropertyName("sending_email")]
        public bool? SendingEmail { get; set; }

        [JsonPropertyName("receiving_email")]
        public bool? ReceivingEmail { get; set; }

        [JsonPropertyName("tls")]
        public string? Tls { get; set; }
    }

    public class ListDomainsParams
    {
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? Q { get; set; }
        public string? Status { get; set; }
    }

    public class DomainListResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }

        [JsonPropertyName("domains")]
        public List<Domain>? Domains { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }

    public class DomainStatusResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("event")]
        public string? Event { get; set; }
    }
}

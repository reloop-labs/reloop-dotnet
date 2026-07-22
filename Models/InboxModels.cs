using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class InboxModels
{
    public static class MailboxStatus
    {
        public const string Active = "active";
        public const string Disabled = "disabled";
    }

    public static class ThreadStatus
    {
        public const string Active = "active";
        public const string Archived = "archived";
        public const string Closed = "closed";
        public const string Trash = "trash";
    }

    public static class ThreadFilter
    {
        public const string Primary = "primary";
        public const string Alerts = "alerts";
        public const string Person = "person";
        public const string Tag = "tag";
    }

    public static class ThreadBatchAction
    {
        public const string Archive = "archive";
        public const string Trash = "trash";
        public const string Restore = "restore";
        public const string Star = "star";
        public const string Unstar = "unstar";
        public const string Read = "read";
        public const string Unread = "unread";
        public const string Important = "important";
        public const string Unimportant = "unimportant";
        public const string Spam = "spam";
        public const string Unspam = "unspam";
        public const string Pin = "pin";
        public const string Unpin = "unpin";
    }

    public class InboxSuccessResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("isRead")]
        public bool? IsRead { get; set; }

        [JsonPropertyName("isStarred")]
        public bool? IsStarred { get; set; }

        [JsonPropertyName("isSpam")]
        public bool? IsSpam { get; set; }

        [JsonPropertyName("isImportant")]
        public bool? IsImportant { get; set; }

        [JsonPropertyName("isPinned")]
        public bool? IsPinned { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("deletedAt")]
        public string? DeletedAt { get; set; }
    }

    public class SendEmailResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    public class SendEmailOrPendingResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("pending")]
        public bool? Pending { get; set; }

        [JsonPropertyName("sendAt")]
        public string? SendAt { get; set; }
    }

    public class ThreadBatchResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("ids")]
        public List<string>? Ids { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }
    }

    public class MessageAttachment
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("storagePath")]
        public string? StoragePath { get; set; }

        [JsonPropertyName("contentDisposition")]
        public string? ContentDisposition { get; set; }

        [JsonPropertyName("contentId")]
        public string? ContentId { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
    }

    public class AttachmentInput
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("content_id")]
        public string? ContentId { get; set; }
    }

    public class Mailbox
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("quota")]
        public string? Quota { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
    }

    public class MailboxDetail : Mailbox
    {
        [JsonPropertyName("domainId")]
        public string? DomainId { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class CreateMailboxResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CreateMailboxParams
    {
        [JsonPropertyName("domainId")]
        public string? DomainId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("quota")]
        public string? Quota { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
    }

    public class UpdateMailboxParams
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("quota")]
        public string? Quota { get; set; }
    }

    public class MessageAttachmentItem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("inboundEmailId")]
        public string? InboundEmailId { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("storagePath")]
        public string? StoragePath { get; set; }

        [JsonPropertyName("contentDisposition")]
        public string? ContentDisposition { get; set; }

        [JsonPropertyName("contentId")]
        public string? ContentId { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("fromEmail")]
        public string? FromEmail { get; set; }

        [JsonPropertyName("fromName")]
        public string? FromName { get; set; }

        [JsonPropertyName("toEmails")]
        public List<string>? ToEmails { get; set; }

        [JsonPropertyName("ccEmails")]
        public List<string>? CcEmails { get; set; }

        [JsonPropertyName("bccEmails")]
        public List<string>? BccEmails { get; set; }

        [JsonPropertyName("replyTo")]
        public string? ReplyTo { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("textBody")]
        public string? TextBody { get; set; }

        [JsonPropertyName("htmlBody")]
        public string? HtmlBody { get; set; }

        [JsonPropertyName("snippet")]
        public string? Snippet { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        [JsonPropertyName("isStarred")]
        public bool IsStarred { get; set; }

        [JsonPropertyName("isSpam")]
        public bool IsSpam { get; set; }

        [JsonPropertyName("spamScore")]
        public double? SpamScore { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("threadId")]
        public string? ThreadId { get; set; }

        [JsonPropertyName("inReplyTo")]
        public string? InReplyTo { get; set; }

        [JsonPropertyName("references")]
        public List<string>? References { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("attachments")]
        public List<MessageAttachmentItem>? Attachments { get; set; }
    }

    public class MessageRaw
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("raw")]
        public string? Raw { get; set; }
    }

    public class ListMessagesParams
    {
        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("q")]
        public string? Q { get; set; }

        [JsonPropertyName("isSpam")]
        public bool? IsSpam { get; set; }
    }

    public class ListSentMessagesParams
    {
        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }
    }

    public class BatchMessagesParams
    {
        [JsonPropertyName("ids")]
        public List<string>? Ids { get; set; }
    }

    public class UpdateMessageParams
    {
        [JsonPropertyName("isRead")]
        public bool? IsRead { get; set; }

        [JsonPropertyName("isStarred")]
        public bool? IsStarred { get; set; }

        [JsonPropertyName("isSpam")]
        public bool? IsSpam { get; set; }
    }

    public class SetMessageReadParams
    {
        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        public SetMessageReadParams()
        {
            IsRead = true;
        }

        public SetMessageReadParams(bool isRead)
        {
            IsRead = isRead;
        }
    }

    public class SetMessageStarParams
    {
        [JsonPropertyName("isStarred")]
        public bool IsStarred { get; set; }

        public SetMessageStarParams()
        {
            IsStarred = true;
        }

        public SetMessageStarParams(bool isStarred)
        {
            IsStarred = isStarred;
        }
    }

    public class SendMessageParams
    {
        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }

        [JsonPropertyName("to")]
        public object? To { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("html")]
        public string? Html { get; set; }

        [JsonPropertyName("cc")]
        public object? Cc { get; set; }

        [JsonPropertyName("bcc")]
        public object? Bcc { get; set; }

        [JsonPropertyName("attachments")]
        public List<AttachmentInput>? Attachments { get; set; }

        [JsonPropertyName("scheduledAt")]
        public string? ScheduledAt { get; set; }

        [JsonPropertyName("undoWindowSeconds")]
        public double? UndoWindowSeconds { get; set; }
    }

    public class ComposeMessageParams
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("html")]
        public string? Html { get; set; }

        [JsonPropertyName("cc")]
        public object? Cc { get; set; }

        [JsonPropertyName("bcc")]
        public object? Bcc { get; set; }

        [JsonPropertyName("attachments")]
        public List<AttachmentInput>? Attachments { get; set; }
    }

    public class ForwardMessageParams : ComposeMessageParams
    {
        [JsonPropertyName("to")]
        public object? To { get; set; }
    }

    public class ThreadLabel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }
    }

    public class Thread
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }

        [JsonPropertyName("organizationId")]
        public string? OrganizationId { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("lastMessagePreview")]
        public string? LastMessagePreview { get; set; }

        [JsonPropertyName("lastMessageAt")]
        public string? LastMessageAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("messageCount")]
        public int MessageCount { get; set; }

        [JsonPropertyName("participants")]
        public List<string>? Participants { get; set; }

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        [JsonPropertyName("isStarred")]
        public bool IsStarred { get; set; }

        [JsonPropertyName("isImportant")]
        public bool? IsImportant { get; set; }

        [JsonPropertyName("isPinned")]
        public bool? IsPinned { get; set; }

        [JsonPropertyName("pinnedAt")]
        public string? PinnedAt { get; set; }

        [JsonPropertyName("labels")]
        public List<ThreadLabel>? Labels { get; set; }

        [JsonPropertyName("deletedAt")]
        public string? DeletedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string? UpdatedAt { get; set; }
    }

    public class ThreadMessage
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("threadId")]
        public string? ThreadId { get; set; }

        [JsonPropertyName("direction")]
        public string? Direction { get; set; }

        [JsonPropertyName("inboundEmailId")]
        public string? InboundEmailId { get; set; }

        [JsonPropertyName("emailLogId")]
        public string? EmailLogId { get; set; }

        [JsonPropertyName("fromEmail")]
        public string? FromEmail { get; set; }

        [JsonPropertyName("fromName")]
        public string? FromName { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("preview")]
        public string? Preview { get; set; }

        [JsonPropertyName("messageAt")]
        public string? MessageAt { get; set; }

        [JsonPropertyName("rfc822MessageId")]
        public string? Rfc822MessageId { get; set; }

        [JsonPropertyName("inReplyTo")]
        public string? InReplyTo { get; set; }

        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("email")]
        public object? Email { get; set; }
    }

    public class ThreadDetail : Thread
    {
        [JsonPropertyName("messages")]
        public List<ThreadMessage>? Messages { get; set; }
    }

    public class ListThreadsParams
    {
        [JsonPropertyName("mailboxId")]
        public string? MailboxId { get; set; }

        [JsonPropertyName("limit")]
        public int? Limit { get; set; }

        [JsonPropertyName("offset")]
        public int? Offset { get; set; }

        [JsonPropertyName("folder")]
        public string? Folder { get; set; }

        [JsonPropertyName("q")]
        public string? Q { get; set; }

        [JsonPropertyName("isPinned")]
        public bool? IsPinned { get; set; }

        [JsonPropertyName("filter")]
        public string? Filter { get; set; }
    }

    public class BatchThreadsParams
    {
        [JsonPropertyName("ids")]
        public List<string>? Ids { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }
    }

    public class UpdateThreadParams
    {
        [JsonPropertyName("isRead")]
        public bool? IsRead { get; set; }

        [JsonPropertyName("isStarred")]
        public bool? IsStarred { get; set; }

        [JsonPropertyName("isImportant")]
        public bool? IsImportant { get; set; }

        [JsonPropertyName("isPinned")]
        public bool? IsPinned { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class SetThreadReadParams
    {
        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; }

        public SetThreadReadParams()
        {
            IsRead = true;
        }

        public SetThreadReadParams(bool isRead)
        {
            IsRead = isRead;
        }
    }

    public class SetThreadStarParams
    {
        [JsonPropertyName("isStarred")]
        public bool IsStarred { get; set; }

        public SetThreadStarParams()
        {
            IsStarred = true;
        }

        public SetThreadStarParams(bool isStarred)
        {
            IsStarred = isStarred;
        }
    }
}

using System.Text.Json.Serialization;

namespace Reloop.Models;

public static class MailModels
{
    public class SendMailTag
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        public SendMailTag()
        {
        }

        public SendMailTag(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public class SendMailAttachment
    {
        [JsonPropertyName("content")]
        public object? Content { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("content_id")]
        public string? ContentId { get; set; }
    }

    public class SendMailTemplate
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("variables")]
        public Dictionary<string, object>? Variables { get; set; }
    }

    public class SendMailParams
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("to")]
        public object? To { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("cc")]
        public object? Cc { get; set; }

        [JsonPropertyName("bcc")]
        public object? Bcc { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("html")]
        public string? Html { get; set; }

        [JsonPropertyName("reply_to")]
        public object? ReplyTo { get; set; }

        [JsonPropertyName("scheduled_at")]
        public string? ScheduledAt { get; set; }

        [JsonPropertyName("headers")]
        public Dictionary<string, string>? Headers { get; set; }

        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }

        [JsonPropertyName("attachments")]
        public List<SendMailAttachment>? Attachments { get; set; }

        [JsonPropertyName("tags")]
        public List<SendMailTag>? Tags { get; set; }

        [JsonPropertyName("template")]
        public SendMailTemplate? Template { get; set; }

        [JsonPropertyName("thread_id")]
        public string? ThreadId { get; set; }
    }

    public class SendMailResponse
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
}

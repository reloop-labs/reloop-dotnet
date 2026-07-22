using System.Collections.Generic;
using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.InboxModels;

namespace Reloop.Services;

/** Manages inbox messages. */
public class InboxMessagesService
{
    private const string MessagesV1 = "/api/inbox/v1/messages";
    private const int BatchIdsMax = 100;
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal InboxMessagesService(ReloopClient client)
    {
        _client = client;
    }

    public Task<Message[]?> ListAsync(ListMessagesParams? parameters = null)
    {
        var query = BuildListMessagesQuery(parameters);
        return _client.FetchAsync<Message[]>(HttpMethod.Get, MessagesV1, null, query);
    }

    public Task<Message[]?> ListSentAsync(ListSentMessagesParams? parameters = null)
    {
        var query = new Dictionary<string, string?>();
        if (parameters?.MailboxId != null)
        {
            query["mailboxId"] = parameters.MailboxId;
        }

        return _client.FetchAsync<Message[]>(HttpMethod.Get, MessagesV1 + "/sent", null, query);
    }

    public Task<Message?> GetAsync(string id)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        return _client.FetchAsync<Message>(HttpMethod.Get, MessagesV1 + "/" + messageId);
    }

    public Task<Message[]?> BatchAsync(BatchMessagesParams? parameters = null)
    {
        parameters ??= new BatchMessagesParams();
        var ids = Validators.RequireInboxIdArray(parameters.Ids, "ids", BatchIdsMax);
        return _client.FetchAsync<Message[]>(
            HttpMethod.Post,
            MessagesV1 + "/batch",
            new BatchMessagesParams { Ids = ids });
    }

    public Task<MessageRaw?> GetRawAsync(string id)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        return _client.FetchAsync<MessageRaw>(HttpMethod.Get, MessagesV1 + "/" + messageId + "/raw");
    }

    public Task<MessageAttachment?> GetAttachmentAsync(string id, string attachmentId)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        var attachment = Validators.RequireInboxAttachmentId(attachmentId, "attachmentId");
        return _client.FetchAsync<MessageAttachment>(
            HttpMethod.Get,
            MessagesV1 + "/" + messageId + "/attachments/" + attachment);
    }

    public Task<InboxSuccessResponse?> UpdateAsync(string id, UpdateMessageParams? parameters)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        if (parameters == null
            || (parameters.IsRead == null && parameters.IsStarred == null && parameters.IsSpam == null))
        {
            throw new ReloopValidationException(
                "update requires at least one of isRead, isStarred, or isSpam.", "params");
        }

        return _client.FetchAsync<InboxSuccessResponse>(PatchMethod, MessagesV1 + "/" + messageId, parameters);
    }

    public Task<InboxSuccessResponse?> SetReadAsync(string id, SetMessageReadParams? parameters = null)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        parameters ??= new SetMessageReadParams();
        return _client.FetchAsync<InboxSuccessResponse>(
            PatchMethod,
            MessagesV1 + "/" + messageId + "/read",
            new SetMessageReadParams(parameters.IsRead));
    }

    public Task<InboxSuccessResponse?> SetStarAsync(string id, SetMessageStarParams? parameters = null)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        parameters ??= new SetMessageStarParams();
        return _client.FetchAsync<InboxSuccessResponse>(
            PatchMethod,
            MessagesV1 + "/" + messageId + "/star",
            new SetMessageStarParams(parameters.IsStarred));
    }

    public Task<InboxSuccessResponse?> DeleteAsync(string id)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Delete, MessagesV1 + "/" + messageId);
    }

    public Task<SendEmailOrPendingResponse?> SendAsync(SendMessageParams? parameters)
    {
        parameters ??= new SendMessageParams();
        var mailboxId = Validators.RequireNonEmptyString(parameters.MailboxId, "mailboxId");
        var to = Validators.RequireRecipient(parameters.To, "to");
        var subject = Validators.RequireNonEmptyString(parameters.Subject, "subject");
        if (parameters.UndoWindowSeconds.HasValue)
        {
            Validators.RequireFiniteNumber(parameters.UndoWindowSeconds.Value, "undoWindowSeconds");
        }

        var body = new SendMessageParams
        {
            MailboxId = mailboxId,
            To = to,
            Subject = subject,
            Text = parameters.Text,
            Html = parameters.Html,
            ScheduledAt = parameters.ScheduledAt,
            UndoWindowSeconds = parameters.UndoWindowSeconds,
            Attachments = parameters.Attachments,
        };

        if (parameters.Cc != null)
        {
            body.Cc = Validators.RequireRecipient(parameters.Cc, "cc");
        }

        if (parameters.Bcc != null)
        {
            body.Bcc = Validators.RequireRecipient(parameters.Bcc, "bcc");
        }

        return _client.FetchAsync<SendEmailOrPendingResponse>(HttpMethod.Post, MessagesV1 + "/send", body);
    }

    public Task<InboxSuccessResponse?> CancelPendingAsync(string id)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(
            HttpMethod.Post,
            MessagesV1 + "/pending/" + messageId + "/cancel");
    }

    public Task<SendEmailResponse?> ReplyAsync(string id, ComposeMessageParams? parameters)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        var body = BuildComposeBody(parameters);
        return _client.FetchAsync<SendEmailResponse>(
            HttpMethod.Post,
            MessagesV1 + "/" + messageId + "/reply",
            body);
    }

    public Task<SendEmailResponse?> ReplyAllAsync(string id, ComposeMessageParams? parameters)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        var body = BuildComposeBody(parameters);
        return _client.FetchAsync<SendEmailResponse>(
            HttpMethod.Post,
            MessagesV1 + "/" + messageId + "/reply-all",
            body);
    }

    public Task<SendEmailResponse?> ForwardAsync(string id, ForwardMessageParams? parameters)
    {
        var messageId = Validators.RequireMessageId(id, "id");
        parameters ??= new ForwardMessageParams();
        var to = Validators.RequireRecipient(parameters.To, "to");
        var body = BuildComposeBody(parameters);
        body["to"] = to;
        return _client.FetchAsync<SendEmailResponse>(
            HttpMethod.Post,
            MessagesV1 + "/" + messageId + "/forward",
            body);
    }

    private static Dictionary<string, string?> BuildListMessagesQuery(ListMessagesParams? parameters)
    {
        var query = new Dictionary<string, string?>();
        if (parameters == null)
        {
            return query;
        }

        if (parameters.MailboxId != null)
        {
            query["mailboxId"] = parameters.MailboxId;
        }

        if (parameters.Limit.HasValue)
        {
            Validators.RequireInboxLimit(parameters.Limit.Value, "limit");
            query["limit"] = parameters.Limit.Value.ToString();
        }

        if (parameters.Offset.HasValue)
        {
            Validators.RequireInboxOffset(parameters.Offset.Value, "offset");
            query["offset"] = parameters.Offset.Value.ToString();
        }

        if (parameters.Q != null)
        {
            query["q"] = parameters.Q;
        }

        if (parameters.IsSpam.HasValue)
        {
            query["isSpam"] = parameters.IsSpam.Value.ToString().ToLowerInvariant();
        }

        return query;
    }

    private static Dictionary<string, object?> BuildComposeBody(ComposeMessageParams? parameters)
    {
        parameters ??= new ComposeMessageParams();
        Validators.RequireComposeBody(parameters.Text, parameters.Html);

        var body = new Dictionary<string, object?>();
        if (parameters.Text != null)
        {
            body["text"] = parameters.Text;
        }

        if (parameters.Html != null)
        {
            body["html"] = parameters.Html;
        }

        if (parameters.Cc != null)
        {
            body["cc"] = Validators.RequireRecipient(parameters.Cc, "cc");
        }

        if (parameters.Bcc != null)
        {
            body["bcc"] = Validators.RequireRecipient(parameters.Bcc, "bcc");
        }

        if (parameters.Attachments != null)
        {
            body["attachments"] = parameters.Attachments;
        }

        return body;
    }
}

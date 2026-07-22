using System.Collections.Generic;
using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.InboxModels;
using InboxThread = Reloop.Models.InboxModels.Thread;

namespace Reloop.Services;

/** Manages inbox threads. */
public class InboxThreadsService
{
    private const string ThreadsV1 = "/api/inbox/v1/threads";
    private const int BatchIdsMax = 100;
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal InboxThreadsService(ReloopClient client)
    {
        _client = client;
    }

    public Task<InboxThread[]?> ListAsync(ListThreadsParams? parameters = null)
    {
        var query = BuildListThreadsQuery(parameters);
        return _client.FetchAsync<InboxThread[]>(HttpMethod.Get, ThreadsV1, null, query);
    }

    public Task<ThreadBatchResponse?> BatchAsync(BatchThreadsParams? parameters = null)
    {
        parameters ??= new BatchThreadsParams();
        var ids = Validators.RequireInboxIdArray(parameters.Ids, "ids", BatchIdsMax);
        Validators.RequireThreadBatchAction(parameters.Action);
        return _client.FetchAsync<ThreadBatchResponse>(
            HttpMethod.Post,
            ThreadsV1 + "/batch",
            new BatchThreadsParams { Ids = ids, Action = parameters.Action });
    }

    public Task<ThreadDetail?> GetAsync(string id)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        return _client.FetchAsync<ThreadDetail>(HttpMethod.Get, ThreadsV1 + "/" + threadId);
    }

    public Task<MessageAttachment?> GetAttachmentAsync(string id, string attachmentId)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        var attachment = Validators.RequireInboxAttachmentId(attachmentId, "attachmentId");
        return _client.FetchAsync<MessageAttachment>(
            HttpMethod.Get,
            ThreadsV1 + "/" + threadId + "/attachments/" + attachment);
    }

    public Task<InboxSuccessResponse?> UpdateAsync(string id, UpdateThreadParams? parameters)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        if (parameters == null
            || (parameters.IsRead == null
                && parameters.IsStarred == null
                && parameters.IsImportant == null
                && parameters.IsPinned == null
                && parameters.Status == null))
        {
            throw new ReloopValidationException(
                "update requires at least one of isRead, isStarred, isImportant, isPinned, or status.",
                "params");
        }

        if (parameters.Status != null)
        {
            Validators.RequireThreadStatus(parameters.Status, "status");
        }

        return _client.FetchAsync<InboxSuccessResponse>(PatchMethod, ThreadsV1 + "/" + threadId, parameters);
    }

    public Task<InboxSuccessResponse?> SetReadAsync(string id, SetThreadReadParams? parameters = null)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        parameters ??= new SetThreadReadParams();
        return _client.FetchAsync<InboxSuccessResponse>(
            PatchMethod,
            ThreadsV1 + "/" + threadId + "/read",
            new SetThreadReadParams(parameters.IsRead));
    }

    public Task<InboxSuccessResponse?> SetStarAsync(string id, SetThreadStarParams? parameters = null)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        parameters ??= new SetThreadStarParams();
        return _client.FetchAsync<InboxSuccessResponse>(
            PatchMethod,
            ThreadsV1 + "/" + threadId + "/star",
            new SetThreadStarParams(parameters.IsStarred));
    }

    public Task<InboxSuccessResponse?> ArchiveAsync(string id)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Post, ThreadsV1 + "/" + threadId + "/archive");
    }

    public Task<InboxSuccessResponse?> TrashAsync(string id)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Post, ThreadsV1 + "/" + threadId + "/trash");
    }

    public Task<InboxSuccessResponse?> RestoreAsync(string id)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Post, ThreadsV1 + "/" + threadId + "/restore");
    }

    public Task<InboxSuccessResponse?> DeleteAsync(string id)
    {
        var threadId = Validators.RequireThreadId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Delete, ThreadsV1 + "/" + threadId);
    }

    private static Dictionary<string, string?> BuildListThreadsQuery(ListThreadsParams? parameters)
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

        if (parameters.Folder != null)
        {
            query["folder"] = parameters.Folder;
        }

        if (parameters.Q != null)
        {
            query["q"] = parameters.Q;
        }

        if (parameters.IsPinned.HasValue)
        {
            query["isPinned"] = parameters.IsPinned.Value.ToString().ToLowerInvariant();
        }

        if (parameters.Filter != null)
        {
            Validators.RequireThreadFilter(parameters.Filter, "filter");
            query["filter"] = parameters.Filter;
        }

        return query;
    }
}

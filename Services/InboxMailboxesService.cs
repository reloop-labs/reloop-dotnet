using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.InboxModels;

namespace Reloop.Services;

/** Manages inbox mailboxes. */
public class InboxMailboxesService
{
    private const string MailboxesV1 = "/api/inbox/v1/mailboxes";
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal InboxMailboxesService(ReloopClient client)
    {
        _client = client;
    }

    public Task<Mailbox[]?> ListAsync()
    {
        return _client.FetchAsync<Mailbox[]>(HttpMethod.Get, MailboxesV1 + "/list");
    }

    public Task<MailboxDetail?> GetAsync(string id)
    {
        var mailboxId = Validators.RequireMailboxId(id, "id");
        return _client.FetchAsync<MailboxDetail>(HttpMethod.Get, MailboxesV1 + "/" + mailboxId);
    }

    public Task<CreateMailboxResponse?> CreateAsync(CreateMailboxParams? parameters)
    {
        parameters ??= new CreateMailboxParams();
        var domainId = Validators.RequireNonEmptyString(parameters.DomainId, "domainId");
        var email = Validators.RequireNonEmptyString(parameters.Email, "email");

        var body = new CreateMailboxParams
        {
            DomainId = domainId,
            Email = email,
            Password = parameters.Password,
            Quota = parameters.Quota,
            DisplayName = parameters.DisplayName,
        };

        return _client.FetchAsync<CreateMailboxResponse>(HttpMethod.Post, MailboxesV1 + "/create", body);
    }

    public Task<InboxSuccessResponse?> UpdateAsync(string id, UpdateMailboxParams? parameters)
    {
        var mailboxId = Validators.RequireMailboxId(id, "id");
        if (parameters == null
            || (parameters.DisplayName == null && parameters.Status == null && parameters.Quota == null))
        {
            throw new ReloopValidationException(
                "update requires at least one of displayName, status, or quota.", "params");
        }

        if (parameters.Status != null)
        {
            Validators.RequireMailboxStatus(parameters.Status, "status");
        }

        return _client.FetchAsync<InboxSuccessResponse>(PatchMethod, MailboxesV1 + "/" + mailboxId, parameters);
    }

    public Task<InboxSuccessResponse?> DeleteAsync(string id)
    {
        var mailboxId = Validators.RequireMailboxId(id, "id");
        return _client.FetchAsync<InboxSuccessResponse>(HttpMethod.Delete, MailboxesV1 + "/" + mailboxId);
    }
}

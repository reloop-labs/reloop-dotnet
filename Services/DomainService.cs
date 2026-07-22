using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Validation;
using static Reloop.Models.DomainModels;

namespace Reloop.Services;

/** Manages sending/receiving domains. */
public class DomainService
{
    private const string DomainV1 = "/api/domain/v1";

    private readonly ReloopClient _client;

    internal DomainService(ReloopClient client)
    {
        _client = client;
    }

    public Task<Domain?> CreateAsync(CreateDomainParams? parameters)
    {
        var domain = Validators.RequireNonEmptyString(parameters?.Domain, "domain");
        var body = new CreateDomainParams(domain);
        if (parameters != null)
        {
            body.ClickTracking = parameters.ClickTracking;
            body.OpenTracking = parameters.OpenTracking;
            body.Tls = parameters.Tls;
            body.SendingEmail = parameters.SendingEmail;
            body.ReceivingEmail = parameters.ReceivingEmail;
        }

        return _client.FetchAsync<Domain>(HttpMethod.Post, DomainV1 + "/create", body);
    }

    public Task<DomainListResponse?> ListAsync(ListDomainsParams? parameters = null)
    {
        var query = new Dictionary<string, string?>();
        if (parameters != null)
        {
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

            if (parameters.Q != null)
            {
                query["q"] = parameters.Q;
            }

            if (parameters.Status != null)
            {
                query["status"] = parameters.Status;
            }
        }

        return _client.FetchAsync<DomainListResponse>(HttpMethod.Get, DomainV1 + "/list", null, query);
    }

    public Task<Domain?> GetAsync(string domainId)
    {
        var id = Validators.RequireNonEmptyString(domainId, "domainId");
        return _client.FetchAsync<Domain>(HttpMethod.Get, DomainV1 + "/" + id);
    }

    public Task<Domain?> UpdateAsync(string domainId, UpdateDomainParams? parameters)
    {
        var id = Validators.RequireNonEmptyString(domainId, "domainId");
        return _client.FetchAsync<Domain>(
            new HttpMethod("PATCH"),
            DomainV1 + "/" + id,
            parameters ?? new UpdateDomainParams());
    }

    public Task DeleteAsync(string domainId)
    {
        var id = Validators.RequireNonEmptyString(domainId, "domainId");
        return _client.FetchAsync<object>(HttpMethod.Delete, DomainV1 + "/" + id);
    }

    public Task<DomainStatusResponse?> VerifyAsync(string domainId)
    {
        var id = Validators.RequireNonEmptyString(domainId, "domainId");
        return _client.FetchAsync<DomainStatusResponse>(
            HttpMethod.Post,
            DomainV1 + "/verify/" + id,
            new Dictionary<string, object>());
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Reloop.Models;

namespace Reloop.Services
{
    public class DomainService
    {
        private readonly ReloopClient _client;

        internal DomainService(ReloopClient client)
        {
            _client = client;
        }

        public Task<Domain?> CreateAsync(CreateDomainParams @params)
        {
            return _client.FetchAsync<Domain>(HttpMethod.Post, "/api/domain/v1/create", @params);
        }

        public Task<DomainListResponse?> ListAsync(ListDomainsParams? @params = null)
        {
            var path = "/api/domain/v1/list";
            var query = BuildListQuery(@params);
            if (!string.IsNullOrEmpty(query))
            {
                path += "?" + query;
            }

            return _client.FetchAsync<DomainListResponse>(HttpMethod.Get, path);
        }

        public Task<Domain?> GetAsync(string domainId)
        {
            return _client.FetchAsync<Domain>(HttpMethod.Get, $"/api/domain/v1/{domainId}");
        }

        public Task<DomainNameserversResponse?> GetNameserversAsync(string domainId)
        {
            return _client.FetchAsync<DomainNameserversResponse>(
                HttpMethod.Get,
                $"/api/domain/v1/nameservers/{domainId}");
        }

        public Task<Domain?> UpdateAsync(string domainId, UpdateDomainParams @params)
        {
            return _client.FetchAsync<Domain>(
                new HttpMethod("PATCH"),
                $"/api/domain/v1/{domainId}",
                @params);
        }

        public Task<Domain?> DeleteAsync(string domainId)
        {
            return _client.FetchAsync<Domain>(HttpMethod.Delete, $"/api/domain/v1/{domainId}");
        }

        public Task<DomainStatusResponse?> VerifyAsync(string domainId)
        {
            return _client.FetchAsync<DomainStatusResponse>(
                HttpMethod.Post,
                $"/api/domain/v1/verify/{domainId}");
        }

        public Task<ForwardDnsResponse?> ForwardDnsAsync(string domainId, ForwardDnsParams @params)
        {
            return _client.FetchAsync<ForwardDnsResponse>(
                HttpMethod.Post,
                $"/api/domain/v1/verify/{domainId}/forward-dns",
                @params);
        }

        internal static string BuildListQuery(ListDomainsParams? @params)
        {
            if (@params == null)
            {
                return string.Empty;
            }

            var query = new List<string>();
            if (@params.Page.HasValue) query.Add($"page={@params.Page.Value}");
            if (@params.Limit.HasValue) query.Add($"limit={@params.Limit.Value}");
            if (!string.IsNullOrEmpty(@params.Q)) query.Add($"q={HttpUtility.UrlEncode(@params.Q)}");
            if (!string.IsNullOrEmpty(@params.Status)) query.Add($"status={HttpUtility.UrlEncode(@params.Status)}");

            return string.Join("&", query);
        }
    }
}

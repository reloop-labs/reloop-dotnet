using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Reloop.Models;

namespace Reloop.Services
{
    public class ApiKeyService
    {
        private readonly ReloopClient _client;

        internal ApiKeyService(ReloopClient client)
        {
            _client = client;
        }

        public Task<ApiKeyWithKey?> CreateAsync(CreateApiKeyParams @params)
        {
            return _client.FetchAsync<ApiKeyWithKey>(HttpMethod.Post, "/api/api-key/v1/", @params);
        }

        public Task<ApiKeyListResponse?> ListAsync(ApiKeyListParams? @params = null)
        {
            var query = new List<string>();
            if (@params != null)
            {
                if (@params.Page.HasValue) query.Add($"page={@params.Page.Value}");
                if (@params.Limit.HasValue) query.Add($"limit={@params.Limit.Value}");
                if (@params.Enabled.HasValue) query.Add($"enabled={@params.Enabled.Value.ToString().ToLower()}");
                if (!string.IsNullOrEmpty(@params.UserId)) query.Add($"userId={HttpUtility.UrlEncode(@params.UserId)}");
                if (!string.IsNullOrEmpty(@params.Q)) query.Add($"q={HttpUtility.UrlEncode(@params.Q)}");
            }

            var path = "/api/api-key/v1/";
            if (query.Count > 0)
            {
                path += "?" + string.Join("&", query);
            }

            return _client.FetchAsync<ApiKeyListResponse>(HttpMethod.Get, path);
        }

        public Task<ApiKey?> GetAsync(string id)
        {
            return _client.FetchAsync<ApiKey>(HttpMethod.Get, $"/api/api-key/v1/{id}");
        }

        public Task<ApiKey?> UpdateAsync(string id, UpdateApiKeyParams @params)
        {
            return _client.FetchAsync<ApiKey>(new HttpMethod("PATCH"), $"/api/api-key/v1/{id}", @params);
        }

        public Task<DeleteApiKeyResponse?> DeleteAsync(string id)
        {
            return _client.FetchAsync<DeleteApiKeyResponse>(HttpMethod.Delete, $"/api/api-key/v1/{id}");
        }

        public Task<ApiKeyWithKey?> RotateAsync(string id)
        {
            return _client.FetchAsync<ApiKeyWithKey>(HttpMethod.Post, $"/api/api-key/v1/rotate/{id}");
        }

        public Task<ApiKey?> EnableAsync(string id)
        {
            return _client.FetchAsync<ApiKey>(HttpMethod.Post, $"/api/api-key/v1/enable/{id}");
        }

        public Task<ApiKey?> DisableAsync(string id)
        {
            return _client.FetchAsync<ApiKey>(HttpMethod.Post, $"/api/api-key/v1/disable/{id}");
        }

        public Task<ApiKey?> PauseAsync(string id)
        {
            return DisableAsync(id);
        }
    }
}

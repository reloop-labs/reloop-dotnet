using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Reloop.Services;

namespace Reloop
{
    public class ReloopException : Exception
    {
        public ReloopException(string message) : base(message) { }
        public ReloopException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ReloopClient : IDisposable
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiKeyService ApiKeys { get; }
        public ContactsService Contacts { get; }
        public DomainService Domain { get; }
        public MailService Mail { get; }

        public ReloopClient(string apiKey, string baseUrl = "https://reloop.sh", HttpClient? httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("Reloop SDK requires an apiKey.");

            _apiKey = apiKey;
            _baseUrl = baseUrl;

            _httpClient = httpClient ?? new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };

            ApiKeys = new ApiKeyService(this);
            Contacts = new ContactsService(this);
            Domain = new DomainService(this);
            Mail = new MailService(this);
        }

        public async Task<T?> FetchAsync<T>(HttpMethod method, string path, object? body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, path);

                if (body != null)
                {
                    var json = JsonSerializer.Serialize(body, _jsonOptions);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new ReloopException($"Reloop API Error: {(int)response.StatusCode} {response.ReasonPhrase}. {errorBody}");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NoContent || typeof(T) == typeof(object))
                {
                    return default;
                }

                var responseStream = await response.Content.ReadAsStreamAsync();
                return await JsonSerializer.DeserializeAsync<T>(responseStream, _jsonOptions);
            }
            catch (HttpRequestException ex)
            {
                throw new ReloopException("Reloop Network Error", ex);
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

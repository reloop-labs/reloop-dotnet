using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Reloop.Exceptions;
using Reloop.Services;

namespace Reloop;

public class ReloopClient : IDisposable
{
    private const string DefaultBaseUrl = "https://reloop.sh";

    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly JsonSerializerOptions _jsonOptionsIncludeNull;

    public ApiKeyService ApiKey { get; }
    public ContactsService Contacts { get; }
    public DomainService Domain { get; }
    public MailService Mail { get; }
    public WebhookService Webhook { get; }
    public InboxService Inbox { get; }

    public ReloopClient(string apiKey, string baseUrl = DefaultBaseUrl, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("Reloop SDK requires an apiKey.");
        }

        _apiKey = apiKey.Trim();
        _baseUrl = NormalizeBaseUrl(baseUrl);

        _ownsHttpClient = httpClient == null;
        _httpClient = httpClient ?? new HttpClient();

        _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        };
        _jsonOptionsIncludeNull = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        ApiKey = new ApiKeyService(this);
        Contacts = new ContactsService(this);
        Domain = new DomainService(this);
        Mail = new MailService(this);
        Webhook = new WebhookService(this);
        Inbox = new InboxService(this);
    }

    public string BaseUrl => _baseUrl;

    internal JsonSerializerOptions JsonOptions => _jsonOptions;

    public Task<T?> FetchAsync<T>(HttpMethod method, string path, object? body = null)
    {
        return FetchAsync<T>(method, path, body, null);
    }

    public async Task<T?> FetchAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        Dictionary<string, string?>? query)
    {
        try
        {
            var requestUri = BuildRequestUri(path, query);
            using var request = new HttpRequestMessage(method, requestUri);
            request.Headers.TryAddWithoutValidation("x-api-key", _apiKey);
            request.Headers.TryAddWithoutValidation("User-Agent", "reloop-dotnet/0.1.0");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (body != null)
            {
                var options = ShouldIncludeNulls(body) ? _jsonOptionsIncludeNull : _jsonOptions;
                var json = JsonSerializer.Serialize(body, options);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                var errBody = new ApiErrorBody();
                try
                {
                    if (!string.IsNullOrWhiteSpace(errorText))
                    {
                        errBody = JsonSerializer.Deserialize<ApiErrorBody>(errorText, _jsonOptions)
                            ?? new ApiErrorBody();
                    }
                }
                catch (JsonException)
                {
                    errBody.Message = errorText;
                }

                throw new ReloopApiException(
                    (int)response.StatusCode,
                    response.ReasonPhrase ?? ((int)response.StatusCode).ToString(),
                    errBody);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return default;
            }

            var responseText = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseText))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseText, _jsonOptions);
            }
            catch (JsonException ex)
            {
                throw new ReloopApiException("Reloop response parsing error: " + ex.Message, ex);
            }
        }
        catch (ReloopApiException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ReloopApiException("Reloop network error: " + ex.Message, ex);
        }
    }

    internal static string NormalizeBaseUrl(string? baseUrl)
    {
        var trimmed = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl!.Trim();
        while (trimmed.EndsWith("/", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^1];
        }

        return string.IsNullOrEmpty(trimmed) ? DefaultBaseUrl : trimmed;
    }

    private Uri BuildRequestUri(string path, Dictionary<string, string?>? query)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Request path is required.", nameof(path));
        }

        // Never attach the API key to a caller-supplied absolute URL (credential exfiltration).
        // Check the raw path before query append; avoid Uri.TryCreate Absolute on "/..." which
        // resolves as file:// on Unix and would incorrectly reject relative API paths.
        if (path.IndexOf("://", StringComparison.Ordinal) >= 0
            || path.StartsWith("//", StringComparison.Ordinal))
        {
            throw new ArgumentException("Request paths must be relative.", nameof(path));
        }

        var pathWithQuery = AppendQuery(path, query);
        if (!pathWithQuery.StartsWith("/", StringComparison.Ordinal))
        {
            pathWithQuery = "/" + pathWithQuery;
        }

        return new Uri(_baseUrl + pathWithQuery);
    }

    private static string AppendQuery(string path, Dictionary<string, string?>? query)
    {
        if (query == null || query.Count == 0)
        {
            return path;
        }

        var parts = query
            .Where(entry => entry.Value != null)
            .Select(entry => Uri.EscapeDataString(entry.Key) + "=" + Uri.EscapeDataString(entry.Value!))
            .ToList();

        if (parts.Count == 0)
        {
            return path;
        }

        var separator = path.IndexOf('?') >= 0 ? "&" : "?";
        return path + separator + string.Join("&", parts);
    }

    private static bool ShouldIncludeNulls(object body)
    {
        return body is IDictionary;
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}

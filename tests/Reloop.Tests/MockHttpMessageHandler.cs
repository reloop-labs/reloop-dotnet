using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Reloop.Tests;

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    public HttpStatusCode ResponseStatusCode { get; set; } = HttpStatusCode.OK;
    public string ResponseBody { get; set; } = "{\"id\":\"test_1\"}";
    public HttpRequestMessage? LastRequest { get; private set; }
    public int RequestCount { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        RequestCount++;

        return Task.FromResult(new HttpResponseMessage(ResponseStatusCode)
        {
            Content = new StringContent(ResponseBody),
        });
    }
}

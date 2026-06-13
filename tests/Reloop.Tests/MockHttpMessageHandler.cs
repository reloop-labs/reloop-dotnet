using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Reloop.Tests;

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"id\":\"test_1\"}")
        });
    }
}

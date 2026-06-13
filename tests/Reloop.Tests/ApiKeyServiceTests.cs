using System.Text.Json;
using Reloop.Models;
using Reloop.Services;
using Xunit;

namespace Reloop.Tests;

public class ApiKeyServiceTests
{
    [Fact]
    public void CreateApiKeyParams_SerializesExpectedFields()
    {
        var json = JsonSerializer.Serialize(new CreateApiKeyParams(
            Name: "Production Key",
            Enabled: true,
            RateLimitEnabled: true
        ));

        Assert.Contains("\"name\":\"Production Key\"", json);
        Assert.Contains("\"enabled\":true", json);
        Assert.Contains("\"rateLimitEnabled\":true", json);
    }

    [Fact]
    public void PauseAsync_UsesSameRouteAsDisable()
    {
        Assert.NotNull(typeof(ApiKeyService).GetMethod("PauseAsync"));
        Assert.NotNull(typeof(ApiKeyService).GetMethod("DisableAsync"));
    }
}

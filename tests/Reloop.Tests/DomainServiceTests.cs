using System.Text.Json;
using Reloop.Models;
using Reloop.Services;
using Xunit;

namespace Reloop.Tests;

public class DomainServiceTests
{
    [Fact]
    public void CreateDomainParams_SerializesWithSnakeCase()
    {
        var json = JsonSerializer.Serialize(new CreateDomainParams(
            Domain: "send.example.com",
            CustomReturnPath: "inbound",
            ClickTracking: true));

        Assert.Contains("\"click_tracking\":true", json);
        Assert.Contains("\"custom_return_path\":\"inbound\"", json);
        Assert.DoesNotContain("clickTracking", json);
    }

    [Fact]
    public void UpdateDomainParams_SerializesWithSnakeCase()
    {
        var json = JsonSerializer.Serialize(new UpdateDomainParams(
            ClickTracking: false,
            OpenTracking: true,
            Tls: "enforced"));

        Assert.Contains("\"click_tracking\":false", json);
        Assert.DoesNotContain("clickTracking", json);
    }

    [Fact]
    public void BuildListQuery_IncludesFilters()
    {
        var query = DomainService.BuildListQuery(new ListDomainsParams
        {
            Page = 2,
            Limit = 5,
            Q = "example",
            Status = "active",
        });

        Assert.Equal("page=2&limit=5&q=example&status=active", query);
    }
}

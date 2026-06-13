using System.Net.Http;
using Xunit;

namespace Reloop.Tests;

public class ContactsServiceRouteTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient()
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://reloop.sh")
        };

        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_UsesContactsCreateRoute()
    {
        var (client, handler) = CreateClient();

        await client.Contacts.CreateAsync(new Dictionary<string, object?>
        {
            ["email"] = "user@example.com",
            ["first_name"] = "Ada",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/create", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task GetAsync_UsesRetrieveRoute()
    {
        var (client, handler) = CreateClient();

        await client.Contacts.GetAsync("con_1");

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/retrieve/con_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ListAsync_WithGroupId_UsesGroupContactsRoute()
    {
        var (client, handler) = CreateClient();

        await client.Contacts.ListAsync(new Dictionary<string, object?>
        {
            ["groupId"] = "grp_1",
            ["page"] = 1,
        });

        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/v1/groups/grp_1/contacts?page=1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task ChannelsAddContactAsync_UsesChannelRoute()
    {
        var (client, handler) = CreateClient();

        await client.Contacts.Channels.AddContactAsync("ch_1", new Dictionary<string, object?>
        {
            ["contact_id"] = "con_1",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/channel/ch_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}

using System.Net.Http;
using Reloop.Models;
using Xunit;
using static Reloop.Models.ContactModels;

namespace Reloop.Tests;

public class ContactsServiceRouteTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient()
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://reloop.sh") };
        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_UsesContactsCreateRoute()
    {
        var (client, handler) = CreateClient();
        await client.Contacts.CreateAsync(new CreateContactParams { Email = "user@example.com", FirstName = "Ada" });
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/create", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task Groups_ListContactsAsync_UsesGroupContactsRoute()
    {
        var (client, handler) = CreateClient();
        await client.Contacts.Groups.ListContactsAsync("grp_1", new ListGroupContactsParams { Page = 1 });
        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/v1/groups/grp_1/contacts?page=1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task Channels_AddContactAsync_UsesChannelRoute()
    {
        var (client, handler) = CreateClient();
        await client.Contacts.Channels.AddContactAsync("ch_1", new AddContactToChannelParams { ContactId = "con_1" });
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/channel/ch_1", handler.LastRequest?.RequestUri?.PathAndQuery);
    }

    [Fact]
    public async Task Properties_CreateAsync_UsesPropertiesCreateRoute()
    {
        var (client, handler) = CreateClient();
        await client.Contacts.Properties.CreateAsync(new CreatePropertyParams { Name = "company", Type = PropertyType.String });
        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/v1/properties/create", handler.LastRequest?.RequestUri?.PathAndQuery);
    }
}

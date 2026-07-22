using System.Net;
using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.ContactModels;

namespace Reloop.Tests;

public class ContactsServiceTests
{
    private static (ReloopClient Client, MockHttpMessageHandler Handler) CreateClient(
        Action<MockHttpMessageHandler>? configure = null)
    {
        var handler = new MockHttpMessageHandler();
        configure?.Invoke(handler);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://reloop.sh") };
        return (new ReloopClient("rl_test", "https://reloop.sh", httpClient), handler);
    }

    [Fact]
    public async Task CreateAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody =
                "{\"object\":\"contact\",\"id\":\"con_1\",\"email\":\"john@example.com\",\"status\":\"subscribed\",\"event\":\"contact.created\"}";
        });

        var response = await client.Contacts.CreateAsync(new CreateContactParams
        {
            Email = "john@example.com",
            FirstName = "John",
            Status = ContactStatus.Subscribed,
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/contacts/create", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Contains("\"email\":\"john@example.com\"", await handler.LastRequest!.Content!.ReadAsStringAsync());
        Assert.Equal("con_1", response!.Id);
    }

    [Fact]
    public async Task CreateAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();
        await Assert.ThrowsAsync<ReloopValidationException>(() => client.Contacts.CreateAsync(new CreateContactParams
        {
            Email = "not-an-email",
        }));
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task UpdateAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();
        await Assert.ThrowsAsync<ReloopValidationException>(
            () => client.Contacts.UpdateAsync("con_1", new UpdateContactParams()));
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(ContactsService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(ContactsService) && !m.IsSpecialName)
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(
            new HashSet<string> { "CreateAsync", "DeleteAsync", "GetAsync", "ListAsync", "UpdateAsync" },
            methods);
    }
}

using System.Net;
using System.Net.Http;
using Reloop.Exceptions;
using Reloop.Models;
using Reloop.Services;
using Xunit;
using static Reloop.Models.InboxModels;

namespace Reloop.Tests;

public class InboxMailboxesServiceTests
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
            h.ResponseBody = "{\"id\":\"mbx_1\",\"email\":\"user@example.com\",\"status\":\"active\"}";
        });

        var response = await client.Inbox.Mailboxes.CreateAsync(new CreateMailboxParams
        {
            DomainId = "dom_1",
            Email = "user@example.com",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/inbox/v1/mailboxes/create", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Equal("mbx_1", response!.Id);
    }

    [Fact]
    public async Task GetAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();
        var err = await Assert.ThrowsAsync<ReloopValidationException>(() => client.Inbox.Mailboxes.GetAsync("  "));
        Assert.Equal("id", err.Field);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(InboxMailboxesService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(InboxMailboxesService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(new HashSet<string> { "CreateAsync", "DeleteAsync", "GetAsync", "ListAsync", "UpdateAsync" }, methods);
    }
}

public class InboxMessagesServiceTests
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
    public async Task SendAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody =
                "{\"success\":true,\"messageId\":\"msg_1\",\"status\":\"queued\",\"timestamp\":\"t\",\"id\":\"em_1\"}";
        });

        var response = await client.Inbox.Messages.SendAsync(new SendMessageParams
        {
            MailboxId = "mbx_1",
            To = "user@example.com",
            Subject = "Hello",
            Html = "<p>Hi</p>",
        });

        Assert.Equal(HttpMethod.Post, handler.LastRequest?.Method);
        Assert.Equal("/api/inbox/v1/messages/send", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.True(response!.Success);
        Assert.Equal("em_1", response.Id);
    }

    [Fact]
    public async Task SendAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();
        var err = await Assert.ThrowsAsync<ReloopValidationException>(() => client.Inbox.Messages.SendAsync(new SendMessageParams
        {
            MailboxId = "",
            To = "user@example.com",
            Subject = "Hello",
        }));
        Assert.Equal("mailboxId", err.Field);

        var bodyErr = await Assert.ThrowsAsync<ReloopValidationException>(() => client.Inbox.Messages.SendAsync(new SendMessageParams
        {
            MailboxId = "mbx_1",
            To = "user@example.com",
            Subject = "Hello",
        }));
        Assert.Equal("params", bodyErr.Field);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task SetReadAsync_DefaultsToTrue()
    {
        var (client, handler) = CreateClient();
        await client.Inbox.Messages.SetReadAsync("msg_1");
        Assert.Contains("\"isRead\":true", handler.LastRequestBody);
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(InboxMessagesService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(InboxMessagesService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(
            new HashSet<string>
            {
                "BatchAsync", "CancelPendingAsync", "DeleteAsync", "ForwardAsync", "GetAsync", "GetAttachmentAsync",
                "GetRawAsync", "ListAsync", "ListSentAsync", "ReplyAllAsync", "ReplyAsync", "SendAsync", "SetReadAsync",
                "SetStarAsync", "UpdateAsync",
            },
            methods);
    }
}

public class InboxThreadsServiceTests
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
    public async Task ListAsync_HappyPath()
    {
        var (client, handler) = CreateClient(h =>
        {
            h.ResponseBody =
                "[{\"id\":\"thr_1\",\"organizationId\":\"org_1\",\"lastMessageAt\":\"t\",\"status\":\"active\"}]";
        });

        var response = await client.Inbox.Threads.ListAsync(new ListThreadsParams { Limit = 50 });
        Assert.Equal(HttpMethod.Get, handler.LastRequest?.Method);
        Assert.Equal("/api/inbox/v1/threads?limit=50", handler.LastRequest?.RequestUri?.PathAndQuery);
        Assert.Single(response!);
        Assert.Equal("thr_1", response![0].Id);
    }

    [Fact]
    public async Task BatchAsync_ValidationNoHttp()
    {
        var (client, handler) = CreateClient();
        var err = await Assert.ThrowsAsync<ReloopValidationException>(() => client.Inbox.Threads.BatchAsync(new BatchThreadsParams
        {
            Ids = new List<string>(),
            Action = ThreadBatchAction.Archive,
        }));
        Assert.Equal("ids", err.Field);
        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public void SurfaceLock()
    {
        var methods = typeof(InboxThreadsService)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(m => m.DeclaringType == typeof(InboxThreadsService))
            .Select(m => m.Name)
            .ToHashSet();

        Assert.Equal(
            new HashSet<string>
            {
                "ArchiveAsync", "BatchAsync", "DeleteAsync", "GetAsync", "GetAttachmentAsync", "ListAsync",
                "RestoreAsync", "SetReadAsync", "SetStarAsync", "TrashAsync", "UpdateAsync",
            },
            methods);
    }
}

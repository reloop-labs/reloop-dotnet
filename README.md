# Reloop .NET SDK

Official .NET client for the [Reloop](https://reloop.sh) API (package **`Reloop`**, version **2.0.0**).

## Before you send

1. **API key** — create one in your Reloop account
2. **Verified domain** — add and verify a sending domain; use it in the `from` address

Full API reference: [reloop.sh/docs](https://reloop.sh/docs)

## Install

```bash
dotnet add package Reloop --version 2.0.0
```

## Quick start

```csharp
using Reloop;
using Reloop.Exceptions;
using Reloop.Models;
using static Reloop.Models.MailModels;

var client = new ReloopClient("rl_your_api_key_here");

try
{
    var result = await client.Mail.SendAsync(new SendMailParams
    {
        From = "Reloop <hello@your-verified-domain.com>",
        To = "user@example.com",
        Subject = "Welcome to Reloop",
        Html = "<p>Thanks for signing up.</p>",
        Text = "Thanks for signing up.",
    });

    Console.WriteLine($"{result?.MessageId} {result?.Id}");
}
catch (ReloopValidationException ex)
{
    Console.Error.WriteLine($"invalid request ({ex.Field}): {ex.Message}");
}
catch (ReloopApiException ex)
{
    Console.Error.WriteLine($"API error {ex.Status}: {ex.Message}");
}
```

## Services

```csharp
client.ApiKey;    // CreateAsync, ListAsync, GetAsync, UpdateAsync, DeleteAsync, RotateAsync, EnableAsync, DisableAsync
client.Mail;      // SendAsync
client.Domain;    // CreateAsync, ListAsync, GetAsync, UpdateAsync, DeleteAsync, VerifyAsync
client.Contacts;  // CRUD + .Properties, .Groups, .Channels
client.Webhook;   // CRUD, PauseAsync/EnableAsync/DisableAsync, TriggerAsync, deliveries + Verify
client.Inbox;     // .Mailboxes, .Messages, .Threads
```

## Errors

| Kind | Type | When |
|------|------|------|
| Bad client args | `ReloopValidationException` | Invalid params — **no HTTP call** |
| HTTP / network | `ReloopApiException` | Non-2xx response or transport failure |
| Webhook HMAC | `WebhookSignatureException` | Local signature verification failed |

Idiomatic .NET exceptions — the equivalent of Node/Python Result objects and Go `(T, error)`.

## Examples

### Mail

```csharp
using static Reloop.Models.MailModels;

await client.Mail.SendAsync(new SendMailParams
{
    From = "hello@your-verified-domain.com",
    To = new[] { "a@example.com", "b@example.com" },
    Subject = "Hello",
    Html = "<p>Hi</p>",
});
```

### API key

```csharp
using static Reloop.Models.ApiKeyModels;

var created = await client.ApiKey.CreateAsync(new CreateApiKeyParams("Production"));
var keys = await client.ApiKey.ListAsync(new ApiKeyListParams { Page = 1, Limit = 20 });
```

### Domain

```csharp
using static Reloop.Models.DomainModels;

var domain = await client.Domain.CreateAsync(new CreateDomainParams("send.example.com")
{
    ClickTracking = true,
});
await client.Domain.VerifyAsync(domain!.Id!);
```

### Contacts

```csharp
using static Reloop.Models.ContactModels;

var contact = await client.Contacts.CreateAsync(new CreateContactParams
{
    Email = "user@example.com",
    FirstName = "Ada",
    Status = ContactStatus.Subscribed,
});

await client.Contacts.Properties.CreateAsync(new CreatePropertyParams
{
    Name = "company",
    Type = PropertyType.String,
});
```

### Webhook verification

```csharp
using Reloop.Services;
using static Reloop.Models.WebhookModels;

var webhookEvent = WebhookService.ConstructEvent(payloadBytes, signatureHeader, secret, tolerance: 300);
// or
var verified = client.Webhook.Verify(new VerifyWebhookParams
{
    Payload = payloadBytes,
    Headers = headers,
    Secret = secret,
});
```

### Inbox

```csharp
using static Reloop.Models.InboxModels;

var mailbox = await client.Inbox.Mailboxes.CreateAsync(new CreateMailboxParams
{
    DomainId = "dom_1",
    Email = "user@example.com",
});

await client.Inbox.Messages.SendAsync(new SendMessageParams
{
    MailboxId = mailbox!.Id!,
    To = "recipient@example.com",
    Subject = "Hello",
    Text = "Hi there",
});
```

## Breaking changes from 0.1.0

- **`ApiKeys` → `ApiKey`** on `ReloopClient`
- **Typed params/responses** instead of `Dictionary<string, object?>` for mail and contacts
- **Removed:** API key `PauseAsync`, domain `GetNameserversAsync` / `ForwardDnsAsync`
- **Added:** `Webhook`, `Inbox` (mailboxes / messages / threads), nested `Contacts.Properties`
- **Exceptions:** `ReloopException` replaced by `ReloopValidationException`, `ReloopApiException`, `WebhookSignatureException`

See [CHANGELOG.md](./CHANGELOG.md) for details.

## License

Licensed under the [Apache License 2.0](./LICENSE) with additional use restrictions from Reloop Labs (same as the [Reloop project](https://github.com/reloop-labs/reloop/blob/main/LICENSE)).

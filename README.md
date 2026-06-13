# Reloop .NET SDK

Official .NET client for the Reloop API.

## Install

```bash
dotnet add package Reloop
```

## Usage

```csharp
using Reloop;
using System.Collections.Generic;

var reloop = new ReloopClient("re_123456789");

await reloop.Contacts.CreateAsync(new Dictionary<string, object?>
{
    ["email"] = "john.doe@example.com",
    ["first_name"] = "John",
    ["last_name"] = "Doe",
    ["unsubscribed"] = false,
});
```

## API Keys

```csharp
using Reloop;
using Reloop.Models;

var reloop = new ReloopClient("rl_123456789");

await reloop.ApiKeys.CreateAsync(new CreateApiKeyParams(
    Name: "Production key",
    Enabled: true,
    RateLimitEnabled: true
));

await reloop.ApiKeys.RotateAsync("key_123456789");
await reloop.ApiKeys.PauseAsync("key_123456789");
await reloop.ApiKeys.EnableAsync("key_123456789");
```

## Domains

```csharp
using Reloop;
using Reloop.Models;

var reloop = new ReloopClient("rl_123456789");

var domain = await reloop.Domain.CreateAsync(new CreateDomainParams(
    Domain: "send.example.com",
    CustomReturnPath: "inbound",
    ClickTracking: true,
    OpenTracking: true,
    Tls: "opportunistic",
    SendingEmail: true,
    ReceivingEmail: true
));

var domains = await reloop.Domain.ListAsync(new ListDomainsParams
{
    Page = 1,
    Limit = 10,
    Status = "active",
});

var one = await reloop.Domain.GetAsync("domain_123456789");

await reloop.Domain.UpdateAsync(
    "domain_123456789",
    new UpdateDomainParams(ClickTracking: false, SendingEmail: true));

var status = await reloop.Domain.VerifyAsync("domain_123456789");

await reloop.Domain.ForwardDnsAsync(
    "domain_123456789",
    new ForwardDnsParams("admin@example.com"));

var nameservers = await reloop.Domain.GetNameserversAsync("domain_123456789");

await reloop.Domain.DeleteAsync("domain_123456789");
```

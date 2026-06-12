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

await reloop.ApiKeys.CreateAsync(new CreateApiKeyParams
{
    Name = "Production key",
    Enabled = true,
    RateLimitEnabled = true,
});
```

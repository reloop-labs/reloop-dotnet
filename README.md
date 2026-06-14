# Reloop .NET SDK

## Before you send

You need two things:

1. **API key** — create one in your Reloop account
2. **Verified domain** — add and verify a sending domain; use it in the `from` address

For setup details and the full API reference, see [reloop.sh/docs](https://reloop.sh/docs).

## Send email

```bash
dotnet add package Reloop
```

```csharp
using Reloop;
using System.Collections.Generic;

var reloop = new ReloopClient("rl_your_api_key_here");

var result = await reloop.Mail.SendAsync(new Dictionary<string, object?>
{
    ["from"] = "Reloop <hello@your-verified-domain.com>",
    ["to"] = "user@example.com",
    ["subject"] = "Welcome to Reloop",
    ["html"] = "<p>Thanks for signing up.</p>",
    ["text"] = "Thanks for signing up.",
});

Console.WriteLine($"{result?.MessageId} {result?.Id}");
```

More examples and optional fields: [reloop.sh/docs](https://reloop.sh/docs)

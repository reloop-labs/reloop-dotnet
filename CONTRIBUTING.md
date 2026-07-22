# Contributing to the Reloop .NET SDK

NuGet package: **`Reloop`** (version aligned with Node/Python/Go/Java at **2.0.0**).

**License:** [Apache License 2.0](./LICENSE) with additional use restrictions from Reloop Labs.

**API reference:** [reloop.sh/docs](https://reloop.sh/docs)

Port new endpoints from the [Node.js SDK](https://github.com/reloop-labs/reloop-node) — Node wins on paths, bodies, and validation messages.

---

## Development setup

```bash
git clone git@github.com:reloop-labs/reloop-dotnet.git
cd reloop-dotnet
dotnet test tests/Reloop.Tests/Reloop.Tests.csproj
```

Requires **.NET 8 SDK** for tests; the library targets **netstandard2.0** (`System.Text.Json`).

---

## Project layout

```
ReloopClient.cs
Version.cs
Exceptions/            # ReloopValidationException, ReloopApiException, ApiErrorBody, WebhookSignatureException
Validation/              # Validators
Models/                  # MailModels, ApiKeyModels, DomainModels, ContactModels, WebhookModels, InboxModels
Services/                # Mail, ApiKey, Domain, Contacts*, Webhook*, Inbox*
tests/Reloop.Tests/      # Route + validation + surface-lock tests, MockHttpMessageHandler
Reloop.csproj            # Version, PackageVersion, PackageLicenseExpression
```

---

## Conventions

| Topic | Rule |
|-------|------|
| Errors | `ReloopValidationException` (no HTTP) / `ReloopApiException` (HTTP/network) / `WebhookSignatureException` (local HMAC) |
| Wire JSON | Match Node (`[JsonPropertyName]` for snake_case vs camelCase) |
| Validation | Fail before HTTP via `Validators`; tests assert **0** requests on validation failures |
| Async API | Public methods end with `Async`; inject `HttpClient` in ctor for tests |
| Tests | `MockHttpMessageHandler`: method, path, `x-api-key`, body; surface-lock method sets |
| Endpoints | Do not invent — copy from Node `paths.ts` / Java services |
| Version | Same semver as Node/Python/Go/Java (`Version.VERSION` + `Reloop.csproj`) |

---

## Pull request checklist

- [ ] `dotnet test tests/Reloop.Tests/Reloop.Tests.csproj` passes
- [ ] New ops have happy path + API error + validation (0 HTTP) + surface test where applicable
- [ ] `Reloop.csproj` / `Version.cs` updated only when releasing

---

## Releasing

1. Set `<Version>` and `<PackageVersion>` in `Reloop.csproj` and `Version.VERSION` to the same value (match other SDKs)
2. Tag and publish via existing GitHub Actions workflows

```bash
git commit -am "chore: release v2.0.0"
git push origin main
git tag v2.0.0
git push origin v2.0.0
```

[`.github/workflows/release.yml`](./.github/workflows/release.yml) uploads source zip + `.nupkg` files.

Publish: [`.github/workflows/publish.yml`](./.github/workflows/publish.yml) (NuGet).

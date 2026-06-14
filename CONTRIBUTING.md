# Contributing to the Reloop .NET SDK

NuGet package: **`Reloop`**.

**License:** [Apache License 2.0](./LICENSE) with additional use restrictions from Reloop Labs.

**API reference:** [reloop.sh/docs](https://reloop.sh/docs)

Port new endpoints from the [Node.js SDK](https://github.com/reloop-labs/reloop-node) reference.

---

## Development setup

```bash
git clone git@github.com:reloop-labs/reloop-dotnet.git
cd reloop-dotnet
dotnet test tests/Reloop.Tests/Reloop.Tests.csproj
```

Requires **.NET 8 SDK** (CI); targets **netstandard2.0**.

---

## Project layout

```
ReloopClient.cs
Services/               # MailService, DomainService, …
Models/Models.cs        # Records with JsonPropertyName
tests/Reloop.Tests/     # Route tests + MockHttpMessageHandler
Reloop.csproj           # Version, PackageVersion, PackageLicenseExpression
```

---

## Conventions

| Topic | Rule |
|-------|------|
| Domain | Typed records; snake_case JSON names |
| Mail send | `Dictionary<string, object?>` with snake_case keys |
| Contacts | camelCase via request helpers |
| Tests | Inject `HttpClient` with `MockHttpMessageHandler` |

---

## Pull request checklist

- [ ] `dotnet test tests/Reloop.Tests/Reloop.Tests.csproj` passes
- [ ] `<Version>` in `Reloop.csproj` bumped only for releases

---

## Releasing

Version: **`Reloop.csproj`** → `<Version>` and `<PackageVersion>`.

```bash
git commit -am "chore: release v1.9.0"
git push origin main
git tag v1.9.0
git push origin v1.9.0
```

[`.github/workflows/release.yml`](./.github/workflows/release.yml) uploads source zip + `.nupkg` files.

Publish: [`.github/workflows/publish.yml`](./.github/workflows/publish.yml) (NuGet).

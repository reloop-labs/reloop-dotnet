# Changelog

## 2.0.0

Breaking rewrite for Node SDK parity (version aligned with Node/Python/Go/Java `2.0.0`):

- Typed params/responses (`SendMailParams`, `ContactModels`, etc.) instead of untyped dictionaries for mail/contacts
- `ReloopValidationException` for invalid client input (no HTTP); `ReloopApiException` for HTTP/network; `WebhookSignatureException` for local HMAC verify
- Facade: `client.ApiKey` (was `ApiKeys`), `Mail`, `Domain`, `Contacts`, `Webhook`, `Inbox`
- Typed Contacts with nested `Properties`, `Groups`, `Channels`
- Full Webhook CRUD + `PauseAsync`/`EnableAsync`/`DisableAsync`/`TriggerAsync`/deliveries + local HMAC `WebhookVerify`
- Full Inbox: `Mailboxes`, `Messages`, `Threads`
- Removed: API key `PauseAsync`, domain `GetNameserversAsync` / `ForwardDnsAsync`
- Paths aligned with Node (`/api/mail/v1/send`, `/api/api-key/v1/`, `/api/domain/v1/`, …)
- xUnit tests with `MockHttpMessageHandler` and surface-lock checks

## 0.1.0

Initial thin client for mail, domain, API keys, and dictionary-based contacts.

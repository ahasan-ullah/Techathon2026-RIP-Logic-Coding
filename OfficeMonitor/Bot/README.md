# OfficeMonitor — Discord Bot

A Discord bot that answers questions about the office's electrical devices, reading from the same [OfficeMonitor API](../README.md) the web dashboard uses — one backend, one source of truth, two clients.

No LLM involved: replies are built from the live API response and rendered as Discord embeds. This keeps every number guaranteed-accurate and removes an external dependency (and its cost/latency/failure surface) from the demo path.

---

## Commands

| Command | What it does |
|---|---|
| `!status` | ON/OFF summary for every room |
| `!room <code>` | Full device breakdown for one room (`drawing`, `work1`, `work2`) |
| `!usage` | Current total power draw, today's estimated kWh, per-room breakdown |

**Bonus — proactive alerts:** the bot also opens its own SignalR connection to the API's hub and posts an embed to a configured channel the moment a new alert fires (after-hours device left on, or a room running continuously for 2+ hours), without anyone asking.

---

## Discord Developer Portal setup

1. Go to [discord.com/developers/applications](https://discord.com/developers/applications) → **New Application** → name it (e.g. "Office Monitor Bot").
2. **Bot** tab (left sidebar) → it creates a bot user automatically → **Reset Token** → copy it now, Discord only shows it once.
3. Same **Bot** tab → **Privileged Gateway Intents** → enable **MESSAGE CONTENT INTENT**. Required — without it, Discord won't deliver command text to the bot at all.
4. **OAuth2** → **URL Generator**:
   - Scopes: `bot`
   - Bot Permissions: `Send Messages`, `Read Message History`, `View Channels`, `Embed Links`
   - Open the generated URL, pick your test server, authorize.
5. In Discord: **User Settings → Advanced → Developer Mode** (on), then right-click the channel you want alerts posted to → **Copy Channel ID**.

---

## Configuration

Non-secret config lives in `appsettings.json` (safe to commit):

```json
{
  "Discord": { "Prefix": "!" },
  "Api": {
    "BaseUrl": "https://localhost:7081/api/",
    "HubUrl": "https://localhost:7081/hub/devices"
  }
}
```

Secrets (bot token, alert channel ID) go through **.NET user secrets** — never committed, never in `appsettings.json`:

```bash
cd Bot
dotnet user-secrets init
dotnet user-secrets set "Discord:Token" "PASTE_YOUR_BOT_TOKEN"
dotnet user-secrets set "Discord:AlertChannelId" "PASTE_YOUR_CHANNEL_ID"
```

In Visual Studio: right-click the `Bot` project → **Manage User Secrets** → paste the same two keys into the editor that opens.

Match `Api:BaseUrl` / `Api:HubUrl` to wherever `dotnet run --project API` actually listens (check its console output).

---

## Running

The bot is a client of the API, so the API must already be running.

```bash
cd API && dotnet run     # terminal 1
cd Bot && dotnet run     # terminal 2
```

In Visual Studio: **Solution → Set Startup Projects… → Multiple startup projects**, set both `API` and `Bot` to *Start* (API above Bot in the list), then F5.

Watch the bot's console for `Logged in as <bot-name>` — if that never appears, the token isn't being picked up (check `AddUserSecrets<Program>()` is wired in `Program.cs` and that you set secrets on the `Bot` project, not `API`).

To see the proactive alert push: leave a device on outside 09:00–17:00 (or wait for the backend's simulator to do it), then either wait for the next simulator tick or hit "Run alert check now" on the web dashboard — the bot should post to the alert channel within seconds.

---

## Project structure

```
Bot/
├── Program.cs                   Host + DI wiring
├── appsettings.json               non-secret config (committed)
├── Configuration/
│   ├── DiscordSettings.cs           token, prefix, alert channel id
│   └── ApiSettings.cs                backend base URL + hub URL
├── Models/
│   └── OfficeDtos.cs                 mirrors the API's JSON shapes
├── Services/
│   ├── OfficeApiClient.cs             typed HTTP client for the backend
│   ├── EmbedFactory.cs                 builds all Discord embeds from DTOs
│   ├── CommandHandlingService.cs        routes prefixed messages to command modules
│   ├── DiscordBotHostedService.cs        logs in, starts the gateway connection
│   └── AlertWatcherService.cs             SignalR listener → proactive alert posts
└── Modules/
    └── OfficeCommandsModule.cs        !status / !room / !usage command handlers
```

Deliberately has no project reference to `BLL`/`DAL` — architecturally the bot is just another client hitting the backend API over HTTP/SignalR, exactly like the React dashboard. `Models/OfficeDtos.cs` defines its own small records mirroring the API's JSON rather than sharing `BLL.DTOs` directly.

---

## Known limitation

`AlertWatcherService` tracks already-posted alert IDs in an in-memory `HashSet`. A bot restart forgets that history, so any alert still active at startup gets posted again. Fine for a demo; a production version would persist the seen-IDs (or just the max seen ID) somewhere durable.

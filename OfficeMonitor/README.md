# OfficeMonitor — Backend

Real-time electrical device monitoring for a 3-room office (18... well, actually 15 — see [Device Inventory](#device-inventory)). Tracks fans and lights per room, computes live power draw, raises operational alerts, and streams every change out over SignalR so a dashboard and a Discord bot can share one source of truth.

> This README covers the **backend** (API + business logic + data layer). The web dashboard and Discord bot are being built separately and will plug into the endpoints and SignalR events documented below.

---

## Contents

- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Device Inventory](#device-inventory)
- [API Reference](#api-reference)
- [Real-Time Events (SignalR)](#real-time-events-signalr)
- [Alert Rules](#alert-rules)
- [Background Simulator](#background-simulator)
- [Configuration](#configuration)

---

## Architecture

Three-layer solution, each project depending only on the one below it:

```
┌─────────────────────────────────────────────────────────────┐
│  API                                                         │
│  Controllers (Devices, Rooms, Usage, Alerts)                  │
│  DeviceHub (SignalR)                                          │
│  DeviceSimulatorHostedService (background loop)                │
└───────────────────────────┬─────────────────────────────────┘
                            │ depends on
┌───────────────────────────▼─────────────────────────────────┐
│  BLL  (business logic)                                        │
│  DeviceService · PowerCalculationService · AlertService        │
│  DTOs, Interfaces                                              │
└───────────────────────────┬─────────────────────────────────┘
                            │ depends on
┌───────────────────────────▼─────────────────────────────────┐
│  DAL  (data access)                                            │
│  UnitOfWork + Repositories · EF Core DbContext · Migrations     │
│  Entities: Room, Device, DeviceState, DeviceStateHistory, Alert │
└─────────────────────────────────────────────────────────────┘
```

**Data flow for a device toggle:**

```
Client (dashboard/bot)
   │  POST /api/devices/{id}/toggle
   ▼
DevicesController → DeviceService → UnitOfWork → SQL Server
   │
   ▼
DeviceHub.Clients.All.SendAsync("DeviceStateChanged", updatedDevice)
   │
   ▼
Every connected client updates instantly — no polling, no page refresh
```

The `DeviceSimulatorHostedService` runs the same path on a timer (toggling random devices, evaluating alerts, and pushing usage) so the dashboard always has live movement even with nobody clicking anything.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8 Web API |
| Real-time | SignalR |
| Data access | Entity Framework Core 8 (SQL Server provider) |
| Docs | Swagger / Swashbuckle |
| Background job | `BackgroundService` (built-in .NET hosted service) |

No AutoMapper — DTO mapping is done with small static helper methods in each service, kept intentionally simple.

---

## Project Structure

```
OfficeMonitor/
├── API/                        ASP.NET Core Web API (entry point)
│   ├── Controllers/             Devices, Rooms, Usage, Alerts
│   ├── Hubs/                     DeviceHub (SignalR)
│   ├── BackgroundServices/       DeviceSimulatorHostedService
│   ├── Program.cs                DI + middleware pipeline
│   └── appsettings.json
├── BLL/                         Business logic (class library)
│   ├── DTOs/
│   ├── Interfaces/                IDeviceService, IAlertService, IPowerCalculationService
│   └── Services/                  DeviceService, AlertService, PowerCalculationService
└── DAL/                         Data access (class library)
    ├── Entities/                  Room, Device, DeviceState, DeviceStateHistory, Alert
    ├── Enums/                     DeviceType, RoomType, AlertType
    ├── Interfaces/ & Repositories/  Generic repo + per-entity repos, UnitOfWork
    ├── Data/OfficeMonitorContext.cs
    └── Migrations/
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A SQL Server instance reachable from your machine (LocalDB, SQL Express, full SQL Server, or a Docker container all work)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef` (if not already installed)

### 1. Clone and restore

```bash
git clone <your-repo-url>
cd OfficeMonitor
dotnet restore
```

### 2. Point it at your database

Edit `API/appsettings.json` → `ConnectionStrings:DefaultConnection` to match your SQL Server instance, e.g.:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OfficeMonitorDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Apply migrations

```bash
dotnet ef database update --project DAL --startup-project API
```

This creates the schema and seeds 3 rooms with their devices (see [Device Inventory](#device-inventory)) via the baked-in `initialDbCreated` migration.

### 4. Run the API

```bash
dotnet run --project API
```

- Swagger UI: `https://localhost:7081/swagger` (or `http://localhost:5112/swagger`)
- SignalR hub: `/hub/devices`

The background simulator starts automatically and begins flipping devices, evaluating alerts, and broadcasting updates every 15 seconds (configurable — see [Configuration](#configuration)).

---

## Device Inventory

The office has 3 rooms, each with **2 fans + 3 lights = 5 devices per room → 15 devices total.**

| Room | Code | Type | Devices |
|---|---|---|---|
| Drawing Room | `drawing` | Waiting area | 2 fans (60W), 3 lights (15W) |
| Work Room 1 | `work1` | Work area | 2 fans (60W), 3 lights (15W) |
| Work Room 2 | `work2` | Work area | 2 fans (60W), 3 lights (15W) |

> The original brief's device-summary graphic says "18 devices total" but its own per-room math (2 fans + 3 lights × 3 rooms) only adds up to 15 — that's an arithmetic slip in the source doc, not a gap in this implementation.

---

## API Reference

All responses are JSON. Base URL: `https://localhost:7081/api`.

| Method | Route | Description |
|---|---|---|
| `GET` | `/devices` | All 15 devices with current on/off state |
| `POST` | `/devices/{id}/toggle` | Set a device's on/off state — body: `{ "isOn": true }` |
| `GET` | `/rooms` | Status of all rooms (devices, on-count, current watts) |
| `GET` | `/rooms/{code}` | Status of one room by code (`drawing`, `work1`, `work2`) |
| `GET` | `/usage` | Current total watts + per-room breakdown + today's estimated kWh |
| `GET` | `/alerts/active` | Currently unresolved alerts |
| `POST` | `/alerts/evaluate` | Manually run the alert engine now (useful for demos/testing) and broadcast the result |

**Toggle example:**

```http
POST /api/devices/3/toggle
Content-Type: application/json

{ "isOn": true }
```

```json
{
  "id": 3,
  "name": "Light 1",
  "type": "Light",
  "roomId": 1,
  "roomName": "Drawing Room",
  "isOn": true,
  "ratedPowerWatts": 15,
  "lastChangedAt": "2026-07-04T10:15:32Z"
}
```

---

## Real-Time Events (SignalR)

Connect to `/hub/devices`. The API never expects clients to invoke hub methods — it's push-only. Three events are broadcast to all connected clients:

| Event | Payload | Fired when |
|---|---|---|
| `DeviceStateChanged` | `DeviceDto` | A device is toggled — manually via the API, or by the simulator |
| `AlertsUpdated` | `AlertDto[]` | The alert engine runs (simulator tick or `POST /alerts/evaluate`) |
| `UsageUpdated` | `UsageDto` | After every simulator tick |

A dashboard just needs to open one connection and merge these three events into its local state — no polling required.

---

## Alert Rules

Implemented in `AlertService`, evaluated every simulator tick (or on demand via `POST /alerts/evaluate`):

- **AfterHours** — any device still on outside 09:00–17:00 (office hours, timezone-configurable) raises a per-device alert. Auto-resolves once back inside office hours.
- **ContinuousUsage** — a room where *every* device has been on continuously for 2+ hours raises a single room-level alert. Auto-resolves as soon as the room no longer qualifies (a device turns off, or the streak resets).

Duplicate alerts are suppressed — an already-active alert for the same device/room + type is never raised twice.

---

## Background Simulator

`DeviceSimulatorHostedService` is what makes the "live" in real-time: since there's no physical hardware, it stands in for it. Every tick it:

1. Randomly toggles 1–2 devices (simulating people arriving/leaving, turning things on/off)
2. Runs the alert engine
3. Broadcasts `DeviceStateChanged`, `AlertsUpdated`, and `UsageUpdated` over SignalR

This is the entire "dummy data" story — no separate script or JSON file needed; state lives in the database and evolves on its own once the API is running.

---

## Configuration

`API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  },
  "OfficeSettings": {
    "TimeZoneId": ""
  },
  "SimulatorSettings": {
    "Enabled": true,
    "IntervalSeconds": 15
  }
}
```

| Key | Purpose |
|---|---|
| `OfficeSettings:TimeZoneId` | IANA/Windows timezone ID used for the 09:00–17:00 office-hours check (e.g. `"Asia/Dhaka"`). Empty → falls back to the server's local timezone. |
| `SimulatorSettings:Enabled` | Turn the background simulator off (e.g. for automated tests) without removing it. |
| `SimulatorSettings:IntervalSeconds` | How often the simulator ticks. Minimum enforced at 5s. |

---

## Roadmap

- [ ] Web dashboard (live device panel, power meter, alerts panel, office floor plan)
- [ ] Discord bot (`!status`, `!room <name>`, `!usage`, proactive alert posts)
- [ ] System diagram + hardware/circuit schematic

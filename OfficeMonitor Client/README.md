# OfficeMonitor — Web Dashboard

The real-time web dashboard for OfficeMonitor. Shows live on/off state for every fan and light across three office rooms, the office's total power draw, per-room breakdowns, and active alerts — all pushed over SignalR with no page refresh. Talks to the [OfficeMonitor API](../OfficeMonitor/README.md), the same backend the Discord bot reads from.

---

## Contents

- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Features](#features)
- [Data Flow](#data-flow)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | React 19 + Vite |
| Routing | React Router 7 |
| Styling | Tailwind CSS 4 + daisyUI (custom `officemonitor` dark navy theme) |
| HTTP | Axios |
| Real-time | `@microsoft/signalr` client |

---

## Project Structure

```
src/
├── api/                 Axios client + SignalR connection + one file per resource
│   ├── client.js          Shared axios instance (baseURL from env)
│   ├── signalr.js         Hub connection factory
│   ├── devices.js, rooms.js, alerts.js, usage.js
├── components/
│   ├── layout/            Header (branding, clock, office-hours/live pills)
│   ├── common/             LiveStatusPill, OfficeHoursPill, ControlsPanel
│   ├── power/               PowerRingMeter
│   ├── floorplan/           FloorPlan (SVG office layout, click-to-toggle fixtures)
│   ├── rooms/                RoomCard (per-room device toggle buttons)
│   └── alerts/                AlertsPanel, AlertItem
├── hooks/
│   └── useOfficeData.js    Single hook: initial fetch + SignalR subscriptions + toggle/evaluate actions
├── pages/                  DashboardPage, RoomDetailPage, NotFoundPage
├── routes/                 AppRoutes
└── utils/                  formatTime, officeHours (client-side 09:00–17:00 check for the header pill)
```

`useOfficeData` is the single source of truth on the client: it loads devices/rooms/alerts/usage once on mount, opens one SignalR connection, and merges `DeviceStateChanged` / `AlertsUpdated` / `UsageUpdated` events into local state. Every page/component reads from it — there's no separate client-side polling.

---

## Getting Started

### Prerequisites

- Node 18+
- The [OfficeMonitor API](../OfficeMonitor/README.md) running locally (this app doesn't work standalone — it needs the backend for data and the SignalR hub for live updates)

### 1. Install

```bash
npm install
```

### 2. Configure the API URL

Copy `.env.local` (or create one) with the backend's address:

```
VITE_API_BASE_URL=https://localhost:7081/api
VITE_HUB_URL=https://localhost:7081/hub/devices
```

Match these to wherever `dotnet run --project API` is actually listening (check the console output — Swagger prints the real port).

### 3. Run

```bash
npm run dev
```

Open the printed local URL (default `http://localhost:5173`). Other scripts:

| Script | Purpose |
|---|---|
| `npm run build` | Production build to `dist/` |
| `npm run preview` | Serve the production build locally |
| `npm run lint` | ESLint |

---

## Configuration

| Env var | Purpose |
|---|---|
| `VITE_API_BASE_URL` | Base URL for REST calls (devices, rooms, alerts, usage) |
| `VITE_HUB_URL` | SignalR hub URL for live device/alert/usage events |

No other configuration — office hours (09:00–17:00) are hardcoded client-side in `utils/officeHours.js` to match the backend's default; if you change `OfficeSettings:TimeZoneId` on the API, update this too so the header pill agrees with the backend's alert evaluation.

---

## Features

- **Live device status** — floor plan (click a fixture to toggle) and per-room cards, both driven by the same state
- **Live power meter** — ring gauge showing total watts drawn vs. max possible, plus today's estimated kWh and devices-on count
- **Active alerts panel** — after-hours and continuous-usage alerts, auto-updating as the backend's alert engine resolves/raises them
- **Manual controls** — toggle any device directly; a "Run alert check now" button forces an immediate alert re-evaluation instead of waiting for the next simulator tick
- **Room detail page** — `/rooms/:code` for a focused view of one room

---

## Data Flow

```
useOfficeData (mount)
   ├─ GET /devices, /rooms, /alerts/active, /usage   → initial state
   └─ SignalR: connect to /hub/devices
         ├─ DeviceStateChanged  → merge into devices[]
         ├─ AlertsUpdated       → replace alerts[]
         └─ UsageUpdated        → replace usage

User clicks a fixture / device button
   → POST /devices/{id}/toggle   (optimistic update, rolled back on failure)
   → backend broadcasts DeviceStateChanged to all clients (including this one)
```

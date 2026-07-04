import * as signalR from '@microsoft/signalr';

export function createHubConnection() {
  return new signalR.HubConnectionBuilder()
    .withUrl(import.meta.env.VITE_HUB_URL)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();
}
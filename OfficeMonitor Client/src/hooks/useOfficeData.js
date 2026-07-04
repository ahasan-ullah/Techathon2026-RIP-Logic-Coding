import { useEffect, useMemo, useRef, useState, useCallback } from "react";
import { getDevices, toggleDevice as apiToggleDevice } from "../api/devices";
import { getRooms } from "../api/rooms";
import {
  getActiveAlerts,
  evaluateAlerts as apiEvaluateAlerts,
} from "../api/alerts";
import { getUsage } from "../api/usage";
import { createHubConnection } from "../api/signalr";

export function useOfficeData() {
  const [devices, setDevices] = useState([]);
  const [roomMeta, setRoomMeta] = useState([]); // { roomId, roomName, code }
  const [alerts, setAlerts] = useState([]);
  const [usage, setUsage] = useState(null);
  const [isConnected, setIsConnected] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const connectionRef = useRef(null);


  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        const [deviceList, roomList, alertList, usageData] = await Promise.all([
          getDevices(),
          getRooms(),
          getActiveAlerts(),
          getUsage(),
        ]);
        if (cancelled) return;
        setDevices(deviceList);
        setRoomMeta(
          roomList.map((r) => ({
            roomId: r.roomId,
            roomName: r.roomName,
            code: r.code,
          })),
        );
        setAlerts(alertList);
        setUsage(usageData);
      } catch (err) {
        if (!cancelled) setError(err);
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, []);

// live updates over SignalR
  useEffect(() => {
    const connection = createHubConnection();
    connectionRef.current = connection;
    let cancelled = false;

    connection.on('DeviceStateChanged', (updatedDevice) => {
      setDevices(prev => prev.map(d => (d.id === updatedDevice.id ? updatedDevice : d)));
    });
    connection.on('AlertsUpdated', (updatedAlerts) => setAlerts(updatedAlerts));
    connection.on('UsageUpdated', (updatedUsage) => setUsage(updatedUsage));

    connection.onreconnecting(() => setIsConnected(false));
    connection.onreconnected(() => setIsConnected(true));
    connection.onclose(() => setIsConnected(false));

    connection.start()
      .then(() => {
        if (cancelled) {
          connection.stop();
        } else {
          setIsConnected(true);
        }
      })
      .catch((err) => {
        if (!cancelled) {
          console.error('SignalR connection failed:', err);
          setIsConnected(false);
        }
      });

    return () => {
      cancelled = true;
      if (connection.state === 'Connected') connection.stop();
    };
  }, []);

  const toggleDevice = useCallback(async (id, nextIsOn) => {
    setDevices((prev) =>
      prev.map((d) => (d.id === id ? { ...d, isOn: nextIsOn } : d)),
    );
    try {
      await apiToggleDevice(id, nextIsOn);
    } catch (err) {
      setDevices((prev) =>
        prev.map((d) => (d.id === id ? { ...d, isOn: !nextIsOn } : d)),
      ); // roll back
      setError(err);
    }
  }, []);

  const runEvaluateAlerts = useCallback(async () => {
    const updated = await apiEvaluateAlerts();
    setAlerts(updated);
  }, []);

  const rooms = useMemo(
    () =>
      roomMeta.map((meta) => {
        const roomDevices = devices.filter((d) => d.roomId === meta.roomId);
        const onCount = roomDevices.filter((d) => d.isOn).length;
        const watts = roomDevices
          .filter((d) => d.isOn)
          .reduce((sum, d) => sum + d.ratedPowerWatts, 0);
        return { ...meta, devices: roomDevices, onCount, watts };
      }),
    [devices, roomMeta],
  );

  const totalWatts = useMemo(
    () =>
      devices
        .filter((d) => d.isOn)
        .reduce((sum, d) => sum + d.ratedPowerWatts, 0),
    [devices],
  );

  const maxWatts = useMemo(
    () => devices.reduce((sum, d) => sum + d.ratedPowerWatts, 0) || 1,
    [devices],
  );

  return {
    devices,
    rooms,
    alerts,
    usage,
    totalWatts,
    maxWatts,
    isConnected,
    isLoading,
    error,
    toggleDevice,
    runEvaluateAlerts,
  };
}

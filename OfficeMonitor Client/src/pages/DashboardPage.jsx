import { useOfficeData } from '../hooks/useOfficeData';
import Header from '../components/layout/Header';
import PowerRingMeter from '../components/power/PowerRingMeter';
import FloorPlan from '../components/floorplan/FloorPlan';
import RoomCard from '../components/rooms/RoomCard';
import AlertsPanel from '../components/alerts/AlertsPanel';
import ControlsPanel from '../components/common/ControlsPanel';

export default function DashboardPage() {
  const { rooms, devices, alerts, usage, totalWatts, maxWatts, isConnected, isLoading, error, toggleDevice, runEvaluateAlerts } = useOfficeData();

  if (isLoading) {
    return <div className="flex min-h-screen items-center justify-center text-base-content/45">Connecting to Office Monitor…</div>;
  }

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center px-6 text-center">
        <div>
          <div className="font-display text-lg font-semibold text-error">Couldn't reach the backend</div>
          <p className="mt-2 text-sm text-base-content/60">
            Check that the API is running and CORS allows this origin. ({error.message})
          </p>
        </div>
      </div>
    );
  }

  const onCount = devices.filter(d => d.isOn).length;

  return (
    <div className="mx-auto max-w-[1240px] px-5 py-7">
      <Header isConnected={isConnected} />

      <div className="mb-4 grid gap-4 lg:grid-cols-[280px_1fr]">
        <div className="card bg-base-100 border border-neutral">
          <div className="card-body p-5">
            <h2 className="mb-4 font-display text-sm font-semibold">Total Draw</h2>
            <PowerRingMeter
              totalWatts={totalWatts}
              maxWatts={maxWatts}
              todayKwh={usage?.todayEstimatedKwh ?? 0}
              onCount={onCount}
              totalCount={devices.length}
            />
          </div>
        </div>

        <div className="card bg-base-100 border border-neutral">
          <div className="card-body p-5">
            <h2 className="mb-4 font-display text-sm font-semibold">
              Floor Plan <span className="font-data text-[10px] font-normal text-base-content/45">click a fixture to toggle</span>
            </h2>
            <FloorPlan rooms={rooms} onToggleDevice={toggleDevice} />
          </div>
        </div>
      </div>

      <div className="mb-4 grid gap-4 md:grid-cols-3">
        {rooms.map(room => (
          <RoomCard key={room.roomId} room={room} onToggleDevice={toggleDevice} />
        ))}
      </div>

      <div className="grid gap-4 lg:grid-cols-[1.4fr_1fr]">
        <AlertsPanel alerts={alerts} />
        <ControlsPanel onEvaluate={runEvaluateAlerts} />
      </div>
    </div>
  );
}
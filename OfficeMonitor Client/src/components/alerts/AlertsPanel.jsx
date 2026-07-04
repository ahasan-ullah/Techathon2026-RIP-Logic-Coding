import AlertItem from './AlertItem';

export default function AlertsPanel({ alerts }) {
  return (
    <div className="card bg-base-100 border border-neutral">
      <div className="card-body p-5">
        <h2 className="mb-1 flex items-center justify-between font-display text-sm font-semibold">
          Active Alerts
          <span className="font-data text-[10px] uppercase tracking-wide text-base-content/45">{alerts.length} active</span>
        </h2>
        <div className="flex max-h-[250px] flex-col gap-2 overflow-y-auto pt-2">
          {alerts.length === 0 ? (
            <div className="flex flex-col items-center gap-2 py-8 text-center text-base-content/45">
              <div className="flex h-7 w-7 items-center justify-center rounded-full border border-secondary/40 text-secondary">✓</div>
              <div>No active alerts — everything looks normal.</div>
            </div>
          ) : (
            alerts.map(a => <AlertItem key={a.id} alert={a} />)
          )}
        </div>
      </div>
    </div>
  );
}
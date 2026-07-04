export default function LiveStatusPill({ isConnected }) {
  return (
    <span className={`badge gap-1.5 font-data text-[11px] ${isConnected ? 'badge-secondary badge-soft' : 'badge-neutral badge-soft'}`}>
      <span className={`h-1.5 w-1.5 rounded-full ${isConnected ? 'bg-secondary animate-pulse' : 'bg-neutral'}`} />
      {isConnected ? 'LIVE' : 'OFFLINE'}
    </span>
  );
}
import { formatShortTime } from '../../utils/formatTime';

export default function AlertItem({ alert }) {
  const isContinuous = alert.type === 'ContinuousUsage';
  return (
    <div className={`alert items-start gap-2.5 py-2.5 ${isContinuous ? 'alert-warning alert-soft' : 'alert-error alert-soft'}`}>
      <div className="flex-1">
        <div className="text-[12.5px] leading-snug">{alert.message}</div>
        <div className="mt-1 font-data text-[10px] opacity-60">
          {alert.type.toUpperCase()} · triggered {formatShortTime(new Date(alert.triggeredAt))}
        </div>
      </div>
    </div>
  );
}
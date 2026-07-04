import { useEffect, useState } from 'react';
import LiveStatusPill from '../common/LiveStatusPill';
import OfficeHoursPill from '../common/OfficeHoursPill';
import { formatClock } from '../../utils/formatTime';

export default function Header({ isConnected }) {
  const [now, setNow] = useState(new Date());

  useEffect(() => {
    const id = setInterval(() => setNow(new Date()), 1000);
    return () => clearInterval(id);
  }, []);

  return (
    <header className="flex flex-wrap items-end justify-between gap-4 border-b border-neutral pb-5 mb-6">
      <div className="flex items-center gap-3">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary shadow-[0_0_18px_rgba(242,169,59,0.5)]">
          <svg viewBox="0 0 24 24" className="h-5 w-5" fill="none" stroke="#14171d" strokeWidth="2.2" strokeLinecap="round" strokeLinejoin="round">
            <path d="M13 2 4 14h6l-1 8 9-12h-6l1-8z" />
          </svg>
        </div>
        <div>
          <div className="font-data text-[11px] uppercase tracking-widest text-base-content/45">Electrical Monitoring System</div>
          <h1 className="font-display text-xl font-semibold -tracking-tight">Office Monitor</h1>
        </div>
      </div>
      <div className="flex items-center gap-4">
        <OfficeHoursPill now={now} />
        <LiveStatusPill isConnected={isConnected} />
        <div className="text-right">
          <div className="font-data text-[10px] uppercase tracking-widest text-base-content/45">Clock</div>
          <div className="font-data text-base">{formatClock(now)}</div>
        </div>
      </div>
    </header>
  );
}
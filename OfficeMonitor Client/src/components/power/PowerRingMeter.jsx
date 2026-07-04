export default function PowerRingMeter({ totalWatts, maxWatts, todayKwh, onCount, totalCount }) {
  const radius = 50;
  const circumference = 2 * Math.PI * radius;
  const pct = Math.min(1, totalWatts / maxWatts);
  const offset = circumference * (1 - pct);

  return (
    <div className="flex flex-col items-center gap-1">
      <div className="relative h-[180px] w-[180px]">
        <svg viewBox="0 0 120 120" className="h-full w-full -rotate-90">
          <defs>
            <linearGradient id="ringGrad" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stopColor="#f2a93b" />
              <stop offset="100%" stopColor="#57c7b8" />
            </linearGradient>
          </defs>
          <circle cx="60" cy="60" r={radius} fill="none" stroke="#2a303b" strokeWidth="10" />
          <circle
            cx="60" cy="60" r={radius} fill="none"
            stroke="url(#ringGrad)" strokeWidth="10" strokeLinecap="round"
            strokeDasharray={circumference}
            strokeDashoffset={offset}
            className="transition-[stroke-dashoffset] duration-500 ease-out"
          />
        </svg>
        <div className="absolute inset-0 flex flex-col items-center justify-center">
          <div className="font-data text-3xl font-semibold">
            {Math.round(totalWatts)}<span className="text-sm font-normal text-base-content/70"> W</span>
          </div>
          <div className="mt-0.5 text-[10px] uppercase tracking-wide text-base-content/45">of {maxWatts}W max</div>
        </div>
      </div>
      <div className="mt-3.5 flex w-full justify-between border-t border-neutral/50 pt-3.5">
        <div>
          <div className="font-data text-[10px] uppercase tracking-wide text-base-content/45">Today Est.</div>
          <div className="font-data text-[15px] mt-0.5">{todayKwh.toFixed(2)} kWh</div>
        </div>
        <div className="text-right">
          <div className="font-data text-[10px] uppercase tracking-wide text-base-content/45">Devices On</div>
          <div className="font-data text-[15px] mt-0.5">{onCount} / {totalCount}</div>
        </div>
      </div>
    </div>
  );
}
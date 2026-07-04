import { useState } from 'react';

export default function ControlsPanel({ onEvaluate }) {
  const [isRunning, setIsRunning] = useState(false);

  const handleClick = async () => {
    setIsRunning(true);
    try {
      await onEvaluate();
    } finally {
      setIsRunning(false);
    }
  };

  return (
    <div className="card border border-dashed border-neutral bg-base-100">
      <div className="card-body p-5">
        <h2 className="font-display text-sm font-semibold text-base-content/70">Controls</h2>
        <p className="mt-1 text-[12px] text-base-content/45 leading-relaxed">
          Alerts are evaluated automatically every simulator tick on the server. Use this to force a check immediately after toggling a device.
        </p>
        <button className="btn btn-sm btn-secondary btn-soft mt-3 w-fit" onClick={handleClick} disabled={isRunning}>
          {isRunning ? 'Checking…' : 'Run alert check now'}
        </button>
      </div>
    </div>
  );
}
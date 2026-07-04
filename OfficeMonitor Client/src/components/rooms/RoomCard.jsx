export default function RoomCard({ room, onToggleDevice }) {
  return (
    <div className="card bg-base-100 border border-neutral">
      <div className="card-body p-5">
        <div className="flex items-baseline justify-between">
          <span className="font-display text-[15px] font-semibold">{room.roomName}</span>
          <span className="font-data text-[10px] text-base-content/45">{room.code}</span>
        </div>
        <div className="mt-1 mb-2 flex gap-4">
          <div>
            <div className="font-data text-[9.5px] uppercase tracking-wide text-base-content/45">On</div>
            <div className="font-data text-base mt-0.5 text-primary">{room.onCount}/{room.devices.length}</div>
          </div>
          <div>
            <div className="font-data text-[9.5px] uppercase tracking-wide text-base-content/45">Draw</div>
            <div className="font-data text-base mt-0.5">{room.watts} W</div>
          </div>
        </div>
        <div className="flex flex-wrap gap-1.5">
          {room.devices.map(d => (
            <button
              key={d.id}
              onClick={() => onToggleDevice(d.id, !d.isOn)}
              className={`btn btn-xs h-auto gap-1.5 font-data text-[11px] normal-case
                ${d.isOn
                  ? (d.type === 'Fan' ? 'btn-secondary btn-soft' : 'btn-primary btn-soft')
                  : 'btn-ghost border border-neutral bg-base-300 text-base-content/70'}`}
            >
              <span className={`h-1.5 w-1.5 rounded-full ${d.isOn ? (d.type === 'Fan' ? 'bg-secondary' : 'bg-primary') : 'bg-[#4a5160]'}`} />
              {d.name}
            </button>
          ))}
        </div>
      </div>
    </div>
  );
}
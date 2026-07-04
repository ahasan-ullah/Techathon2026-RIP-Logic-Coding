const ROOM_WIDTH = 280, ROOM_HEIGHT = 220, GAP = 15, START_X = 15, START_Y = 40;
const FAN_POSITIONS = [[70, 55], [210, 55]];
const LIGHT_POSITIONS = [[40, 150], [140, 170], [240, 150]];

function FanIcon({ device, x, y, onToggle }) {
  const stroke = device.isOn ? '#57c7b8' : '#4a5160';
  return (
    <g className="cursor-pointer" transform={`translate(${x},${y})`} onClick={() => onToggle(device.id, !device.isOn)}>
      <g className={device.isOn ? 'animate-fan' : ''}>
        <circle r="16" fill="none" stroke={stroke} strokeWidth="1.4"
          className={device.isOn ? 'drop-shadow-[0_0_4px_rgba(87,199,184,0.6)]' : ''} />
        <path d="M0,0 L0,-14 M0,0 L12,7 M0,0 L-12,7" stroke={stroke} strokeWidth="1.6" />
      </g>
    </g>
  );
}

function BulbIcon({ device, x, y, onToggle }) {
  return (
    <g className="cursor-pointer" transform={`translate(${x},${y})`} onClick={() => onToggle(device.id, !device.isOn)}>
      <circle
        r="8"
        fill={device.isOn ? '#f2a93b' : '#3a4150'}
        stroke={device.isOn ? '#f2a93b' : '#333b47'}
        strokeWidth="1.5"
        className={device.isOn ? 'drop-shadow-[0_0_6px_rgba(242,169,59,0.55)]' : ''}
      />
    </g>
  );
}

export default function FloorPlan({ rooms, onToggleDevice }) {
  return (
    <svg viewBox="0 0 900 300" className="h-auto w-full">
      {rooms.map((room, i) => {
        const x = START_X + i * (ROOM_WIDTH + GAP);
        const fans = room.devices.filter(d => d.type === 'Fan');
        const lights = room.devices.filter(d => d.type === 'Light');
        return (
          <g key={room.roomId}>
            <rect x={x} y={START_Y} width={ROOM_WIDTH} height={ROOM_HEIGHT} rx="6" fill="#14171d" stroke="#333b47" strokeWidth="1.5" />
            <text x={x + 12} y={START_Y - 14} fontFamily="'IBM Plex Mono',monospace" fontSize="10.5" fill="#5c6474" letterSpacing="0.6">
              {room.roomName.toUpperCase()}
            </text>
            {fans.map((d, idx) => (
              <FanIcon key={d.id} device={d} x={x + FAN_POSITIONS[idx][0]} y={START_Y + FAN_POSITIONS[idx][1]} onToggle={onToggleDevice} />
            ))}
            {lights.map((d, idx) => (
              <BulbIcon key={d.id} device={d} x={x + LIGHT_POSITIONS[idx][0]} y={START_Y + LIGHT_POSITIONS[idx][1]} onToggle={onToggleDevice} />
            ))}
          </g>
        );
      })}
    </svg>
  );
}
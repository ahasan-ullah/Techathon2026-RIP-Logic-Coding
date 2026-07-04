import { useParams, Link } from 'react-router-dom';
import { useOfficeData } from '../hooks/useOfficeData';
import RoomCard from '../components/rooms/RoomCard';

export default function RoomDetailPage() {
  const { code } = useParams();
  const { rooms, isLoading, toggleDevice } = useOfficeData();

  if (isLoading) return <div className="p-8 text-base-content/45">Loading…</div>;

  const room = rooms.find(r => r.code === code);

  if (!room) {
    return (
      <div className="p-8">
        <p className="text-base-content/60">No room found for code "{code}".</p>
        <Link to="/" className="link link-secondary mt-2 inline-block">Back to dashboard</Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-lg px-5 py-7">
      <Link to="/" className="link link-secondary font-data text-xs">← Back to dashboard</Link>
      <div className="mt-4">
        <RoomCard room={room} onToggleDevice={toggleDevice} />
      </div>
    </div>
  );
}
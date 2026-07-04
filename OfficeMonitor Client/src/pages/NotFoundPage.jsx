import { Link } from 'react-router-dom';

export default function NotFoundPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-2">
      <div className="font-display text-lg font-semibold">Page not found</div>
      <Link to="/" className="link link-secondary text-sm">Back to dashboard</Link>
    </div>
  );
}
import { Routes, Route } from 'react-router-dom';
import DashboardPage from '../pages/DashboardPage';
import RoomDetailPage from '../pages/RoomDetailPage';
import NotFoundPage from '../pages/NotFoundPage';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<DashboardPage />} />
      <Route path="/rooms/:code" element={<RoomDetailPage />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
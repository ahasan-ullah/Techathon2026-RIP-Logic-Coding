import { apiClient } from './client';

export const getRooms = () => apiClient.get('/rooms').then(res => res.data);

export const getRoomByCode = (code) => apiClient.get(`/rooms/${code}`).then(res => res.data);
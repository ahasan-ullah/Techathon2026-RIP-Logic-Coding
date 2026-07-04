import { apiClient } from './client';

export const getDevices = () => apiClient.get('/devices').then(res => res.data);

export const toggleDevice = (id, isOn) =>
  apiClient.post(`/devices/${id}/toggle`, { isOn }).then(res => res.data);
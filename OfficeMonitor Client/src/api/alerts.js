import { apiClient } from './client';

export const getActiveAlerts = () => apiClient.get('/alerts/active').then(res => res.data);

export const evaluateAlerts = () => apiClient.post('/alerts/evaluate').then(res => res.data);
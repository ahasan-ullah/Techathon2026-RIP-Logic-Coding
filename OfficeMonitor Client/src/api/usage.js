import { apiClient } from './client';

export const getUsage = () => apiClient.get('/usage').then(res => res.data);
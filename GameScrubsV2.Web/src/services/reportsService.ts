import { API_BASE_URL } from '@/api.config';
import { type RecentActivityGroup } from '@/models/RecentActivity';
import { getErrorMessage } from './apiErrorHandler';

export const reportsService = {
  async getRecentActivity(): Promise<RecentActivityGroup[]> {
    const response = await fetch(`${API_BASE_URL}/reports/recent-activity`);
    if (!response.ok) {
      const errorMessage = await getErrorMessage(response);
      throw new Error(errorMessage);
    }
    return response.json();
  },
};

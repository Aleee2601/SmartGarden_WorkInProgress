import apiClient from './apiClient';
import ENV from '../config/env';

const analyticsService = {
  /**
   * Get historical sensor data for a plant
   * @param {number} plantId - Plant ID
   * @param {Date} startDate - Start date
   * @param {Date} endDate - End date
   * @param {string} interval - 'hourly', 'daily', or 'weekly'
   */
  async getHistoricalData(plantId, startDate, endDate, interval = 'hourly') {
    try {
      const params = new URLSearchParams({
        startDate: startDate.toISOString(),
        endDate: endDate.toISOString(),
        interval
      });

      const response = await apiClient.get(
        `${ENV.API.BASE_URL}/analytics/plant/${plantId}/historical?${params}`
      );
      return response;
    } catch (error) {
      console.error('Error fetching historical data:', error);
      throw new Error(error.message || 'Failed to fetch historical data');
    }
  },

  /**
   * Get summary statistics for a plant
   * @param {number} plantId - Plant ID
   * @param {number} days - Number of days to look back
   */
  async getPlantSummary(plantId, days = 7) {
    try {
      const response = await apiClient.get(
        `${ENV.API.BASE_URL}/analytics/plant/${plantId}/summary?days=${days}`
      );
      return response;
    } catch (error) {
      console.error('Error fetching plant summary:', error);
      throw new Error(error.message || 'Failed to fetch plant summary');
    }
  }
};

export default analyticsService;

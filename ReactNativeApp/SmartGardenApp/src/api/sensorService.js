// Sensor API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Sensor Service
 * Handles sensor readings, history, and statistics
 */
class SensorService {
  /**
   * Post sensor reading (used by ESP32 devices)
   * @param {Object} sensorData - Sensor reading data
   * @returns {Promise<Object>}
   */
  async postReading(sensorData) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.SENSOR.POST_READING, {
        deviceId: sensorData.deviceId,
        plantId: sensorData.plantId,
        soilMoisture: sensorData.soilMoisture,
        airTemperature: sensorData.airTemperature,
        airHumidity: sensorData.airHumidity,
        lightLevel: sensorData.lightLevel,
        airQuality: sensorData.airQuality,
        waterLevel: sensorData.waterLevel,
        timestamp: sensorData.timestamp || Math.floor(Date.now() / 1000),
        signature: sensorData.signature
      });

      return response;
    } catch (error) {
      console.error('Post sensor reading error:', error);
      throw new Error(error.message || 'Failed to post sensor reading.');
    }
  }

  /**
   * Get all sensor readings
   * @param {Object} filters - Optional filters
   * @returns {Promise<Array>}
   */
  async getAllReadings(filters = {}) {
    try {
      const queryParams = new URLSearchParams(filters).toString();
      const endpoint = queryParams
        ? `${ENV.ENDPOINTS.SENSOR.GET_READINGS}?${queryParams}`
        : ENV.ENDPOINTS.SENSOR.GET_READINGS;

      const response = await apiClient.get(endpoint);
      return response;
    } catch (error) {
      console.error('Get sensor readings error:', error);
      throw new Error(error.message || 'Failed to fetch sensor readings.');
    }
  }

  /**
   * Get sensor readings for a specific plant
   * @param {number} plantId
   * @param {Object} options - Pagination and filters
   * @returns {Promise<Array>}
   */
  async getReadingsByPlant(plantId, options = {}) {
    try {
      const queryParams = new URLSearchParams(options).toString();
      const endpoint = queryParams
        ? `${ENV.ENDPOINTS.SENSOR.GET_BY_PLANT(plantId)}?${queryParams}`
        : ENV.ENDPOINTS.SENSOR.GET_BY_PLANT(plantId);

      const response = await apiClient.get(endpoint);
      return response;
    } catch (error) {
      console.error('Get plant sensor readings error:', error);
      throw new Error(error.message || 'Failed to fetch plant sensor readings.');
    }
  }

  /**
   * Get latest sensor reading for a plant
   * @param {number} plantId
   * @returns {Promise<Object>}
   */
  async getLatestReading(plantId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.SENSOR.GET_LATEST(plantId));
      return response;
    } catch (error) {
      console.error('Get latest reading error:', error);
      throw new Error(error.message || 'Failed to fetch latest sensor reading.');
    }
  }

  /**
   * Get sensor history for a plant
   * @param {number} plantId
   * @param {number} hours - Number of hours to fetch (default: 24)
   * @returns {Promise<Array>}
   */
  async getSensorHistory(plantId, hours = 24) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.SENSOR.GET_HISTORY(plantId, hours));
      return response;
    } catch (error) {
      console.error('Get sensor history error:', error);
      throw new Error(error.message || 'Failed to fetch sensor history.');
    }
  }

  /**
   * Get sensor statistics for a plant
   * @param {number} plantId
   * @param {number} days - Number of days for statistics (default: 7)
   * @returns {Promise<Object>}
   */
  async getStatistics(plantId, days = 7) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.SENSOR.GET_STATISTICS(plantId, days));
      return response;
    } catch (error) {
      console.error('Get sensor statistics error:', error);
      throw new Error(error.message || 'Failed to fetch sensor statistics.');
    }
  }

  /**
   * Get weekly statistics (for charts)
   * @param {number} plantId
   * @returns {Promise<Array>}
   */
  async getWeeklyStatistics(plantId) {
    try {
      const response = await this.getStatistics(plantId, 7);

      // Transform data for weekly chart
      const weeklyData = this.transformToWeeklyChart(response);
      return weeklyData;
    } catch (error) {
      console.error('Get weekly statistics error:', error);
      throw new Error(error.message || 'Failed to fetch weekly statistics.');
    }
  }

  /**
   * Transform statistics data to weekly chart format
   * @param {Object} statistics
   * @returns {Array}
   */
  transformToWeeklyChart(statistics) {
    const days = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'];
    const today = new Date();
    const weeklyData = [];

    for (let i = 6; i >= 0; i--) {
      const date = new Date(today);
      date.setDate(date.getDate() - i);

      const dayIndex = date.getDay();
      const dayName = days[dayIndex === 0 ? 6 : dayIndex - 1]; // Adjust for Monday start

      // Find data for this day in statistics
      const dayData = statistics.dailyAverages?.find(d => {
        const statDate = new Date(d.date);
        return statDate.toDateString() === date.toDateString();
      }) || {};

      weeklyData.push({
        day: dayName,
        light: dayData.avgLight || 0,
        water: dayData.totalWatering || 0,
        temp: dayData.avgTemperature || 0,
        soil: dayData.avgSoilMoisture || 0
      });
    }

    return weeklyData;
  }

  /**
   * Get real-time sensor data with polling
   * @param {number} plantId
   * @param {Function} callback - Called with new data
   * @param {number} interval - Polling interval in ms (default: 60000 = 1 minute)
   * @returns {Function} Stop function
   */
  startPolling(plantId, callback, interval = 60000) {
    let isPolling = true;
    let timeoutId;

    const poll = async () => {
      if (!isPolling) return;

      try {
        const data = await this.getLatestReading(plantId);
        callback(data);
      } catch (error) {
        console.error('Polling error:', error);
      }

      if (isPolling) {
        timeoutId = setTimeout(poll, interval);
      }
    };

    // Start polling
    poll();

    // Return stop function
    return () => {
      isPolling = false;
      if (timeoutId) clearTimeout(timeoutId);
    };
  }

  /**
   * Calculate sensor value percentage based on calibration
   * @param {number} value - Raw sensor value
   * @param {number} min - Calibrated minimum value
   * @param {number} max - Calibrated maximum value
   * @returns {number} Percentage (0-100)
   */
  calculatePercentage(value, min, max) {
    if (min === max) return 50;
    const percentage = ((value - min) / (max - min)) * 100;
    return Math.max(0, Math.min(100, percentage));
  }

  /**
   * Check if sensor value is within healthy range
   * @param {number} value
   * @param {number} min
   * @param {number} max
   * @returns {boolean}
   */
  isHealthy(value, min, max) {
    return value >= min && value <= max;
  }

  /**
   * Get sensor status (healthy, warning, critical)
   * @param {number} value
   * @param {number} min
   * @param {number} max
   * @returns {string}
   */
  getStatus(value, min, max) {
    if (value < min) {
      const diff = min - value;
      const range = max - min;
      return diff > range * 0.3 ? 'critical' : 'warning';
    }

    if (value > max) {
      const diff = value - max;
      const range = max - min;
      return diff > range * 0.3 ? 'critical' : 'warning';
    }

    return 'healthy';
  }
}

// Export singleton instance
const sensorService = new SensorService();
export default sensorService;

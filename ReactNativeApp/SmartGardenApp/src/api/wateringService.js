// Watering API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Watering Service
 * Handles manual watering, schedules, and auto-watering
 */
class WateringService {
  /**
   * Trigger manual watering
   * @param {number} plantId
   * @param {number} duration - Duration in seconds
   * @returns {Promise<Object>}
   */
  async waterManually(plantId, duration = 30) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.WATERING.MANUAL, {
        plantId,
        duration: Math.min(Math.max(duration, ENV.WATERING.MIN_DURATION), ENV.WATERING.MAX_DURATION)
      });

      return response;
    } catch (error) {
      console.error('Manual watering error:', error);
      throw new Error(error.message || 'Failed to trigger manual watering.');
    }
  }

  /**
   * Create watering schedule
   * @param {Object} scheduleData
   * @returns {Promise<Object>}
   */
  async createSchedule(scheduleData) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.WATERING.SCHEDULE, {
        plantId: scheduleData.plantId,
        scheduleName: scheduleData.scheduleName,
        isActive: scheduleData.isActive || true,

        // Time settings
        startTime: scheduleData.startTime,
        frequency: scheduleData.frequency, // daily, weekly, custom
        daysOfWeek: scheduleData.daysOfWeek, // For weekly schedules
        interval: scheduleData.interval, // For custom intervals

        // Watering settings
        duration: scheduleData.duration || ENV.WATERING.DEFAULT_DURATION,

        // Conditions (optional)
        onlyWhenDry: scheduleData.onlyWhenDry || false,
        minSoilMoisture: scheduleData.minSoilMoisture
      });

      return response;
    } catch (error) {
      console.error('Create schedule error:', error);
      throw new Error(error.message || 'Failed to create watering schedule.');
    }
  }

  /**
   * Get watering schedules for a plant
   * @param {number} plantId
   * @returns {Promise<Array>}
   */
  async getSchedules(plantId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.WATERING.GET_SCHEDULES(plantId));
      return response;
    } catch (error) {
      console.error('Get schedules error:', error);
      throw new Error(error.message || 'Failed to fetch watering schedules.');
    }
  }

  /**
   * Update watering schedule
   * @param {number} scheduleId
   * @param {Object} scheduleData
   * @returns {Promise<Object>}
   */
  async updateSchedule(scheduleId, scheduleData) {
    try {
      const response = await apiClient.put(`${ENV.ENDPOINTS.WATERING.SCHEDULE}/${scheduleId}`, scheduleData);
      return response;
    } catch (error) {
      console.error('Update schedule error:', error);
      throw new Error(error.message || 'Failed to update watering schedule.');
    }
  }

  /**
   * Delete watering schedule
   * @param {number} scheduleId
   * @returns {Promise<void>}
   */
  async deleteSchedule(scheduleId) {
    try {
      await apiClient.delete(`${ENV.ENDPOINTS.WATERING.SCHEDULE}/${scheduleId}`);
    } catch (error) {
      console.error('Delete schedule error:', error);
      throw new Error(error.message || 'Failed to delete watering schedule.');
    }
  }

  /**
   * Enable/disable schedule
   * @param {number} scheduleId
   * @param {boolean} isActive
   * @returns {Promise<Object>}
   */
  async toggleSchedule(scheduleId, isActive) {
    try {
      const response = await apiClient.patch(`${ENV.ENDPOINTS.WATERING.SCHEDULE}/${scheduleId}`, {
        isActive
      });
      return response;
    } catch (error) {
      console.error('Toggle schedule error:', error);
      throw new Error(error.message || 'Failed to toggle watering schedule.');
    }
  }

  /**
   * Configure auto-watering for a plant
   * @param {number} plantId
   * @param {Object} config
   * @returns {Promise<Object>}
   */
  async configureAutoWatering(plantId, config) {
    try {
      const response = await apiClient.put(ENV.ENDPOINTS.WATERING.AUTO_CONFIG(plantId), {
        autoWateringEnabled: config.enabled,
        wateringDuration: config.duration || ENV.WATERING.DEFAULT_DURATION,
        minSoilMoisture: config.minSoilMoisture || 30,
        maxSoilMoisture: config.maxSoilMoisture || 70,
        wateringIntensity: config.intensity || 3 // 1-5 scale
      });

      return response;
    } catch (error) {
      console.error('Configure auto-watering error:', error);
      throw new Error(error.message || 'Failed to configure auto-watering.');
    }
  }

  /**
   * Get watering commands for device (called by ESP32)
   * @param {string} deviceId
   * @returns {Promise<Object>}
   */
  async getWateringCommands(deviceId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.WATERING.COMMANDS(deviceId));
      return response;
    } catch (error) {
      console.error('Get watering commands error:', error);
      throw new Error(error.message || 'Failed to fetch watering commands.');
    }
  }

  /**
   * Get watering history for a plant
   * @param {number} plantId
   * @param {Object} options - Pagination and filters
   * @returns {Promise<Array>}
   */
  async getWateringHistory(plantId, options = {}) {
    try {
      const queryParams = new URLSearchParams(options).toString();
      const endpoint = queryParams
        ? `/watering/plant/${plantId}/history?${queryParams}`
        : `/watering/plant/${plantId}/history`;

      const response = await apiClient.get(endpoint);
      return response;
    } catch (error) {
      console.error('Get watering history error:', error);
      throw new Error(error.message || 'Failed to fetch watering history.');
    }
  }

  /**
   * Get watering statistics for a plant
   * @param {number} plantId
   * @param {number} days - Number of days
   * @returns {Promise<Object>}
   */
  async getWateringStatistics(plantId, days = 7) {
    try {
      const response = await apiClient.get(`/watering/plant/${plantId}/statistics?days=${days}`);
      return response;
    } catch (error) {
      console.error('Get watering statistics error:', error);
      throw new Error(error.message || 'Failed to fetch watering statistics.');
    }
  }

  /**
   * Calculate recommended watering duration based on soil moisture
   * @param {number} currentMoisture - Current soil moisture percentage
   * @param {number} targetMoisture - Target soil moisture percentage
   * @param {number} baseDuration - Base watering duration in seconds
   * @returns {number} Recommended duration in seconds
   */
  calculateDuration(currentMoisture, targetMoisture, baseDuration = 30) {
    const moistureDiff = targetMoisture - currentMoisture;

    if (moistureDiff <= 0) return 0;

    // Linear scaling: 1% moisture = baseDuration / 20 seconds
    const duration = (moistureDiff / 20) * baseDuration;

    return Math.min(Math.max(duration, ENV.WATERING.MIN_DURATION), ENV.WATERING.MAX_DURATION);
  }

  /**
   * Check if watering is needed based on sensor data
   * @param {Object} sensorData
   * @param {Object} thresholds
   * @returns {boolean}
   */
  isWateringNeeded(sensorData, thresholds) {
    if (!sensorData || !thresholds) return false;

    return sensorData.soilMoisture < thresholds.minSoilMoisture;
  }

  /**
   * Get next scheduled watering time
   * @param {Array} schedules
   * @returns {Date|null}
   */
  getNextWateringTime(schedules) {
    if (!schedules || schedules.length === 0) return null;

    const activeSchedules = schedules.filter(s => s.isActive);
    if (activeSchedules.length === 0) return null;

    // Get the earliest next execution time
    const nextTimes = activeSchedules
      .map(s => new Date(s.nextExecutionTime))
      .filter(d => !isNaN(d.getTime()))
      .sort((a, b) => a - b);

    return nextTimes.length > 0 ? nextTimes[0] : null;
  }
}

// Export singleton instance
const wateringService = new WateringService();
export default wateringService;

// Plant API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Plant Service
 * Handles plant CRUD operations and calibration
 */
class PlantService {
  /**
   * Get all plants for current user
   * @returns {Promise<Array>} List of plants
   */
  async getAllPlants() {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.PLANTS.GET_ALL);
      return response;
    } catch (error) {
      console.error('Get plants error:', error);
      throw new Error(error.message || 'Failed to fetch plants.');
    }
  }

  /**
   * Get plant by ID
   * @param {number} plantId
   * @returns {Promise<Object>} Plant data
   */
  async getPlantById(plantId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.PLANTS.GET_BY_ID(plantId));
      return response;
    } catch (error) {
      console.error('Get plant error:', error);
      throw new Error(error.message || 'Failed to fetch plant details.');
    }
  }

  /**
   * Create new plant
   * @param {Object} plantData - Plant information
   * @returns {Promise<Object>} Created plant
   */
  async createPlant(plantData) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.PLANTS.CREATE, {
        name: plantData.name,
        species: plantData.species,
        description: plantData.description,
        location: plantData.location,
        imageUrl: plantData.imageUrl,
        deviceId: plantData.deviceId,

        // Watering thresholds
        minSoilMoisture: plantData.minSoilMoisture || 30,
        maxSoilMoisture: plantData.maxSoilMoisture || 70,

        // Environmental thresholds
        minTemperature: plantData.minTemperature || 15,
        maxTemperature: plantData.maxTemperature || 30,
        minHumidity: plantData.minHumidity || 40,
        maxHumidity: plantData.maxHumidity || 80,
        minLightLevel: plantData.minLightLevel || 200,
        maxLightLevel: plantData.maxLightLevel || 800,

        // Auto watering
        autoWateringEnabled: plantData.autoWateringEnabled || false,
        wateringDuration: plantData.wateringDuration || 30
      });

      return response;
    } catch (error) {
      console.error('Create plant error:', error);
      throw new Error(error.message || 'Failed to create plant.');
    }
  }

  /**
   * Update plant
   * @param {number} plantId
   * @param {Object} plantData
   * @returns {Promise<Object>} Updated plant
   */
  async updatePlant(plantId, plantData) {
    try {
      const response = await apiClient.put(ENV.ENDPOINTS.PLANTS.UPDATE(plantId), plantData);
      return response;
    } catch (error) {
      console.error('Update plant error:', error);
      throw new Error(error.message || 'Failed to update plant.');
    }
  }

  /**
   * Delete plant
   * @param {number} plantId
   * @returns {Promise<void>}
   */
  async deletePlant(plantId) {
    try {
      await apiClient.delete(ENV.ENDPOINTS.PLANTS.DELETE(plantId));
    } catch (error) {
      console.error('Delete plant error:', error);
      throw new Error(error.message || 'Failed to delete plant.');
    }
  }

  /**
   * Get plant calibration data
   * @param {number} plantId
   * @returns {Promise<Object>} Calibration data
   */
  async getCalibration(plantId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.PLANTS.CALIBRATION(plantId));
      return response;
    } catch (error) {
      console.error('Get calibration error:', error);
      throw new Error(error.message || 'Failed to fetch calibration data.');
    }
  }

  /**
   * Update plant calibration data
   * @param {number} plantId
   * @param {Object} calibrationData
   * @returns {Promise<Object>} Updated calibration
   */
  async updateCalibration(plantId, calibrationData) {
    try {
      const response = await apiClient.put(
        ENV.ENDPOINTS.PLANTS.UPDATE_CALIBRATION(plantId),
        {
          calibrationMode: calibrationData.calibrationMode,

          // Light sensor calibration
          lightDark: calibrationData.lightSensor?.dark?.value,
          lightLow: calibrationData.lightSensor?.lowLight?.value,
          lightMedium: calibrationData.lightSensor?.mediumIndirect?.value,
          lightBright: calibrationData.lightSensor?.brightIndirect?.value,
          lightDirect: calibrationData.lightSensor?.brightDirect?.value,

          // Soil moisture calibration
          soilDry: calibrationData.soilMoisture?.dry?.value,
          soilMoist: calibrationData.soilMoisture?.moist?.value,
          soilSaturated: calibrationData.soilMoisture?.saturated?.value,

          // Water level calibration
          waterEmpty: calibrationData.waterLevel?.empty?.value,
          waterHalf: calibrationData.waterLevel?.half?.value,
          waterFull: calibrationData.waterLevel?.full?.value,

          // Temperature calibration
          temperatureRoom: calibrationData.temperature?.room?.value
        }
      );

      return response;
    } catch (error) {
      console.error('Update calibration error:', error);
      throw new Error(error.message || 'Failed to update calibration.');
    }
  }

  /**
   * Enable/disable calibration mode for a plant
   * @param {number} plantId
   * @param {boolean} enabled
   * @returns {Promise<Object>}
   */
  async setCalibrationMode(plantId, enabled) {
    try {
      const response = await apiClient.put(
        ENV.ENDPOINTS.PLANTS.UPDATE_CALIBRATION(plantId),
        { calibrationMode: enabled }
      );
      return response;
    } catch (error) {
      console.error('Set calibration mode error:', error);
      throw new Error(error.message || 'Failed to set calibration mode.');
    }
  }

  /**
   * Update auto-watering settings
   * @param {number} plantId
   * @param {Object} settings - Auto-watering configuration
   * @returns {Promise<Object>}
   */
  async updateAutoWatering(plantId, settings) {
    try {
      const response = await apiClient.put(ENV.ENDPOINTS.PLANTS.UPDATE(plantId), {
        autoWateringEnabled: settings.enabled,
        wateringDuration: settings.duration,
        minSoilMoisture: settings.minSoilMoisture,
        maxSoilMoisture: settings.maxSoilMoisture
      });
      return response;
    } catch (error) {
      console.error('Update auto-watering error:', error);
      throw new Error(error.message || 'Failed to update auto-watering settings.');
    }
  }

  /**
   * Search plants by name or species (local search)
   * @param {string} query
   * @returns {Promise<Array>}
   */
  async searchPlantsLocal(query) {
    try {
      const response = await apiClient.get(`${ENV.ENDPOINTS.PLANTS.GET_ALL}?search=${encodeURIComponent(query)}`);
      return response;
    } catch (error) {
      console.error('Search plants error:', error);
      throw new Error(error.message || 'Failed to search plants.');
    }
  }

  /**
   * 🌟 SMART SEARCH: Search for plant species in external database
   * Returns plants with auto-suggested moisture thresholds
   * @param {string} query - Search term (e.g., "basil", "tomato")
   * @returns {Promise<Array>} Plant search results with smart defaults
   */
  async searchPlants(query) {
    try {
      const response = await apiClient.get(`${ENV.API.BASE_URL}/plant/search?q=${encodeURIComponent(query)}`);
      return response;
    } catch (error) {
      console.error('Smart search error:', error);
      throw new Error(error.message || 'Failed to search plants from database.');
    }
  }

  /**
   * 🌟 Create plant from smart search result
   * Accepts pre-filled data with auto-suggested thresholds
   * @param {Object} plantData - Plant data from search wizard
   * @returns {Promise<Object>} Created plant
   */
  async createPlantFromSearch(plantData) {
    try {
      const response = await apiClient.post(`${ENV.API.BASE_URL}/plant/from-search`, {
        nickname: plantData.nickname,
        speciesName: plantData.speciesName,
        imageUrl: plantData.imageUrl,
        minMoistureThreshold: plantData.minMoistureThreshold,
        roomName: plantData.roomName,
        isOutdoor: plantData.isOutdoor || false,
        deviceId: plantData.deviceId || null
      });

      return response;
    } catch (error) {
      console.error('Create plant from search error:', error);
      throw new Error(error.message || 'Failed to create plant.');
    }
  }
}

// Export singleton instance
const plantService = new PlantService();
export default plantService;

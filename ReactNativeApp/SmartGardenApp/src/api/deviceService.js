// Device API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Device Service
 * Handles device management and approval
 */
class DeviceService {
  /**
   * Get all devices
   * @returns {Promise<Array>}
   */
  async getAllDevices() {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.DEVICES.GET_ALL);
      return response;
    } catch (error) {
      console.error('Get devices error:', error);
      throw new Error(error.message || 'Failed to fetch devices.');
    }
  }

  /**
   * Get device by ID
   * @param {number} deviceId
   * @returns {Promise<Object>}
   */
  async getDeviceById(deviceId) {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.DEVICES.GET_BY_ID(deviceId));
      return response;
    } catch (error) {
      console.error('Get device error:', error);
      throw new Error(error.message || 'Failed to fetch device details.');
    }
  }

  /**
   * Get pending devices awaiting approval
   * @returns {Promise<Array>}
   */
  async getPendingDevices() {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.DEVICE_AUTH.PENDING);
      return response;
    } catch (error) {
      console.error('Get pending devices error:', error);
      throw new Error(error.message || 'Failed to fetch pending devices.');
    }
  }

  /**
   * Approve a device
   * @param {number} deviceId
   * @returns {Promise<Object>}
   */
  async approveDevice(deviceId) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.DEVICE_AUTH.APPROVE, {
        deviceId
      });
      return response;
    } catch (error) {
      console.error('Approve device error:', error);
      throw new Error(error.message || 'Failed to approve device.');
    }
  }

  /**
   * Update device information
   * @param {number} deviceId
   * @param {Object} deviceData
   * @returns {Promise<Object>}
   */
  async updateDevice(deviceId, deviceData) {
    try {
      const response = await apiClient.put(ENV.ENDPOINTS.DEVICES.UPDATE(deviceId), {
        deviceName: deviceData.deviceName,
        location: deviceData.location,
        description: deviceData.description,
        isActive: deviceData.isActive
      });
      return response;
    } catch (error) {
      console.error('Update device error:', error);
      throw new Error(error.message || 'Failed to update device.');
    }
  }

  /**
   * Delete/deactivate device
   * @param {number} deviceId
   * @returns {Promise<void>}
   */
  async deleteDevice(deviceId) {
    try {
      await apiClient.delete(ENV.ENDPOINTS.DEVICES.DELETE(deviceId));
    } catch (error) {
      console.error('Delete device error:', error);
      throw new Error(error.message || 'Failed to delete device.');
    }
  }

  /**
   * Get device heartbeat status
   * @param {number} deviceId
   * @returns {Promise<Object>}
   */
  async getDeviceStatus(deviceId) {
    try {
      const device = await this.getDeviceById(deviceId);

      // Calculate online status based on last heartbeat
      const isOnline = this.isDeviceOnline(device.lastHeartbeat);

      return {
        ...device,
        isOnline,
        status: isOnline ? 'online' : 'offline'
      };
    } catch (error) {
      console.error('Get device status error:', error);
      throw new Error(error.message || 'Failed to fetch device status.');
    }
  }

  /**
   * Check if device is online based on last heartbeat
   * @param {string} lastHeartbeat - ISO timestamp
   * @returns {boolean}
   */
  isDeviceOnline(lastHeartbeat) {
    if (!lastHeartbeat) return false;

    const now = new Date();
    const lastSeen = new Date(lastHeartbeat);
    const diffMinutes = (now - lastSeen) / (1000 * 60);

    // Device is considered online if heartbeat was received in last 5 minutes
    return diffMinutes < 5;
  }

  /**
   * Get device signal strength category
   * @param {number} rssi - WiFi signal strength in dBm
   * @returns {string}
   */
  getSignalStrength(rssi) {
    if (rssi >= -50) return 'excellent';
    if (rssi >= -60) return 'good';
    if (rssi >= -70) return 'fair';
    return 'poor';
  }

  /**
   * Get battery level category
   * @param {number} batteryLevel - Battery percentage
   * @returns {string}
   */
  getBatteryStatus(batteryLevel) {
    if (batteryLevel === null || batteryLevel === undefined) return 'powered';
    if (batteryLevel > 75) return 'full';
    if (batteryLevel > 50) return 'good';
    if (batteryLevel > 25) return 'low';
    return 'critical';
  }

  /**
   * Register a new device (called by ESP32)
   * @param {Object} deviceData
   * @returns {Promise<Object>}
   */
  async registerDevice(deviceData) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.DEVICE_AUTH.REGISTER, {
        macAddress: deviceData.macAddress,
        model: deviceData.model,
        firmwareVersion: deviceData.firmwareVersion,
        serialNumber: deviceData.serialNumber
      });
      return response;
    } catch (error) {
      console.error('Register device error:', error);
      throw new Error(error.message || 'Failed to register device.');
    }
  }

  /**
   * Send device heartbeat (called by ESP32)
   * @param {Object} heartbeatData
   * @returns {Promise<Object>}
   */
  async sendHeartbeat(heartbeatData) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.DEVICE_AUTH.HEARTBEAT, {
        deviceId: heartbeatData.deviceId,
        batteryLevel: heartbeatData.batteryLevel,
        signalStrength: heartbeatData.signalStrength,
        firmwareVersion: heartbeatData.firmwareVersion,
        ipAddress: heartbeatData.ipAddress
      });
      return response;
    } catch (error) {
      console.error('Send heartbeat error:', error);
      throw new Error(error.message || 'Failed to send heartbeat.');
    }
  }

  /**
   * Refresh device token (called by ESP32)
   * @param {string} deviceId
   * @param {string} refreshToken
   * @returns {Promise<Object>}
   */
  async refreshDeviceToken(deviceId, refreshToken) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.DEVICE_AUTH.REFRESH_TOKEN, {
        deviceId,
        refreshToken
      });
      return response;
    } catch (error) {
      console.error('Refresh device token error:', error);
      throw new Error(error.message || 'Failed to refresh device token.');
    }
  }
}

// Export singleton instance
const deviceService = new DeviceService();
export default deviceService;

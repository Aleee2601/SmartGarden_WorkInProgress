// Export API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Export Service
 * Handles data export and report generation
 */
class ExportService {
  /**
   * Export sensor data to CSV
   * @param {number} plantId
   * @param {Date} startDate
   * @param {Date} endDate
   * @returns {Promise<Blob>}
   */
  async exportSensorDataCsv(plantId, startDate = null, endDate = null) {
    try {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate.toISOString());
      if (endDate) params.append('endDate', endDate.toISOString());

      const queryString = params.toString();
      const url = `/export/plant/${plantId}/sensors/csv${queryString ? '?' + queryString : ''}`;

      const response = await fetch(`${ENV.API_BASE_URL}${url}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${apiClient.token}`
        }
      });

      if (!response.ok) throw new Error('Failed to export CSV');

      const blob = await response.blob();
      this.downloadBlob(blob, `sensor-data-plant-${plantId}.csv`);

      return blob;
    } catch (error) {
      console.error('Export CSV error:', error);
      throw new Error(error.message || 'Failed to export sensor data.');
    }
  }

  /**
   * Export all plants to CSV
   * @returns {Promise<Blob>}
   */
  async exportAllPlantsCsv() {
    try {
      const response = await fetch(`${ENV.API_BASE_URL}/export/plants/csv`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${apiClient.token}`
        }
      });

      if (!response.ok) throw new Error('Failed to export all plants');

      const blob = await response.blob();
      this.downloadBlob(blob, `all-plants-${new Date().toISOString().split('T')[0]}.csv`);

      return blob;
    } catch (error) {
      console.error('Export all plants error:', error);
      throw new Error(error.message || 'Failed to export plants data.');
    }
  }

  /**
   * Generate plant report (HTML/PDF)
   * @param {number} plantId
   * @param {Date} startDate
   * @param {Date} endDate
   * @returns {Promise<Blob>}
   */
  async generatePlantReport(plantId, startDate = null, endDate = null) {
    try {
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate.toISOString());
      if (endDate) params.append('endDate', endDate.toISOString());

      const queryString = params.toString();
      const url = `/export/plant/${plantId}/report${queryString ? '?' + queryString : ''}`;

      const response = await fetch(`${ENV.API_BASE_URL}${url}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${apiClient.token}`
        }
      });

      if (!response.ok) throw new Error('Failed to generate report');

      const blob = await response.blob();
      this.downloadBlob(blob, `plant-report-${plantId}.html`);

      return blob;
    } catch (error) {
      console.error('Generate report error:', error);
      throw new Error(error.message || 'Failed to generate plant report.');
    }
  }

  /**
   * Generate user summary report
   * @returns {Promise<Blob>}
   */
  async generateUserSummary() {
    try {
      const response = await fetch(`${ENV.API_BASE_URL}/export/summary`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${apiClient.token}`
        }
      });

      if (!response.ok) throw new Error('Failed to generate summary');

      const blob = await response.blob();
      this.downloadBlob(blob, `garden-summary-${new Date().toISOString().split('T')[0]}.html`);

      return blob;
    } catch (error) {
      console.error('Generate summary error:', error);
      throw new Error(error.message || 'Failed to generate summary report.');
    }
  }

  /**
   * Download sensor data with format selection
   * @param {number} plantId
   * @param {string} format - 'csv', 'html', or 'pdf'
   * @param {Date} startDate
   * @param {Date} endDate
   * @returns {Promise<Blob>}
   */
  async downloadSensorData(plantId, format = 'csv', startDate = null, endDate = null) {
    try {
      const params = new URLSearchParams({ format });
      if (startDate) params.append('startDate', startDate.toISOString());
      if (endDate) params.append('endDate', endDate.toISOString());

      const url = `/export/plant/${plantId}/sensors/download?${params.toString()}`;

      const response = await fetch(`${ENV.API_BASE_URL}${url}`, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${apiClient.token}`
        }
      });

      if (!response.ok) throw new Error(`Failed to download ${format.toUpperCase()}`);

      const blob = await response.blob();
      const extension = format === 'csv' ? 'csv' : 'html';
      this.downloadBlob(blob, `sensor-data-${plantId}.${extension}`);

      return blob;
    } catch (error) {
      console.error('Download sensor data error:', error);
      throw new Error(error.message || 'Failed to download sensor data.');
    }
  }

  /**
   * Helper: Download blob as file
   * @param {Blob} blob
   * @param {string} filename
   */
  downloadBlob(blob, filename) {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  /**
   * Get date range presets
   * @returns {Object}
   */
  getDatePresets() {
    const now = new Date();
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

    return {
      today: {
        label: 'Today',
        startDate: today,
        endDate: now
      },
      last7Days: {
        label: 'Last 7 Days',
        startDate: new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000),
        endDate: now
      },
      last30Days: {
        label: 'Last 30 Days',
        startDate: new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000),
        endDate: now
      },
      thisMonth: {
        label: 'This Month',
        startDate: new Date(now.getFullYear(), now.getMonth(), 1),
        endDate: now
      },
      lastMonth: {
        label: 'Last Month',
        startDate: new Date(now.getFullYear(), now.getMonth() - 1, 1),
        endDate: new Date(now.getFullYear(), now.getMonth(), 0)
      },
      all: {
        label: 'All Time',
        startDate: null,
        endDate: null
      }
    };
  }
}

// Export singleton instance
const exportService = new ExportService();
export default exportService;

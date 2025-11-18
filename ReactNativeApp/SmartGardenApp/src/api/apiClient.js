// API Client with Authentication Interceptors

import ENV from '../config/env';

/**
 * API Client for SmartGarden Backend
 * Handles all HTTP requests with automatic token management
 */
class ApiClient {
  constructor() {
    this.baseURL = ENV.API_BASE_URL;
    this.timeout = ENV.APP.TIMEOUT;
    this.token = null;
    this.refreshToken = null;

    // Load tokens from localStorage
    this.loadTokens();
  }

  /**
   * Load tokens from localStorage
   */
  loadTokens() {
    if (typeof window !== 'undefined') {
      this.token = localStorage.getItem(ENV.STORAGE_KEYS.USER_TOKEN);
      this.refreshToken = localStorage.getItem(ENV.STORAGE_KEYS.REFRESH_TOKEN);
    }
  }

  /**
   * Save tokens to localStorage
   */
  saveTokens(token, refreshToken) {
    this.token = token;
    this.refreshToken = refreshToken;

    if (typeof window !== 'undefined') {
      if (token) localStorage.setItem(ENV.STORAGE_KEYS.USER_TOKEN, token);
      if (refreshToken) localStorage.setItem(ENV.STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
    }
  }

  /**
   * Clear tokens from memory and localStorage
   */
  clearTokens() {
    this.token = null;
    this.refreshToken = null;

    if (typeof window !== 'undefined') {
      localStorage.removeItem(ENV.STORAGE_KEYS.USER_TOKEN);
      localStorage.removeItem(ENV.STORAGE_KEYS.REFRESH_TOKEN);
      localStorage.removeItem(ENV.STORAGE_KEYS.USER_DATA);
    }
  }

  /**
   * Build headers for API requests
   */
  buildHeaders(customHeaders = {}) {
    const headers = {
      'Content-Type': 'application/json',
      ...customHeaders
    };

    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }

    return headers;
  }

  /**
   * Make HTTP request with automatic retry and token refresh
   */
  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const headers = this.buildHeaders(options.headers);

    const config = {
      method: options.method || 'GET',
      headers,
      ...options
    };

    // Add body if present
    if (options.body) {
      config.body = JSON.stringify(options.body);
    }

    try {
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), this.timeout);

      config.signal = controller.signal;

      const response = await fetch(url, config);
      clearTimeout(timeoutId);

      // Handle 401 Unauthorized - Token expired
      if (response.status === 401 && this.refreshToken) {
        console.log('Token expired, attempting refresh...');
        const refreshed = await this.refreshAccessToken();

        if (refreshed) {
          // Retry original request with new token
          const retryHeaders = this.buildHeaders(options.headers);
          const retryResponse = await fetch(url, {
            ...config,
            headers: retryHeaders
          });

          return this.handleResponse(retryResponse);
        } else {
          // Refresh failed, clear tokens and throw error
          this.clearTokens();
          throw new Error('Session expired. Please login again.');
        }
      }

      return this.handleResponse(response);

    } catch (error) {
      if (error.name === 'AbortError') {
        throw new Error('Request timeout. Please check your connection.');
      }
      throw error;
    }
  }

  /**
   * Handle API response
   */
  async handleResponse(response) {
    const contentType = response.headers.get('content-type');
    const isJson = contentType && contentType.includes('application/json');

    const data = isJson ? await response.json() : await response.text();

    if (!response.ok) {
      const error = new Error(data.message || data.error || `HTTP ${response.status}: ${response.statusText}`);
      error.status = response.status;
      error.data = data;
      throw error;
    }

    return data;
  }

  /**
   * Refresh access token using refresh token
   */
  async refreshAccessToken() {
    try {
      const response = await fetch(`${this.baseURL}${ENV.ENDPOINTS.AUTH.REFRESH_TOKEN}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ refreshToken: this.refreshToken })
      });

      if (response.ok) {
        const data = await response.json();
        this.saveTokens(data.token, data.refreshToken || this.refreshToken);
        return true;
      }

      return false;
    } catch (error) {
      console.error('Token refresh failed:', error);
      return false;
    }
  }

  /**
   * GET request
   */
  async get(endpoint, options = {}) {
    return this.request(endpoint, { ...options, method: 'GET' });
  }

  /**
   * POST request
   */
  async post(endpoint, body, options = {}) {
    return this.request(endpoint, { ...options, method: 'POST', body });
  }

  /**
   * PUT request
   */
  async put(endpoint, body, options = {}) {
    return this.request(endpoint, { ...options, method: 'PUT', body });
  }

  /**
   * DELETE request
   */
  async delete(endpoint, options = {}) {
    return this.request(endpoint, { ...options, method: 'DELETE' });
  }

  /**
   * PATCH request
   */
  async patch(endpoint, body, options = {}) {
    return this.request(endpoint, { ...options, method: 'PATCH', body });
  }
}

// Export singleton instance
const apiClient = new ApiClient();
export default apiClient;

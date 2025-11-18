// Authentication API Service

import apiClient from './apiClient';
import ENV from '../config/env';

/**
 * Authentication Service
 * Handles user login, registration, and session management
 */
class AuthService {
  /**
   * Login user
   * @param {string} email - User email
   * @param {string} password - User password
   * @returns {Promise<Object>} User data and tokens
   */
  async login(email, password) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.AUTH.LOGIN, {
        email,
        password
      });

      // Save tokens and user data
      if (response.token) {
        apiClient.saveTokens(response.token, response.refreshToken);
        this.saveUserData(response.user || response);
      }

      return response;
    } catch (error) {
      console.error('Login error:', error);
      throw new Error(error.message || 'Login failed. Please check your credentials.');
    }
  }

  /**
   * Register new user
   * @param {Object} userData - User registration data
   * @returns {Promise<Object>} User data and tokens
   */
  async register({ username, email, password, firstName, lastName }) {
    try {
      const response = await apiClient.post(ENV.ENDPOINTS.AUTH.REGISTER, {
        username,
        email,
        password,
        firstName: firstName || username,
        lastName: lastName || ''
      });

      // Save tokens and user data
      if (response.token) {
        apiClient.saveTokens(response.token, response.refreshToken);
        this.saveUserData(response.user || response);
      }

      return response;
    } catch (error) {
      console.error('Registration error:', error);
      throw new Error(error.message || 'Registration failed. Please try again.');
    }
  }

  /**
   * Logout user
   */
  async logout() {
    try {
      // Call backend logout endpoint
      await apiClient.post(ENV.ENDPOINTS.AUTH.LOGOUT);
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      // Always clear local data
      apiClient.clearTokens();
      this.clearUserData();
    }
  }

  /**
   * Get current user profile
   * @returns {Promise<Object>} User profile data
   */
  async getProfile() {
    try {
      const response = await apiClient.get(ENV.ENDPOINTS.AUTH.PROFILE);
      this.saveUserData(response);
      return response;
    } catch (error) {
      console.error('Get profile error:', error);
      throw new Error(error.message || 'Failed to fetch user profile.');
    }
  }

  /**
   * Refresh access token
   * @returns {Promise<Object>} New tokens
   */
  async refreshToken() {
    try {
      const refreshToken = localStorage.getItem(ENV.STORAGE_KEYS.REFRESH_TOKEN);

      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      const response = await apiClient.post(ENV.ENDPOINTS.AUTH.REFRESH_TOKEN, {
        refreshToken
      });

      if (response.token) {
        apiClient.saveTokens(response.token, response.refreshToken || refreshToken);
      }

      return response;
    } catch (error) {
      console.error('Token refresh error:', error);
      // If refresh fails, logout user
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
  }

  /**
   * Check if user is authenticated
   * @returns {boolean}
   */
  isAuthenticated() {
    const token = localStorage.getItem(ENV.STORAGE_KEYS.USER_TOKEN);
    return !!token;
  }

  /**
   * Get current user data from localStorage
   * @returns {Object|null}
   */
  getCurrentUser() {
    try {
      const userData = localStorage.getItem(ENV.STORAGE_KEYS.USER_DATA);
      return userData ? JSON.parse(userData) : null;
    } catch (error) {
      console.error('Error parsing user data:', error);
      return null;
    }
  }

  /**
   * Save user data to localStorage
   * @param {Object} userData
   */
  saveUserData(userData) {
    if (typeof window !== 'undefined') {
      localStorage.setItem(ENV.STORAGE_KEYS.USER_DATA, JSON.stringify(userData));
    }
  }

  /**
   * Clear user data from localStorage
   */
  clearUserData() {
    if (typeof window !== 'undefined') {
      localStorage.removeItem(ENV.STORAGE_KEYS.USER_DATA);
    }
  }

  /**
   * Update user password
   * @param {string} currentPassword
   * @param {string} newPassword
   * @returns {Promise<Object>}
   */
  async updatePassword(currentPassword, newPassword) {
    try {
      const response = await apiClient.post('/auth/change-password', {
        currentPassword,
        newPassword
      });
      return response;
    } catch (error) {
      console.error('Password update error:', error);
      throw new Error(error.message || 'Failed to update password.');
    }
  }

  /**
   * Request password reset
   * @param {string} email
   * @returns {Promise<Object>}
   */
  async requestPasswordReset(email) {
    try {
      const response = await apiClient.post('/auth/forgot-password', { email });
      return response;
    } catch (error) {
      console.error('Password reset request error:', error);
      throw new Error(error.message || 'Failed to request password reset.');
    }
  }

  /**
   * Reset password with token
   * @param {string} token
   * @param {string} newPassword
   * @returns {Promise<Object>}
   */
  async resetPassword(token, newPassword) {
    try {
      const response = await apiClient.post('/auth/reset-password', {
        token,
        newPassword
      });
      return response;
    } catch (error) {
      console.error('Password reset error:', error);
      throw new Error(error.message || 'Failed to reset password.');
    }
  }
}

// Export singleton instance
const authService = new AuthService();
export default authService;

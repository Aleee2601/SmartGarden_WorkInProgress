// Environment Configuration for SmartGarden React App

const ENV = {
  // API Configuration
  API_BASE_URL: process.env.REACT_APP_API_URL || 'https://localhost:5000/api',

  // API Endpoints
  ENDPOINTS: {
    // Authentication
    AUTH: {
      LOGIN: '/auth/login',
      REGISTER: '/auth/register',
      REFRESH_TOKEN: '/auth/refresh-token',
      LOGOUT: '/auth/logout',
      PROFILE: '/auth/profile'
    },

    // Device Auth
    DEVICE_AUTH: {
      PENDING: '/device-auth/pending',
      APPROVE: '/device-auth/approve',
      REGISTER: '/device-auth/register',
      HEARTBEAT: '/device-auth/heartbeat',
      REFRESH_TOKEN: '/device-auth/refresh-token'
    },

    // Plants
    PLANTS: {
      BASE: '/plants',
      GET_ALL: '/plants',
      GET_BY_ID: (id) => `/plants/${id}`,
      CREATE: '/plants',
      UPDATE: (id) => `/plants/${id}`,
      DELETE: (id) => `/plants/${id}`,
      CALIBRATION: (id) => `/plants/${id}/calibration`,
      UPDATE_CALIBRATION: (id) => `/plants/${id}/calibration`
    },

    // Sensor Readings
    SENSOR: {
      POST_READING: '/sensor',
      GET_READINGS: '/sensor/readings',
      GET_BY_PLANT: (plantId) => `/sensor/plant/${plantId}`,
      GET_LATEST: (plantId) => `/sensor/plant/${plantId}/latest`,
      GET_HISTORY: (plantId, hours) => `/sensor/plant/${plantId}/history?hours=${hours}`,
      GET_STATISTICS: (plantId, days) => `/sensor/plant/${plantId}/statistics?days=${days}`
    },

    // Watering
    WATERING: {
      MANUAL: '/watering/manual',
      SCHEDULE: '/watering/schedule',
      GET_SCHEDULES: (plantId) => `/watering/plant/${plantId}/schedules`,
      AUTO_CONFIG: (plantId) => `/watering/plant/${plantId}/auto`,
      COMMANDS: (deviceId) => `/watering/device/${deviceId}/commands`
    },

    // Devices
    DEVICES: {
      GET_ALL: '/devices',
      GET_BY_ID: (id) => `/devices/${id}`,
      UPDATE: (id) => `/devices/${id}`,
      DELETE: (id) => `/devices/${id}`
    },

    // Alerts
    ALERTS: {
      GET_ALL: '/alerts',
      GET_BY_PLANT: (plantId) => `/alerts/plant/${plantId}`,
      MARK_READ: (id) => `/alerts/${id}/read`,
      DELETE: (id) => `/alerts/${id}`
    }
  },

  // App Configuration
  APP: {
    NAME: 'Bloomly',
    VERSION: '1.0.0',
    TIMEOUT: 10000, // 10 seconds
    RETRY_ATTEMPTS: 3,
    RETRY_DELAY: 1000 // 1 second
  },

  // Storage Keys
  STORAGE_KEYS: {
    USER_TOKEN: 'smartgarden_user_token',
    REFRESH_TOKEN: 'smartgarden_refresh_token',
    USER_DATA: 'smartgarden_user_data',
    CALIBRATION_DATA: 'smartgarden_calibration_data'
  },

  // Sensor Configuration
  SENSORS: {
    READING_INTERVAL: 15 * 60 * 1000, // 15 minutes in milliseconds
    CALIBRATION_COUNTDOWN: 10, // seconds
    TYPES: {
      SOIL_MOISTURE: 'soilMoisture',
      TEMPERATURE: 'airTemperature',
      HUMIDITY: 'airHumidity',
      LIGHT: 'lightLevel',
      AIR_QUALITY: 'airQuality',
      WATER_LEVEL: 'waterLevel'
    }
  },

  // Watering Configuration
  WATERING: {
    MIN_DURATION: 1, // seconds
    MAX_DURATION: 300, // 5 minutes
    DEFAULT_DURATION: 30 // seconds
  }
};

export default ENV;

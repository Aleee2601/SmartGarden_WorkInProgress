import React, { useState, useEffect } from 'react';
import {
  LineChart,
  Line,
  AreaChart,
  Area,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  ReferenceLine,
  ReferenceArea
} from 'recharts';
import { Calendar, TrendingUp, Droplet, Sun, Thermometer, Activity, Download, RefreshCw, Loader } from 'lucide-react';
import { analyticsService } from '../api';

const PlantAnalytics = ({ plantId, plantName }) => {
  const [historicalData, setHistoricalData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [dateRange, setDateRange] = useState('7days');
  const [interval, setInterval] = useState('hourly');
  const [selectedMetrics, setSelectedMetrics] = useState(['soilMoisture', 'waterLevel']);

  const dateRangeOptions = [
    { value: '24hours', label: 'Last 24 Hours', days: 1, interval: 'hourly' },
    { value: '7days', label: 'Last 7 Days', days: 7, interval: 'hourly' },
    { value: '30days', label: 'Last 30 Days', days: 30, interval: 'daily' },
    { value: '90days', label: 'Last 90 Days', days: 90, interval: 'daily' }
  ];

  const metricOptions = [
    { value: 'soilMoisture', label: 'Soil Moisture', color: '#22c55e', icon: Droplet },
    { value: 'waterLevel', label: 'Water Level', color: '#3b82f6', icon: Droplet },
    { value: 'airTemp', label: 'Temperature', color: '#ef4444', icon: Thermometer },
    { value: 'airHumidity', label: 'Humidity', color: '#06b6d4', icon: Droplet },
    { value: 'lightLevel', label: 'Light Level', color: '#f59e0b', icon: Sun }
  ];

  useEffect(() => {
    if (plantId) {
      fetchHistoricalData();
    }
  }, [plantId, dateRange]);

  const fetchHistoricalData = async () => {
    setLoading(true);
    setError(null);

    try {
      const selectedRange = dateRangeOptions.find(r => r.value === dateRange);
      const endDate = new Date();
      const startDate = new Date();
      startDate.setDate(startDate.getDate() - selectedRange.days);

      const data = await analyticsService.getHistoricalData(
        plantId,
        startDate,
        endDate,
        selectedRange.interval
      );

      setHistoricalData(data);
      setInterval(selectedRange.interval);
    } catch (err) {
      setError(err.message || 'Failed to load analytics data');
      console.error('Error fetching analytics:', err);
    } finally {
      setLoading(false);
    }
  };

  const toggleMetric = (metric) => {
    setSelectedMetrics(prev =>
      prev.includes(metric)
        ? prev.filter(m => m !== metric)
        : [...prev, metric]
    );
  };

  const formatXAxis = (timestamp) => {
    const date = new Date(timestamp);
    if (interval === 'hourly') {
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    } else if (interval === 'daily') {
      return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
    } else {
      return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
    }
  };

  const formatChartData = () => {
    if (!historicalData?.dataPoints) return [];

    return historicalData.dataPoints.map(point => ({
      timestamp: point.timestamp,
      displayTime: formatXAxis(point.timestamp),
      soilMoisture: point.avgSoilMoisture,
      minSoilMoisture: point.minSoilMoisture,
      maxSoilMoisture: point.maxSoilMoisture,
      waterLevel: point.avgWaterLevel,
      airTemp: point.avgAirTemp,
      minAirTemp: point.minAirTemp,
      maxAirTemp: point.maxAirTemp,
      airHumidity: point.avgAirHumidity,
      lightLevel: point.avgLightLevel,
      readingCount: point.readingCount
    }));
  };

  const CustomTooltip = ({ active, payload, label }) => {
    if (active && payload && payload.length) {
      return (
        <div className="bg-white border-2 border-gray-200 rounded-lg shadow-xl p-4">
          <p className="text-sm font-semibold text-gray-800 mb-2">{label}</p>
          {payload.map((entry, index) => (
            <div key={index} className="flex items-center space-x-2 text-sm">
              <div
                className="w-3 h-3 rounded-full"
                style={{ backgroundColor: entry.color }}
              />
              <span className="text-gray-700">{entry.name}:</span>
              <span className="font-semibold text-gray-900">
                {entry.value.toFixed(1)}
                {entry.name.includes('Temp') ? '°C' : entry.name.includes('Level') || entry.name.includes('Moisture') || entry.name.includes('Humidity') ? '%' : ' lux'}
              </span>
            </div>
          ))}
        </div>
      );
    }
    return null;
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <Loader className="w-8 h-8 text-green-600 animate-spin" />
        <span className="ml-3 text-gray-600">Loading analytics...</span>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <p className="text-red-600 mb-4">{error}</p>
        <button
          onClick={fetchHistoricalData}
          className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-all"
        >
          Retry
        </button>
      </div>
    );
  }

  const chartData = formatChartData();
  const stats = historicalData?.statistics;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-800 flex items-center space-x-2">
            <TrendingUp className="w-6 h-6 text-green-600" />
            <span>Historical Analytics</span>
          </h2>
          <p className="text-gray-600 text-sm mt-1">{plantName || 'Plant'}</p>
        </div>
        <button
          onClick={fetchHistoricalData}
          className="flex items-center space-x-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-all"
        >
          <RefreshCw className="w-4 h-4" />
          <span>Refresh</span>
        </button>
      </div>

      {/* Date Range Selector */}
      <div className="bg-white rounded-lg shadow-md p-4">
        <div className="flex items-center space-x-2 mb-3">
          <Calendar className="w-5 h-5 text-gray-600" />
          <span className="font-semibold text-gray-800">Time Period</span>
        </div>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
          {dateRangeOptions.map((option) => (
            <button
              key={option.value}
              onClick={() => setDateRange(option.value)}
              className={`px-4 py-2 rounded-lg font-medium transition-all ${
                dateRange === option.value
                  ? 'bg-green-600 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              }`}
            >
              {option.label}
            </button>
          ))}
        </div>
      </div>

      {/* Metric Selector */}
      <div className="bg-white rounded-lg shadow-md p-4">
        <div className="flex items-center space-x-2 mb-3">
          <Activity className="w-5 h-5 text-gray-600" />
          <span className="font-semibold text-gray-800">Metrics to Display</span>
        </div>
        <div className="flex flex-wrap gap-2">
          {metricOptions.map((metric) => {
            const Icon = metric.icon;
            return (
              <button
                key={metric.value}
                onClick={() => toggleMetric(metric.value)}
                className={`flex items-center space-x-2 px-4 py-2 rounded-lg font-medium transition-all ${
                  selectedMetrics.includes(metric.value)
                    ? 'text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
                style={{
                  backgroundColor: selectedMetrics.includes(metric.value) ? metric.color : undefined
                }}
              >
                <Icon className="w-4 h-4" />
                <span>{metric.label}</span>
              </button>
            );
          })}
        </div>
      </div>

      {/* Statistics Summary */}
      {stats && (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <div className="bg-white rounded-lg shadow-md p-4">
            <p className="text-sm text-gray-600 mb-1">Avg Soil Moisture</p>
            <p className="text-2xl font-bold text-green-600">{stats.avgSoilMoisture.toFixed(1)}%</p>
            <p className="text-xs text-gray-500 mt-1">
              Min: {stats.minSoilMoisture.toFixed(1)}% / Max: {stats.maxSoilMoisture.toFixed(1)}%
            </p>
          </div>
          <div className="bg-white rounded-lg shadow-md p-4">
            <p className="text-sm text-gray-600 mb-1">Avg Temperature</p>
            <p className="text-2xl font-bold text-red-600">{stats.avgAirTemp.toFixed(1)}°C</p>
          </div>
          <div className="bg-white rounded-lg shadow-md p-4">
            <p className="text-sm text-gray-600 mb-1">Total Waterings</p>
            <p className="text-2xl font-bold text-blue-600">{stats.totalWaterings}</p>
          </div>
          <div className="bg-white rounded-lg shadow-md p-4">
            <p className="text-sm text-gray-600 mb-1">Total Readings</p>
            <p className="text-2xl font-bold text-gray-800">{stats.totalReadings}</p>
          </div>
        </div>
      )}

      {/* Main Chart */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <h3 className="text-lg font-semibold text-gray-800 mb-4">Sensor Trends</h3>
        <ResponsiveContainer width="100%" height={400}>
          <LineChart data={chartData}>
            <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
            <XAxis
              dataKey="displayTime"
              stroke="#6b7280"
              style={{ fontSize: '12px' }}
            />
            <YAxis
              stroke="#6b7280"
              style={{ fontSize: '12px' }}
              label={{ value: 'Value', angle: -90, position: 'insideLeft', style: { fontSize: '12px' } }}
            />
            <Tooltip content={<CustomTooltip />} />
            <Legend />

            {selectedMetrics.includes('soilMoisture') && (
              <>
                <Line
                  type="monotone"
                  dataKey="soilMoisture"
                  stroke="#22c55e"
                  strokeWidth={2}
                  dot={false}
                  name="Soil Moisture"
                />
                <Line
                  type="monotone"
                  dataKey="minSoilMoisture"
                  stroke="#22c55e"
                  strokeWidth={1}
                  strokeDasharray="5 5"
                  dot={false}
                  name="Min Soil"
                />
                <Line
                  type="monotone"
                  dataKey="maxSoilMoisture"
                  stroke="#22c55e"
                  strokeWidth={1}
                  strokeDasharray="5 5"
                  dot={false}
                  name="Max Soil"
                />
              </>
            )}

            {selectedMetrics.includes('waterLevel') && (
              <Line
                type="monotone"
                dataKey="waterLevel"
                stroke="#3b82f6"
                strokeWidth={2}
                dot={false}
                name="Water Level"
              />
            )}

            {selectedMetrics.includes('airTemp') && (
              <Line
                type="monotone"
                dataKey="airTemp"
                stroke="#ef4444"
                strokeWidth={2}
                dot={false}
                name="Temperature"
              />
            )}

            {selectedMetrics.includes('airHumidity') && (
              <Line
                type="monotone"
                dataKey="airHumidity"
                stroke="#06b6d4"
                strokeWidth={2}
                dot={false}
                name="Humidity"
              />
            )}

            {selectedMetrics.includes('lightLevel') && (
              <Line
                type="monotone"
                dataKey="lightLevel"
                stroke="#f59e0b"
                strokeWidth={2}
                dot={false}
                name="Light Level"
              />
            )}
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Watering Events Timeline */}
      {historicalData?.wateringEvents && historicalData.wateringEvents.length > 0 && (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h3 className="text-lg font-semibold text-gray-800 mb-4">Watering History</h3>
          <ResponsiveContainer width="100%" height={200}>
            <BarChart data={historicalData.wateringEvents.map(w => ({
              timestamp: formatXAxis(w.timestamp),
              duration: w.durationSec,
              mode: w.mode
            }))}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
              <XAxis dataKey="timestamp" stroke="#6b7280" style={{ fontSize: '12px' }} />
              <YAxis stroke="#6b7280" style={{ fontSize: '12px' }} label={{ value: 'Duration (sec)', angle: -90, position: 'insideLeft' }} />
              <Tooltip />
              <Legend />
              <Bar dataKey="duration" fill="#3b82f6" name="Duration (seconds)" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      )}
    </div>
  );
};

export default PlantAnalytics;

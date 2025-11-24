import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { Leaf, Droplet, Sun, Thermometer, Droplets, AlertCircle, Loader, RefreshCw } from 'lucide-react';
import { plantService } from '../api';
import ENV from '../config/env';

const Dashboard = () => {
  const [plants, setPlants] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [connectionStatus, setConnectionStatus] = useState('Disconnected');

  const connectionRef = useRef(null);

  // Fetch initial plant list
  useEffect(() => {
    fetchPlants();
  }, []);

  // Setup SignalR connection
  useEffect(() => {
    const setupSignalR = async () => {
      try {
        // Create SignalR connection
        const connection = new signalR.HubConnectionBuilder()
          .withUrl(`${ENV.API.BASE_URL}/hubs/plant`, {
            skipNegotiation: false,
            transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents,
            withCredentials: true
          })
          .withAutomaticReconnect({
            nextRetryDelayInMilliseconds: (retryContext) => {
              // Exponential backoff: 2s, 4s, 8s, 16s, 30s
              if (retryContext.elapsedMilliseconds < 60000) {
                return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
              }
              return 30000;
            }
          })
          .configureLogging(signalR.LogLevel.Information)
          .build();

        // Handle incoming real-time updates
        connection.on('ReceiveUpdate', (update) => {
          console.log('Received real-time update:', update);

          // Update specific plant in the list
          setPlants((prevPlants) =>
            prevPlants.map((plant) =>
              plant.plantId === update.plantId
                ? {
                    ...plant,
                    // Update sensor readings
                    soilMoisture: update.soilMoisture,
                    waterLevel: update.waterLevel,
                    airTemp: update.airTemp,
                    lightLevel: update.lightLevel,
                    airHumidity: update.airHumidity,
                    airQuality: update.airQuality,
                    lastUpdate: update.timestamp,
                    isWatering: update.isWatering
                  }
                : plant
            )
          );
        });

        // Connection lifecycle events
        connection.onreconnecting((error) => {
          console.warn('SignalR reconnecting:', error);
          setConnectionStatus('Reconnecting...');
        });

        connection.onreconnected((connectionId) => {
          console.log('SignalR reconnected:', connectionId);
          setConnectionStatus('Connected');
        });

        connection.onclose((error) => {
          console.error('SignalR connection closed:', error);
          setConnectionStatus('Disconnected');
        });

        // Start the connection
        await connection.start();
        console.log('SignalR connected successfully');
        setConnectionStatus('Connected');

        connectionRef.current = connection;
      } catch (err) {
        console.error('SignalR connection error:', err);
        setConnectionStatus('Connection Failed');
      }
    };

    setupSignalR();

    // Cleanup on unmount
    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop();
      }
    };
  }, []);

  const fetchPlants = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await plantService.getAllPlants();
      setPlants(response.map(plant => ({
        ...plant,
        soilMoisture: plant.latestSensorReading?.soilMoisture || 0,
        waterLevel: plant.latestSensorReading?.waterLevel || 0,
        airTemp: plant.latestSensorReading?.airTemp || 0,
        lightLevel: plant.latestSensorReading?.lightLevel || 0,
        airHumidity: plant.latestSensorReading?.airHumidity || 0,
        airQuality: plant.latestSensorReading?.airQuality || 0,
        lastUpdate: plant.latestSensorReading?.createdAt || null,
        isWatering: false
      })));
    } catch (err) {
      setError(err.message || 'Failed to load plants');
      console.error('Error fetching plants:', err);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (soilMoisture) => {
    if (soilMoisture < 20) return 'bg-red-500';
    if (soilMoisture < 40) return 'bg-orange-500';
    if (soilMoisture < 60) return 'bg-yellow-500';
    return 'bg-green-500';
  };

  const getStatusText = (soilMoisture) => {
    if (soilMoisture < 20) return 'Very Dry';
    if (soilMoisture < 40) return 'Dry';
    if (soilMoisture < 60) return 'Moderate';
    return 'Moist';
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <Loader className="w-12 h-12 text-green-600 animate-spin mx-auto mb-4" />
          <p className="text-gray-600">Loading your plants...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full">
          <AlertCircle className="w-16 h-16 text-red-500 mx-auto mb-4" />
          <h2 className="text-2xl font-bold text-gray-800 mb-2 text-center">Error Loading Plants</h2>
          <p className="text-gray-600 text-center mb-6">{error}</p>
          <button
            onClick={fetchPlants}
            className="w-full py-3 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-all flex items-center justify-center space-x-2"
          >
            <RefreshCw className="w-5 h-5" />
            <span>Retry</span>
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-4xl font-bold text-gray-800 flex items-center space-x-3">
                <Leaf className="w-10 h-10 text-green-600" />
                <span>SmartGarden Dashboard</span>
              </h1>
              <p className="text-gray-600 mt-2">Real-time plant monitoring</p>
            </div>
            <div className="text-right">
              <div className="flex items-center space-x-2">
                <div className={`w-3 h-3 rounded-full ${
                  connectionStatus === 'Connected' ? 'bg-green-500' :
                  connectionStatus === 'Reconnecting...' ? 'bg-yellow-500 animate-pulse' :
                  'bg-red-500'
                }`} />
                <span className="text-sm text-gray-600">{connectionStatus}</span>
              </div>
              <button
                onClick={fetchPlants}
                className="mt-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-all flex items-center space-x-2"
              >
                <RefreshCw className="w-4 h-4" />
                <span>Refresh</span>
              </button>
            </div>
          </div>
        </div>

        {/* Plants Grid */}
        {plants.length === 0 ? (
          <div className="bg-white rounded-lg shadow-xl p-12 text-center">
            <Leaf className="w-24 h-24 text-gray-300 mx-auto mb-6" />
            <h2 className="text-2xl font-bold text-gray-800 mb-2">No Plants Yet</h2>
            <p className="text-gray-600">Add your first plant to start monitoring!</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {plants.map((plant) => (
              <div
                key={plant.plantId}
                className={`bg-white rounded-xl shadow-lg overflow-hidden transition-all duration-300 hover:shadow-2xl ${
                  plant.isWatering ? 'ring-4 ring-blue-400 animate-pulse' : ''
                }`}
              >
                {/* Plant Header */}
                <div className="bg-gradient-to-br from-green-500 to-green-600 p-6 text-white">
                  <div className="flex items-center justify-between mb-2">
                    <h3 className="text-2xl font-bold">{plant.nickname || 'Unnamed Plant'}</h3>
                    {plant.isWatering && (
                      <Droplets className="w-8 h-8 animate-bounce" />
                    )}
                  </div>
                  <p className="text-green-100 text-sm">{plant.species?.commonName || 'Unknown Species'}</p>
                  <p className="text-green-100 text-xs mt-1">
                    {plant.roomName || 'No location'} • {plant.isOutdoor ? 'Outdoor' : 'Indoor'}
                  </p>
                </div>

                {/* Soil Moisture - Primary Metric */}
                <div className="p-6 bg-gray-50">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm font-semibold text-gray-700">Soil Moisture</span>
                    <span className={`text-xs font-bold px-2 py-1 rounded-full text-white ${getStatusColor(plant.soilMoisture)}`}>
                      {getStatusText(plant.soilMoisture)}
                    </span>
                  </div>
                  <div className="relative h-8 bg-gray-200 rounded-full overflow-hidden">
                    <div
                      className={`absolute top-0 left-0 h-full transition-all duration-500 ${getStatusColor(plant.soilMoisture)}`}
                      style={{ width: `${plant.soilMoisture}%` }}
                    />
                    <div className="absolute inset-0 flex items-center justify-center">
                      <span className="text-sm font-bold text-gray-800">{plant.soilMoisture.toFixed(1)}%</span>
                    </div>
                  </div>
                </div>

                {/* Sensor Readings */}
                <div className="p-6 grid grid-cols-2 gap-4">
                  {/* Water Level */}
                  <div className="flex items-center space-x-3">
                    <Droplet className={`w-8 h-8 ${plant.waterLevel < 20 ? 'text-red-500' : 'text-blue-500'}`} />
                    <div>
                      <p className="text-xs text-gray-600">Water Tank</p>
                      <p className="text-lg font-bold text-gray-800">{plant.waterLevel.toFixed(1)}%</p>
                    </div>
                  </div>

                  {/* Light Level */}
                  <div className="flex items-center space-x-3">
                    <Sun className="w-8 h-8 text-yellow-500" />
                    <div>
                      <p className="text-xs text-gray-600">Light</p>
                      <p className="text-lg font-bold text-gray-800">{plant.lightLevel.toFixed(0)} lux</p>
                    </div>
                  </div>

                  {/* Temperature */}
                  <div className="flex items-center space-x-3">
                    <Thermometer className="w-8 h-8 text-orange-500" />
                    <div>
                      <p className="text-xs text-gray-600">Temperature</p>
                      <p className="text-lg font-bold text-gray-800">{plant.airTemp.toFixed(1)}°C</p>
                    </div>
                  </div>

                  {/* Humidity */}
                  <div className="flex items-center space-x-3">
                    <Droplets className="w-8 h-8 text-cyan-500" />
                    <div>
                      <p className="text-xs text-gray-600">Humidity</p>
                      <p className="text-lg font-bold text-gray-800">{plant.airHumidity.toFixed(1)}%</p>
                    </div>
                  </div>
                </div>

                {/* Last Update */}
                <div className="px-6 pb-4">
                  <p className="text-xs text-gray-500 text-center">
                    Last update: {plant.lastUpdate
                      ? new Date(plant.lastUpdate).toLocaleString()
                      : 'Never'}
                  </p>
                </div>

                {/* Watering Indicator */}
                {plant.isWatering && (
                  <div className="bg-blue-500 text-white text-center py-2 font-semibold">
                    💧 Watering in progress...
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;

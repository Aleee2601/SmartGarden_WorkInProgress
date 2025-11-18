import React, { useState, useEffect } from 'react';
import {
  Leaf, Settings, ArrowLeft, Sun, Droplet, ThermometerSun,
  Sprout, Droplets, Battery, Thermometer, Menu, User, HelpCircle,
  Info, Home, Plus, Check, X, Play
} from 'lucide-react';

// Main App Component
function App() {
  const [currentScreen, setCurrentScreen] = useState('login');
  const [showMenu, setShowMenu] = useState(false);
  const [calibrationData, setCalibrationData] = useState({
    lightSensor: {
      dark: { calibrated: true, value: 0 },
      lowLight: { calibrated: true, value: 100 },
      mediumIndirect: { calibrated: true, value: 300 },
      brightIndirect: { calibrated: false, value: null },
      brightDirect: { calibrated: false, value: null }
    },
    soilMoisture: {
      dry: { calibrated: false, value: null },
      moist: { calibrated: false, value: null },
      saturated: { calibrated: false, value: null }
    },
    waterLevel: {
      empty: { calibrated: false, value: null },
      half: { calibrated: false, value: null },
      full: { calibrated: false, value: null }
    },
    temperature: {
      room: { calibrated: false, value: null }
    }
  });

  const renderScreen = () => {
    switch (currentScreen) {
      case 'login':
        return <LoginScreen onNavigate={setCurrentScreen} />;
      case 'signup':
        return <SignUpScreen onNavigate={setCurrentScreen} />;
      case 'calibration':
        return (
          <CalibrationScreen
            onNavigate={setCurrentScreen}
            calibrationData={calibrationData}
            setCalibrationData={setCalibrationData}
          />
        );
      case 'dashboard':
        return (
          <DashboardScreen
            onNavigate={setCurrentScreen}
            showMenu={showMenu}
            setShowMenu={setShowMenu}
          />
        );
      case 'plantDetail':
        return <PlantDetailScreen onNavigate={setCurrentScreen} />;
      default:
        return <LoginScreen onNavigate={setCurrentScreen} />;
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="w-full max-w-md">
        {renderScreen()}
      </div>
    </div>
  );
}

// Login Screen Component
function LoginScreen({ onNavigate }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = () => {
    onNavigate('dashboard');
  };

  return (
    <div className="bg-white rounded-3xl shadow-xl p-8 space-y-6">
      <div className="flex flex-col items-center space-y-4">
        <div className="w-24 h-24 bg-gradient-to-br from-green-400 to-green-600 rounded-full flex items-center justify-center">
          <Leaf className="w-16 h-16 text-white" />
        </div>
        <h1 className="text-4xl font-bold text-gray-800">Bloomly</h1>
      </div>

      <div className="space-y-4">
        <h2 className="text-2xl font-semibold text-gray-800">Welcome Back</h2>

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full px-4 py-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full px-4 py-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
        />

        <button
          onClick={handleLogin}
          className="w-full py-3 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg font-semibold hover:from-green-600 hover:to-green-700 transition-all transform hover:scale-105"
        >
          Login
        </button>
      </div>

      <div className="text-center">
        <button
          onClick={() => onNavigate('signup')}
          className="text-green-600 hover:text-green-700 font-medium"
        >
          Create new account
        </button>
      </div>
    </div>
  );
}

// Sign Up Screen Component
function SignUpScreen({ onNavigate }) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleSignUp = () => {
    onNavigate('calibration');
  };

  return (
    <div className="bg-white rounded-3xl shadow-xl p-8 space-y-6">
      <div className="flex flex-col items-center space-y-4">
        <div className="w-24 h-24 bg-gradient-to-br from-green-400 to-green-600 rounded-full flex items-center justify-center">
          <Leaf className="w-16 h-16 text-white" />
        </div>
        <h1 className="text-4xl font-bold text-gray-800">Bloomly</h1>
      </div>

      <div className="space-y-4">
        <h2 className="text-2xl font-semibold text-gray-800">Create new account</h2>

        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          className="w-full px-4 py-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
        />

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full px-4 py-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full px-4 py-3 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-green-500"
        />

        <button
          onClick={handleSignUp}
          className="w-full py-3 bg-gray-400 text-white rounded-lg font-semibold hover:bg-gray-500 transition-all"
        >
          Create new account
        </button>
      </div>

      <div className="text-center">
        <button
          onClick={() => onNavigate('login')}
          className="text-green-600 hover:text-green-700 font-medium"
        >
          Login
        </button>
      </div>
    </div>
  );
}

// Calibration Screen Component
function CalibrationScreen({ onNavigate, calibrationData, setCalibrationData }) {
  const [currentSensor, setCurrentSensor] = useState('light');
  const [showModal, setShowModal] = useState(false);
  const [activeCalibration, setActiveCalibration] = useState(null);

  const sensorTypes = [
    { id: 'light', name: 'Light Sensor', icon: Sun },
    { id: 'soilMoisture', name: 'Soil Moisture', icon: Droplets },
    { id: 'waterLevel', name: 'Water Level', icon: Droplet },
    { id: 'temperature', name: 'Temperature', icon: Thermometer }
  ];

  const lightSteps = [
    { key: 'dark', label: 'Dark', instruction: 'Cover the sensor completely' },
    { key: 'lowLight', label: 'Low Light', instruction: 'Place sensor in dim room' },
    { key: 'mediumIndirect', label: 'Medium Indirect', instruction: 'Place near window, no direct sun' },
    { key: 'brightIndirect', label: 'Bright Indirect', instruction: 'Place in well-lit room, indirect light' },
    { key: 'brightDirect', label: 'Bright Direct', instruction: 'Place in direct sunlight' }
  ];

  const soilSteps = [
    { key: 'dry', label: 'Dry Soil', instruction: 'Insert sensor into completely dry soil' },
    { key: 'moist', label: 'Moist Soil', instruction: 'Insert sensor into moist soil' },
    { key: 'saturated', label: 'Saturated Soil (Wet)', instruction: 'Insert sensor into very wet soil' }
  ];

  const waterSteps = [
    { key: 'empty', label: 'Empty Tank (0%)', instruction: 'Place sensor in empty water tank' },
    { key: 'half', label: 'Half Full (50%)', instruction: 'Fill tank to 50% and place sensor' },
    { key: 'full', label: 'Full Tank (100%)', instruction: 'Fill tank completely and place sensor' }
  ];

  const temperatureSteps = [
    { key: 'room', label: 'Room Temperature', instruction: 'Ensure sensor is at room temperature (18-25°C)' }
  ];

  const getStepsForSensor = (sensorType) => {
    switch (sensorType) {
      case 'light': return lightSteps;
      case 'soilMoisture': return soilSteps;
      case 'waterLevel': return waterSteps;
      case 'temperature': return temperatureSteps;
      default: return [];
    }
  };

  const startCalibration = (sensorType, stepKey, instruction) => {
    setActiveCalibration({ sensorType, stepKey, instruction });
    setShowModal(true);
  };

  const completeCalibration = (sensorType, stepKey, value) => {
    setCalibrationData(prev => ({
      ...prev,
      [sensorType]: {
        ...prev[sensorType],
        [stepKey]: { calibrated: true, value }
      }
    }));
  };

  const calculateProgress = () => {
    let total = 0;
    let completed = 0;

    Object.values(calibrationData).forEach(sensorData => {
      Object.values(sensorData).forEach(step => {
        total++;
        if (step.calibrated) completed++;
      });
    });

    return (completed / total) * 100;
  };

  const isAllCalibrated = () => {
    return Object.values(calibrationData).every(sensorData =>
      Object.values(sensorData).every(step => step.calibrated)
    );
  };

  const progress = calculateProgress();

  return (
    <div className="bg-white rounded-3xl shadow-xl overflow-hidden">
      {/* Progress Bar */}
      <div className="bg-gray-100 p-4">
        <div className="w-full bg-gray-300 rounded-full h-3">
          <div
            className="bg-gradient-to-r from-green-400 to-green-600 h-3 rounded-full transition-all duration-500"
            style={{ width: `${progress}%` }}
          />
        </div>
        <p className="text-sm text-gray-600 mt-2 text-center">
          {Math.round(progress)}% Complete
        </p>
      </div>

      {/* Header */}
      <div className="p-6 border-b">
        <h1 className="text-2xl font-bold text-gray-800">Calibrating Your Sensors</h1>
        <p className="text-gray-600 mt-1">Complete all steps for accurate readings</p>
      </div>

      {/* Sensor Tabs */}
      <div className="flex overflow-x-auto border-b">
        {sensorTypes.map((sensor) => {
          const Icon = sensor.icon;
          return (
            <button
              key={sensor.id}
              onClick={() => setCurrentSensor(sensor.id)}
              className={`flex-1 min-w-fit px-4 py-3 flex items-center justify-center space-x-2 transition-all ${
                currentSensor === sensor.id
                  ? 'bg-green-500 text-white'
                  : 'bg-white text-gray-600 hover:bg-gray-50'
              }`}
            >
              <Icon className="w-5 h-5" />
              <span className="text-sm font-medium whitespace-nowrap">{sensor.name}</span>
            </button>
          );
        })}
      </div>

      {/* Calibration Steps */}
      <div className="p-6 space-y-4 max-h-96 overflow-y-auto">
        {getStepsForSensor(currentSensor).map((step, index) => {
          const isCalibrated = calibrationData[currentSensor][step.key].calibrated;

          return (
            <div
              key={step.key}
              className="border rounded-lg p-4 flex items-center justify-between hover:shadow-md transition-all"
            >
              <div className="flex items-center space-x-3">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center ${
                  isCalibrated ? 'bg-green-500' : 'bg-gray-200'
                }`}>
                  {isCalibrated ? (
                    <Check className="w-5 h-5 text-white" />
                  ) : (
                    <span className="text-sm font-semibold text-gray-600">{index + 1}</span>
                  )}
                </div>
                <div>
                  <p className="font-medium text-gray-800">{step.label}</p>
                  {isCalibrated && (
                    <span className="text-xs text-green-600 font-medium">Completed</span>
                  )}
                </div>
              </div>

              {isCalibrated ? (
                <button
                  onClick={() => startCalibration(currentSensor, step.key, step.instruction)}
                  className="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg text-sm hover:bg-gray-300 transition-all"
                >
                  Recalibrate
                </button>
              ) : (
                <button
                  onClick={() => startCalibration(currentSensor, step.key, step.instruction)}
                  className="px-4 py-2 bg-orange-500 text-white rounded-lg text-sm hover:bg-orange-600 transition-all flex items-center space-x-2"
                >
                  <Play className="w-4 h-4" />
                  <span>Calibrate</span>
                </button>
              )}
            </div>
          );
        })}
      </div>

      {/* Guide Image Section */}
      <div className="p-6 bg-gray-50 border-t">
        <div className="bg-white rounded-lg p-4 border-2 border-dashed border-gray-300">
          <p className="text-sm text-gray-600 text-center">
            {currentSensor === 'light' && '💡 Ensure sensor is exposed to different light conditions'}
            {currentSensor === 'soilMoisture' && '🌱 Use actual soil samples at different moisture levels'}
            {currentSensor === 'waterLevel' && '💧 Use a transparent container to verify water levels'}
            {currentSensor === 'temperature' && '🌡️ Wait for sensor to stabilize at room temperature'}
          </p>
        </div>
      </div>

      {/* Navigation Buttons */}
      <div className="p-6 border-t flex justify-between">
        <button
          onClick={() => onNavigate('signup')}
          className="px-6 py-3 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition-all"
        >
          Skip for Now
        </button>

        {isAllCalibrated() && (
          <button
            onClick={() => onNavigate('dashboard')}
            className="px-6 py-3 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg hover:from-green-600 hover:to-green-700 transition-all flex items-center space-x-2"
          >
            <Check className="w-5 h-5" />
            <span>Complete Setup</span>
          </button>
        )}
      </div>

      {/* Calibration Modal */}
      {showModal && activeCalibration && (
        <CalibrationModal
          sensorType={activeCalibration.sensorType}
          stepKey={activeCalibration.stepKey}
          instruction={activeCalibration.instruction}
          onComplete={(value) => {
            completeCalibration(activeCalibration.sensorType, activeCalibration.stepKey, value);
            setShowModal(false);
            setActiveCalibration(null);
          }}
          onCancel={() => {
            setShowModal(false);
            setActiveCalibration(null);
          }}
        />
      )}
    </div>
  );
}

// Calibration Modal Component
function CalibrationModal({ sensorType, stepKey, instruction, onComplete, onCancel }) {
  const [countdown, setCountdown] = useState(10);
  const [isCalibrating, setIsCalibrating] = useState(false);

  useEffect(() => {
    if (isCalibrating && countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    } else if (isCalibrating && countdown === 0) {
      // Generate random calibration value
      const randomValue = Math.floor(Math.random() * 1000);
      onComplete(randomValue);
    }
  }, [isCalibrating, countdown, onComplete]);

  const startCalibrating = () => {
    setIsCalibrating(true);
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-2xl shadow-2xl p-6 max-w-sm w-full">
        <div className="flex justify-between items-start mb-4">
          <h3 className="text-xl font-bold text-gray-800">Calibrating Sensor</h3>
          <button
            onClick={onCancel}
            className="text-gray-400 hover:text-gray-600"
          >
            <X className="w-6 h-6" />
          </button>
        </div>

        <div className="space-y-6">
          {/* Instruction */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <p className="text-sm text-blue-800 font-medium">{instruction}</p>
          </div>

          {/* Countdown Circle */}
          {isCalibrating ? (
            <div className="flex flex-col items-center space-y-4">
              <div className="relative w-32 h-32">
                <svg className="transform -rotate-90 w-32 h-32">
                  <circle
                    cx="64"
                    cy="64"
                    r="56"
                    stroke="#e5e7eb"
                    strokeWidth="8"
                    fill="none"
                  />
                  <circle
                    cx="64"
                    cy="64"
                    r="56"
                    stroke="#22c55e"
                    strokeWidth="8"
                    fill="none"
                    strokeDasharray={`${2 * Math.PI * 56}`}
                    strokeDashoffset={`${2 * Math.PI * 56 * (countdown / 10)}`}
                    className="transition-all duration-1000"
                  />
                </svg>
                <div className="absolute inset-0 flex items-center justify-center">
                  <span className="text-3xl font-bold text-gray-800">{countdown}</span>
                </div>
              </div>
              <p className="text-gray-600 text-sm">Calibrating...</p>
            </div>
          ) : (
            <div className="flex flex-col items-center space-y-4">
              <div className="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center">
                <Play className="w-10 h-10 text-green-600" />
              </div>
              <button
                onClick={startCalibrating}
                className="w-full py-3 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg font-semibold hover:from-green-600 hover:to-green-700 transition-all"
              >
                Start Calibration
              </button>
            </div>
          )}

          {/* Cancel Button */}
          <button
            onClick={onCancel}
            className="w-full py-2 text-gray-600 hover:text-gray-800 font-medium"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

// Dashboard Screen Component
function DashboardScreen({ onNavigate, showMenu, setShowMenu }) {
  const plants = [
    {
      id: 1,
      name: 'My Ficus',
      scientificName: 'Ficus lyrata',
      type: 'INDOOR',
      image: '🌿',
      needsAttention: false
    }
  ];

  const tips = [
    { title: 'Watering Rule', icon: '💧', color: 'gray', text: 'Water when top 2 inches of soil is dry' },
    { title: 'Light Check', icon: '☀️', color: 'green', text: 'Most plants need 6-8 hours of light' },
    { title: 'Low Light Champions', icon: '🌱', color: 'green', text: 'Snake plants thrive in low light' },
    { title: 'Finger Test', icon: '👆', color: 'gray', text: 'Stick finger in soil to check moisture' }
  ];

  return (
    <div className="bg-white rounded-3xl shadow-xl overflow-hidden">
      {/* Header */}
      <div className="bg-gradient-to-r from-green-500 to-green-600 p-6">
        <div className="flex items-center justify-between">
          <div className="flex items-center space-x-3">
            <button
              onClick={() => setShowMenu(!showMenu)}
              className="text-white hover:bg-white hover:bg-opacity-20 p-2 rounded-lg transition-all"
            >
              <Menu className="w-6 h-6" />
            </button>
            <div className="flex items-center space-x-3">
              <div className="w-12 h-12 bg-white rounded-full flex items-center justify-center">
                <User className="w-6 h-6 text-green-600" />
              </div>
              <div>
                <p className="text-white font-semibold">Alexandra</p>
                <p className="text-green-100 text-sm">Plant Parent</p>
              </div>
            </div>
          </div>
          <button className="text-white hover:bg-white hover:bg-opacity-20 p-2 rounded-lg transition-all">
            <Settings className="w-6 h-6" />
          </button>
        </div>
      </div>

      {/* Menu Sidebar */}
      {showMenu && (
        <div className="fixed inset-0 bg-black bg-opacity-50 z-50" onClick={() => setShowMenu(false)}>
          <div
            className="absolute left-0 top-0 bottom-0 w-64 bg-white shadow-2xl p-6 space-y-4"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-gray-800">Menu</h3>
              <button onClick={() => setShowMenu(false)}>
                <X className="w-6 h-6 text-gray-600" />
              </button>
            </div>

            <button className="w-full flex items-center space-x-3 p-3 hover:bg-gray-100 rounded-lg transition-all">
              <User className="w-5 h-5 text-gray-600" />
              <span className="text-gray-800">Profile</span>
            </button>

            <button className="w-full flex items-center space-x-3 p-3 hover:bg-gray-100 rounded-lg transition-all">
              <Plus className="w-5 h-5 text-gray-600" />
              <span className="text-gray-800">Add a new plant</span>
            </button>

            <button
              onClick={() => {
                setShowMenu(false);
                onNavigate('calibration');
              }}
              className="w-full flex items-center space-x-3 p-3 hover:bg-gray-100 rounded-lg transition-all"
            >
              <Settings className="w-5 h-5 text-gray-600" />
              <span className="text-gray-800">Calibrate sensors</span>
            </button>

            <button className="w-full flex items-center space-x-3 p-3 hover:bg-gray-100 rounded-lg transition-all">
              <HelpCircle className="w-5 h-5 text-gray-600" />
              <span className="text-gray-800">Help</span>
            </button>

            <button className="w-full flex items-center space-x-3 p-3 hover:bg-gray-100 rounded-lg transition-all">
              <Info className="w-5 h-5 text-gray-600" />
              <span className="text-gray-800">About Us</span>
            </button>
          </div>
        </div>
      )}

      {/* Plants Section */}
      <div className="p-6 space-y-4">
        <h2 className="text-xl font-bold text-gray-800">My Plants</h2>

        {plants.map((plant) => (
          <div
            key={plant.id}
            onClick={() => onNavigate('plantDetail')}
            className="bg-gradient-to-br from-green-400 to-green-600 rounded-2xl p-6 cursor-pointer hover:shadow-lg transition-all transform hover:scale-105"
          >
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="text-6xl">{plant.image}</div>
                <div>
                  <h3 className="text-white font-bold text-lg">{plant.name}</h3>
                  <p className="text-green-100 text-sm italic">{plant.scientificName}</p>
                  <span className="inline-block mt-2 px-3 py-1 bg-white bg-opacity-30 text-white text-xs font-semibold rounded-full">
                    {plant.type}
                  </span>
                </div>
              </div>
              {plant.needsAttention && (
                <div className="w-3 h-3 bg-red-500 rounded-full animate-pulse" />
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Tips & Tricks */}
      <div className="p-6 space-y-4">
        <h2 className="text-xl font-bold text-gray-800">Tips & Tricks</h2>

        <div className="grid grid-cols-2 gap-3">
          {tips.map((tip, index) => (
            <div
              key={index}
              className={`${
                tip.color === 'green' ? 'bg-green-500' : 'bg-gray-400'
              } rounded-xl p-4 text-white cursor-pointer hover:shadow-lg transition-all transform hover:scale-105`}
            >
              <div className="text-3xl mb-2">{tip.icon}</div>
              <h4 className="font-semibold text-sm mb-1">{tip.title}</h4>
              <p className="text-xs opacity-90">{tip.text}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

// Plant Detail Screen Component
function PlantDetailScreen({ onNavigate }) {
  const [autoWatering, setAutoWatering] = useState(true);
  const [wateringIntensity, setWateringIntensity] = useState(3);

  const sensorData = [
    { label: 'Water tank', value: 10, unit: '%', icon: Battery, warning: true },
    { label: 'Light', value: 78, unit: '%', icon: Sun, warning: false },
    { label: 'Temperature', value: 24, unit: '°C', icon: ThermometerSun, warning: false },
    { label: 'Soil Moisture', value: 46, unit: '%', icon: Droplets, warning: false }
  ];

  const weeklyData = [
    { day: 'Mo', light: 80, water: 30, temp: 60, soil: 45 },
    { day: 'Tu', light: 75, water: 0, temp: 65, soil: 40 },
    { day: 'We', light: 85, water: 35, temp: 62, soil: 48 },
    { day: 'Th', light: 70, water: 0, temp: 58, soil: 42 },
    { day: 'Fr', light: 90, water: 40, temp: 70, soil: 50 },
    { day: 'Sa', light: 95, water: 0, temp: 68, soil: 46 },
    { day: 'Su', light: 88, water: 38, temp: 65, soil: 49 }
  ];

  return (
    <div className="bg-white rounded-3xl shadow-xl overflow-hidden">
      {/* Header */}
      <div className="bg-gradient-to-br from-green-400 to-green-600 p-6">
        <button
          onClick={() => onNavigate('dashboard')}
          className="text-white hover:bg-white hover:bg-opacity-20 p-2 rounded-lg transition-all inline-flex items-center space-x-2 mb-4"
        >
          <ArrowLeft className="w-5 h-5" />
          <span>Back</span>
        </button>

        <div className="flex items-start space-x-4">
          <div className="text-7xl">🌿</div>
          <div className="flex-1 text-white">
            <h1 className="text-2xl font-bold">My Ficus</h1>
            <p className="text-green-100 italic">Ficus lyrata</p>
            <div className="mt-3 flex items-center space-x-4 text-sm">
              <div className="flex items-center space-x-1">
                <span>🕐</span>
                <span>26 weeks</span>
              </div>
              <div className="flex items-center space-x-1">
                <Droplet className="w-4 h-4" />
                <span>19%</span>
              </div>
              <div className="flex items-center space-x-1">
                <Sprout className="w-4 h-4" />
                <span>86%</span>
              </div>
            </div>
            <p className="mt-2 text-sm">Next watering: <span className="font-semibold">36 min</span></p>
          </div>
        </div>
      </div>

      {/* Alert Banner */}
      <div className="bg-red-500 text-white p-3 flex items-center space-x-2">
        <span className="text-lg">⚠️</span>
        <span className="font-semibold">Please fill the water tank!</span>
      </div>

      {/* Sensor Stats */}
      <div className="p-6 space-y-4">
        <h2 className="text-lg font-bold text-gray-800">Sensor Readings</h2>

        <div className="grid grid-cols-2 gap-3">
          {sensorData.map((sensor, index) => {
            const Icon = sensor.icon;
            return (
              <div
                key={index}
                className={`rounded-xl p-4 ${
                  sensor.warning ? 'bg-red-50 border-2 border-red-300' : 'bg-gray-50'
                }`}
              >
                <div className="flex items-center justify-between mb-2">
                  <Icon className={`w-5 h-5 ${sensor.warning ? 'text-red-600' : 'text-gray-600'}`} />
                  {sensor.warning && (
                    <span className="text-xs bg-red-500 text-white px-2 py-1 rounded-full font-semibold">
                      LOW
                    </span>
                  )}
                </div>
                <p className="text-xs text-gray-600 mb-1">{sensor.label}</p>
                <p className={`text-2xl font-bold ${sensor.warning ? 'text-red-600' : 'text-gray-800'}`}>
                  {sensor.value}{sensor.unit}
                </p>
              </div>
            );
          })}
        </div>
      </div>

      {/* Automatic Watering */}
      <div className="p-6 space-y-4 border-t">
        <h2 className="text-lg font-bold text-gray-800">Automatic Watering</h2>

        <div className="flex items-center justify-between p-4 bg-gray-50 rounded-xl">
          <span className="text-gray-700 font-medium">Auto Mode</span>
          <div className="flex items-center space-x-2 bg-white rounded-full p-1 shadow-inner">
            <button
              onClick={() => setAutoWatering(false)}
              className={`px-4 py-2 rounded-full text-sm font-semibold transition-all ${
                !autoWatering ? 'bg-gray-700 text-white' : 'text-gray-600'
              }`}
            >
              OFF
            </button>
            <button
              onClick={() => setAutoWatering(true)}
              className={`px-4 py-2 rounded-full text-sm font-semibold transition-all ${
                autoWatering ? 'bg-green-500 text-white' : 'text-gray-600'
              }`}
            >
              ON
            </button>
          </div>
        </div>

        {autoWatering && (
          <div className="space-y-2">
            <label className="text-sm text-gray-600">Watering Frequency</label>
            <div className="flex items-center space-x-2">
              {[1, 2, 3, 4, 5].map((level) => (
                <button
                  key={level}
                  onClick={() => setWateringIntensity(level)}
                  className={`w-8 h-8 rounded-full ${
                    level <= wateringIntensity ? 'bg-green-500' : 'bg-gray-200'
                  } transition-all`}
                />
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Weekly Stats */}
      <div className="p-6 space-y-4 border-t">
        <h2 className="text-lg font-bold text-gray-800">Weekly Statistics</h2>

        <div className="bg-gray-50 rounded-xl p-4">
          <div className="flex items-end justify-between h-48 space-x-2">
            {weeklyData.map((day, index) => (
              <div key={index} className="flex-1 flex flex-col items-center space-y-1">
                <div className="w-full bg-gray-200 rounded-t-lg relative" style={{ height: '150px' }}>
                  <div
                    className="absolute bottom-0 w-full bg-green-500 rounded-t-lg"
                    style={{ height: `${(day.light / 100) * 150}px` }}
                  />
                  <div
                    className="absolute bottom-0 w-full bg-blue-400"
                    style={{ height: `${(day.water / 100) * 150}px` }}
                  />
                  <div
                    className="absolute bottom-0 w-full bg-orange-400"
                    style={{ height: `${(day.temp / 100) * 150}px` }}
                  />
                  <div
                    className="absolute bottom-0 w-full bg-amber-600"
                    style={{ height: `${(day.soil / 100) * 150}px` }}
                  />
                </div>
                <span className="text-xs font-semibold text-gray-600">{day.day}</span>
              </div>
            ))}
          </div>

          {/* Legend */}
          <div className="grid grid-cols-2 gap-2 mt-4">
            <div className="flex items-center space-x-2">
              <div className="w-4 h-4 bg-green-500 rounded" />
              <span className="text-xs text-gray-600">Light</span>
            </div>
            <div className="flex items-center space-x-2">
              <div className="w-4 h-4 bg-blue-400 rounded" />
              <span className="text-xs text-gray-600">Water</span>
            </div>
            <div className="flex items-center space-x-2">
              <div className="w-4 h-4 bg-orange-400 rounded" />
              <span className="text-xs text-gray-600">Temperature</span>
            </div>
            <div className="flex items-center space-x-2">
              <div className="w-4 h-4 bg-amber-600 rounded" />
              <span className="text-xs text-gray-600">Soil Moisture</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default App;

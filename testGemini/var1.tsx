import React, { useState, useEffect, useRef, useMemo } from 'react';
import { 
  Leaf, Droplets, Sun, Thermometer, Wind, Plus, Search, 
  Settings, BarChart3, Home, LogOut, Bell, Check, 
  AlertTriangle, Smartphone, Wifi, Trash2, X, Menu,
  Sparkles, MessageSquare, Loader2, Send
} from 'lucide-react';
import { 
  LineChart, Line, AreaChart, Area, XAxis, YAxis, CartesianGrid, 
  Tooltip, ResponsiveContainer, Legend 
} from 'recharts';

/**
 * GEMINI API CONFIGURATION
 */
const apiKey = ""; // The execution environment provides the key at runtime

const callGemini = async (prompt) => {
  try {
    const response = await fetch(
      `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-preview-09-2025:generateContent?key=${apiKey}`,
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          contents: [{ parts: [{ text: prompt }] }],
        }),
      }
    );

    const data = await response.json();
    if (data.error) throw new Error(data.error.message);
    return data.candidates?.[0]?.content?.parts?.[0]?.text || "I couldn't generate a response.";
  } catch (error) {
    console.error("Gemini API Error:", error);
    return "Sorry, I'm having trouble connecting to the botanical database right now.";
  }
};

/**
 * MOCK DATA & CONFIGURATION
 */
const MOCK_PLANT_DB = [
  { id: 101, common_name: "Basil", scientific_name: "Ocimum basilicum", moisture_guide: "Frequent", suggested_threshold: 60, image: "🌿" },
  { id: 102, common_name: "Snake Plant", scientific_name: "Sansevieria trifasciata", moisture_guide: "Minimal", suggested_threshold: 20, image: "🌵" },
  { id: 103, common_name: "Monstera", scientific_name: "Monstera deliciosa", moisture_guide: "Average", suggested_threshold: 40, image: "🍃" },
  { id: 104, common_name: "Tomato", scientific_name: "Solanum lycopersicum", moisture_guide: "Average", suggested_threshold: 45, image: "🍅" },
  { id: 105, common_name: "Peace Lily", scientific_name: "Spathiphyllum", moisture_guide: "Frequent", suggested_threshold: 65, image: "🌸" },
  { id: 106, common_name: "Aloe Vera", scientific_name: "Aloe barbadensis miller", moisture_guide: "Minimal", suggested_threshold: 15, image: "🪴" },
];

const TIPS = [
  "Water when the top 2 inches of soil are dry.",
  "Snake plants thrive on neglect—don't overwater!",
  "Yellow leaves often mean overwatering, brown means dry.",
  "Most houseplants prefer bright, indirect sunlight.",
];

// Utility to generate mock history data
const generateHistory = (points = 24) => {
  return Array.from({ length: points }).map((_, i) => ({
    time: `${i}:00`,
    moisture: 40 + Math.random() * 30,
    temp: 20 + Math.random() * 5,
    light: i > 6 && i < 20 ? 500 + Math.random() * 1000 : 50,
  }));
};

/**
 * MAIN APP COMPONENT
 */
export default function App() {
  // -- Global State --
  const [user, setUser] = useState(null);
  const [view, setView] = useState('login'); // login, dashboard, detail, add, calibrate, doctor
  const [plants, setPlants] = useState([]);
  const [activePlantId, setActivePlantId] = useState(null);
  const [notifications, setNotifications] = useState([]);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  
  // -- Simulation State --
  useEffect(() => {
    if (!user) return;

    const interval = setInterval(() => {
      setPlants(prevPlants => prevPlants.map(plant => {
        // 1. Simulate Moisture Decay
        let newMoisture = plant.sensorData.soilMoisture - (0.05 + Math.random() * 0.1); 
        
        // 2. Simulate Watering Actuation
        let isWatering = plant.status.isWatering;
        if (isWatering) {
          newMoisture += 5.0; 
          if (newMoisture >= plant.thresholds.max) {
            isWatering = false; 
          }
        } else if (plant.settings.autoWater && newMoisture < plant.thresholds.min) {
          isWatering = true; 
          addNotification(`Auto-watering triggered for ${plant.nickname}`);
        }

        // 3. Simulate other environmental factors
        const time = Date.now();
        const flicker = Math.random() * 2 - 1;

        return {
          ...plant,
          status: { ...plant.status, isWatering },
          sensorData: {
            ...plant.sensorData,
            soilMoisture: Math.min(100, Math.max(0, newMoisture)),
            temp: 22 + Math.sin(time / 10000) * 2 + flicker,
            humidity: 60 + flicker,
            light: 800 + (Math.sin(time / 5000) * 200),
            tankLevel: isWatering ? Math.max(0, plant.sensorData.tankLevel - 0.5) : plant.sensorData.tankLevel
          }
        };
      }));
    }, 1000); 

    return () => clearInterval(interval);
  }, [user]);

  // -- Persistence --
  useEffect(() => {
    const saved = localStorage.getItem('smartgarden_plants');
    if (saved) setPlants(JSON.parse(saved));
  }, []);

  useEffect(() => {
    localStorage.setItem('smartgarden_plants', JSON.stringify(plants));
  }, [plants]);

  // -- Actions --
  const addNotification = (msg) => {
    setNotifications(prev => [{ id: Date.now(), msg, read: false }, ...prev]);
  };

  const handleLogin = (e) => {
    e.preventDefault();
    setUser({ name: "Gardener", email: "user@smartgarden.io" });
    setView(plants.length === 0 ? 'add' : 'dashboard');
  };

  const handleLogout = () => {
    setUser(null);
    setView('login');
  };

  const deletePlant = (id) => {
    if(confirm("Are you sure you want to remove this plant?")) {
      setPlants(prev => prev.filter(p => p.id !== id));
      setView('dashboard');
    }
  };

  const getActivePlant = () => plants.find(p => p.id === activePlantId);

  // -- Render Router --
  const renderContent = () => {
    switch(view) {
      case 'login': return <LoginScreen onLogin={handleLogin} />;
      case 'dashboard': return <Dashboard plants={plants} onViewPlant={(id) => { setActivePlantId(id); setView('detail'); }} onAdd={() => setView('add')} />;
      case 'detail': return <PlantDetail plant={getActivePlant()} onBack={() => setView('dashboard')} onDelete={deletePlant} setPlants={setPlants} />;
      case 'add': return <AddPlantWizard onCancel={() => setView('dashboard')} onComplete={(p) => { setPlants(prev => [...prev, p]); setView('calibrate'); setActivePlantId(p.id); }} />;
      case 'calibrate': return <CalibrationScreen onComplete={() => setView('dashboard')} />;
      case 'doctor': return <AiDoctorView />;
      default: return <Dashboard plants={plants} />;
    }
  };

  if (!user) return <LoginScreen onLogin={handleLogin} />;

  return (
    <div className="flex h-screen bg-slate-50 font-sans text-slate-800 overflow-hidden">
      {/* Sidebar (Desktop) */}
      <aside className={`fixed inset-y-0 left-0 z-50 w-64 bg-emerald-900 text-white transform transition-transform duration-200 ease-in-out md:relative md:translate-x-0 ${isSidebarOpen ? 'translate-x-0' : '-translate-x-full'}`}>
        <div className="p-6 flex items-center space-x-3 border-b border-emerald-800">
          <div className="w-10 h-10 bg-emerald-500 rounded-lg flex items-center justify-center">
            <Leaf className="text-white" />
          </div>
          <div>
            <h1 className="text-xl font-bold tracking-tight">SmartGarden</h1>
            <p className="text-xs text-emerald-300 opacity-80">v2.1 AI Enabled</p>
          </div>
        </div>

        <nav className="p-4 space-y-2">
          <NavItem icon={<Home />} label="Dashboard" active={view === 'dashboard'} onClick={() => { setView('dashboard'); setIsSidebarOpen(false); }} />
          <NavItem icon={<Plus />} label="Add Plant" active={view === 'add'} onClick={() => { setView('add'); setIsSidebarOpen(false); }} />
          <NavItem icon={<Sparkles />} label="AI Plant Doctor" active={view === 'doctor'} onClick={() => { setView('doctor'); setIsSidebarOpen(false); }} />
          <NavItem icon={<Settings />} label="Calibration" active={view === 'calibrate'} onClick={() => { setView('calibrate'); setIsSidebarOpen(false); }} />
        </nav>

        <div className="absolute bottom-0 w-full p-6 border-t border-emerald-800">
          <div className="flex items-center space-x-3 mb-4">
            <div className="w-10 h-10 rounded-full bg-emerald-700 flex items-center justify-center text-sm font-bold">
              {user.name.charAt(0)}
            </div>
            <div>
              <p className="text-sm font-medium">{user.name}</p>
              <p className="text-xs text-emerald-400">Online</p>
            </div>
          </div>
          <button onClick={handleLogout} className="flex items-center space-x-2 text-emerald-300 hover:text-white transition-colors text-sm">
            <LogOut size={16} /> <span>Sign Out</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col h-full overflow-hidden">
        {/* Header */}
        <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-4 md:px-8">
          <button onClick={() => setIsSidebarOpen(true)} className="md:hidden text-slate-500 hover:text-emerald-600">
            <Menu />
          </button>
          <div className="flex-1 px-4">
          </div>
          <div className="flex items-center space-x-4">
             <div className="relative">
                <Bell className="text-slate-400 hover:text-emerald-600 cursor-pointer" size={20} />
                {notifications.length > 0 && (
                  <span className="absolute -top-1 -right-1 w-2 h-2 bg-red-500 rounded-full animate-pulse"></span>
                )}
             </div>
          </div>
        </header>

        {/* Scrollable Content Area */}
        <main className="flex-1 overflow-y-auto p-4 md:p-8">
          <div className="max-w-6xl mx-auto h-full">
            {renderContent()}
          </div>
        </main>
      </div>
    </div>
  );
}

/**
 * SUB-COMPONENTS
 */

const NavItem = ({ icon, label, active, onClick }) => (
  <button 
    onClick={onClick}
    className={`w-full flex items-center space-x-3 px-4 py-3 rounded-xl transition-all ${
      active ? 'bg-emerald-800 text-white shadow-lg' : 'text-emerald-100 hover:bg-emerald-800/50 hover:text-white'
    }`}
  >
    {React.cloneElement(icon, { size: 20 })}
    <span className="font-medium">{label}</span>
  </button>
);

const LoginScreen = ({ onLogin }) => (
  <div className="min-h-screen bg-emerald-900 flex items-center justify-center p-4">
    <div className="bg-white w-full max-w-md rounded-2xl shadow-2xl overflow-hidden">
      <div className="h-32 bg-emerald-600 flex items-center justify-center">
        <div className="w-16 h-16 bg-white rounded-2xl flex items-center justify-center shadow-lg transform rotate-3">
          <Leaf className="text-emerald-600 w-10 h-10" />
        </div>
      </div>
      <div className="p-8">
        <h2 className="text-2xl font-bold text-center text-slate-800 mb-2">Welcome Back</h2>
        <p className="text-center text-slate-500 mb-8">Sign in to monitor your SmartGarden</p>
        <form onSubmit={onLogin} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Email</label>
            <input type="email" defaultValue="user@smartgarden.io" className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-emerald-500 outline-none transition-all" />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-1">Password</label>
            <input type="password" defaultValue="password" className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-emerald-500 outline-none transition-all" />
          </div>
          <button className="w-full bg-emerald-600 hover:bg-emerald-700 text-white font-bold py-3 rounded-lg shadow-lg hover:shadow-xl transition-all transform hover:-translate-y-0.5">
            Login to SmartGarden
          </button>
        </form>
        <div className="mt-6 text-center">
          <p className="text-xs text-slate-400">Demo Mode: Just click Login</p>
        </div>
      </div>
    </div>
  </div>
);

const Dashboard = ({ plants, onViewPlant, onAdd }) => (
  <div className="space-y-8 animate-in fade-in duration-500">
    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div>
        <h2 className="text-3xl font-bold text-slate-800">My Plants</h2>
        <p className="text-slate-500 mt-1">
          {plants.length} active device{plants.length !== 1 && 's'} monitoring your garden
        </p>
      </div>
      <button onClick={onAdd} className="bg-emerald-600 text-white px-6 py-2.5 rounded-lg font-medium shadow-md hover:bg-emerald-700 hover:shadow-lg transition-all flex items-center gap-2">
        <Plus size={18} /> Add Plant
      </button>
    </div>

    {plants.length === 0 ? (
      <div className="bg-white border-2 border-dashed border-slate-300 rounded-xl p-12 text-center">
        <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center mx-auto mb-4">
          <Leaf className="text-slate-400" />
        </div>
        <h3 className="text-lg font-medium text-slate-800">No plants yet</h3>
        <p className="text-slate-500 mb-6">Connect your first ESP32 device to get started</p>
        <button onClick={onAdd} className="text-emerald-600 font-medium hover:underline">Launch Setup Wizard</button>
      </div>
    ) : (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {plants.map(plant => <PlantCard key={plant.id} plant={plant} onClick={() => onViewPlant(plant.id)} />)}
      </div>
    )}

    {/* Tips Section */}
    <div className="bg-gradient-to-r from-emerald-50 to-teal-50 border border-emerald-100 rounded-xl p-6">
      <div className="flex items-start gap-4">
        <div className="bg-white p-2 rounded-lg shadow-sm text-emerald-600">
          <Sun size={20} />
        </div>
        <div>
          <h4 className="font-bold text-emerald-900">Did you know?</h4>
          <p className="text-emerald-800 text-sm mt-1">{TIPS[Math.floor(Math.random() * TIPS.length)]}</p>
        </div>
      </div>
    </div>
  </div>
);

const PlantCard = ({ plant, onClick }) => {
  const isThirsty = plant.sensorData.soilMoisture < plant.thresholds.min;
  
  return (
    <div onClick={onClick} className="bg-white rounded-xl shadow-sm hover:shadow-xl border border-slate-100 overflow-hidden cursor-pointer transition-all duration-300 group">
      <div className="h-24 bg-slate-100 relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-b from-transparent to-black/10"></div>
        <div className="absolute bottom-3 left-4 flex items-center gap-3">
            <span className="text-3xl">{plant.icon}</span>
            <div>
                <h3 className="font-bold text-lg text-slate-800 leading-tight group-hover:text-emerald-700 transition-colors">{plant.nickname}</h3>
                <p className="text-xs text-slate-500 font-medium">{plant.species}</p>
            </div>
        </div>
        <div className={`absolute top-3 right-3 px-2 py-1 rounded-full text-xs font-bold flex items-center gap-1 ${plant.status.online ? 'bg-emerald-100 text-emerald-700' : 'bg-red-100 text-red-700'}`}>
          {plant.status.online ? <Wifi size={12} /> : <AlertTriangle size={12} />}
          {plant.status.online ? 'Online' : 'Offline'}
        </div>
      </div>
      
      <div className="p-4">
        <div className="grid grid-cols-3 gap-2 mb-4">
            <MetricSmall icon={<Droplets />} value={`${plant.sensorData.soilMoisture.toFixed(0)}%`} label="Soil" alert={isThirsty} />
            <MetricSmall icon={<Thermometer />} value={`${plant.sensorData.temp.toFixed(0)}°C`} label="Temp" />
            <MetricSmall icon={<Sun />} value={`${(plant.sensorData.light / 100).toFixed(0)}%`} label="Light" />
        </div>
        
        <div className="flex items-center justify-between text-xs text-slate-400 border-t border-slate-100 pt-3">
             <span className="flex items-center gap-1"><Smartphone size={12} /> {plant.device.name}</span>
             <span>Last updated: Just now</span>
        </div>
      </div>
    </div>
  );
};

const MetricSmall = ({ icon, value, label, alert }) => (
    <div className={`text-center p-2 rounded-lg ${alert ? 'bg-amber-50 ring-1 ring-amber-200' : 'bg-slate-50'}`}>
        <div className={`flex justify-center mb-1 ${alert ? 'text-amber-500 animate-pulse' : 'text-slate-400'}`}>{React.cloneElement(icon, { size: 16 })}</div>
        <div className={`font-bold text-sm ${alert ? 'text-amber-700' : 'text-slate-700'}`}>{value}</div>
        <div className="text-[10px] uppercase tracking-wide text-slate-400">{label}</div>
    </div>
);

const PlantDetail = ({ plant, onBack, onDelete, setPlants }) => {
  const [historyData] = useState(generateHistory());
  const [aiAnalysis, setAiAnalysis] = useState(null);
  const [isAiLoading, setIsAiLoading] = useState(false);
  const [usageTip, setUsageTip] = useState(null);
  const [isUsageLoading, setIsUsageLoading] = useState(false);

  const handleManualWater = () => {
    setPlants(prev => prev.map(p => {
        if(p.id === plant.id) {
            return { ...p, status: { ...p.status, isWatering: true }};
        }
        return p;
    }));
  };

  const toggleAutoWater = () => {
    setPlants(prev => prev.map(p => {
        if(p.id === plant.id) {
            return { ...p, settings: { ...p.settings, autoWater: !p.settings.autoWater }};
        }
        return p;
    }));
  };

  const getUsageTip = async () => {
    setIsUsageLoading(true);
    const prompt = `Give me one interesting usage tip, recipe idea, or folklore fact about the plant species "${plant.species}". Keep it concise (under 50 words).`;
    const tip = await callGemini(prompt);
    setUsageTip(tip);
    setIsUsageLoading(false);
  };

  const analyzeVitals = async () => {
    setIsAiLoading(true);
    setAiAnalysis(null);
    
    const prompt = `
      You are an expert botanist AI for the SmartGarden system. 
      Analyze the following live sensor data for a plant and provide a health assessment and 1 specific care tip.
      Keep it short (max 2 sentences).
      
      Plant Details:
      - Nickname: ${plant.nickname}
      - Species: ${plant.species}
      - Location: ${plant.location}
      
      Sensor Data:
      - Soil Moisture: ${plant.sensorData.soilMoisture.toFixed(1)}% (Target: ${plant.thresholds.min}-${plant.thresholds.max}%)
      - Temperature: ${plant.sensorData.temp.toFixed(1)}°C
      - Light Level: ${plant.sensorData.light.toFixed(0)} lux
      - Water Tank: ${plant.sensorData.tankLevel.toFixed(0)}%
      
      Is this plant happy?
    `;

    const response = await callGemini(prompt);
    setAiAnalysis(response);
    setIsAiLoading(false);
  };

  if(!plant) return <div>Loading...</div>;

  return (
    <div className="animate-in slide-in-from-right duration-300 pb-20">
      <div className="flex items-center gap-4 mb-6">
        <button onClick={onBack} className="p-2 hover:bg-slate-100 rounded-full transition-colors text-slate-500">
          <Home size={20} />
        </button>
        <h2 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
            <span>{plant.icon}</span> {plant.nickname}
        </h2>
        <div className="ml-auto flex items-center gap-2">
            <button onClick={() => onDelete(plant.id)} className="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors" title="Delete Plant">
                <Trash2 size={18} />
            </button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Main Control Panel */}
        <div className="lg:col-span-2 space-y-6">
            
            {/* Status Card */}
            <div className="bg-white rounded-2xl shadow-sm border border-slate-200 p-6 relative overflow-hidden">
                {plant.status.isWatering && (
                    <div className="absolute top-0 left-0 w-full h-1 bg-blue-500 animate-linear-progress"></div>
                )}
                
                <div className="flex justify-between items-start mb-6">
                    <div>
                        <h3 className="text-lg font-bold text-slate-800">Live Status</h3>
                        <p className="text-sm text-slate-500">Real-time telemetry from {plant.device.name}</p>
                    </div>
                    {plant.status.isWatering && (
                        <div className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-bold animate-pulse flex items-center gap-1">
                            <Droplets size={12} /> Watering Active
                        </div>
                    )}
                </div>

                <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <SensorBig 
                        icon={<Droplets />} 
                        label="Soil Moisture" 
                        value={plant.sensorData.soilMoisture.toFixed(1)} 
                        unit="%" 
                        color="text-emerald-600" 
                        sub={`Target: ${plant.thresholds.min}-${plant.thresholds.max}%`}
                    />
                    <SensorBig 
                        icon={<Thermometer />} 
                        label="Temperature" 
                        value={plant.sensorData.temp.toFixed(1)} 
                        unit="°C" 
                        color="text-orange-500" 
                        sub="Optimal: 18-26°C"
                    />
                    <SensorBig 
                        icon={<Sun />} 
                        label="Light Level" 
                        value={(plant.sensorData.light).toFixed(0)} 
                        unit="lux" 
                        color="text-amber-500" 
                        sub="Direct Sunlight"
                    />
                     <SensorBig 
                        icon={<Waves />} 
                        label="Water Tank" 
                        value={plant.sensorData.tankLevel.toFixed(0)} 
                        unit="%" 
                        color="text-blue-500" 
                        sub={plant.sensorData.tankLevel < 20 ? "Refill Soon" : "Good Level"}
                        alert={plant.sensorData.tankLevel < 20}
                    />
                </div>
            </div>

            {/* AI Analysis Card */}
            <div className="bg-gradient-to-br from-indigo-50 to-purple-50 rounded-2xl shadow-sm border border-indigo-100 p-6">
                <div className="flex items-center justify-between mb-4">
                  <h3 className="text-lg font-bold text-indigo-900 flex items-center gap-2">
                    <Sparkles className="text-indigo-600" size={18} />
                    Smart Vitals Analysis
                  </h3>
                  <button 
                    onClick={analyzeVitals}
                    disabled={isAiLoading}
                    className="bg-indigo-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-indigo-700 shadow-md transition-all flex items-center gap-2 disabled:opacity-50"
                  >
                    {isAiLoading ? <Loader2 className="animate-spin" size={16} /> : <Sparkles size={16} />}
                    {isAiLoading ? 'Analyzing...' : 'Analyze Vitals ✨'}
                  </button>
                </div>
                
                {aiAnalysis ? (
                  <div className="bg-white/60 p-4 rounded-xl border border-indigo-100 text-indigo-900 text-sm leading-relaxed animate-in fade-in">
                    {aiAnalysis}
                  </div>
                ) : (
                  <p className="text-indigo-400 text-sm italic">
                    Click "Analyze Vitals" to have Gemini assess current sensor readings against {plant.species} requirements.
                  </p>
                )}
            </div>

            {/* Charts Area */}
            <div className="bg-white rounded-2xl shadow-sm border border-slate-200 p-6">
                <div className="flex items-center justify-between mb-4">
                    <h3 className="font-bold text-slate-800">24h History</h3>
                    <div className="flex gap-2">
                        {['Soil', 'Temp'].map(t => (
                            <button key={t} className="px-3 py-1 text-xs font-medium bg-slate-100 text-slate-600 rounded hover:bg-slate-200">{t}</button>
                        ))}
                    </div>
                </div>
                <div className="h-64 w-full">
                    <ResponsiveContainer width="100%" height="100%">
                        <AreaChart data={historyData}>
                            <defs>
                                <linearGradient id="colorMoisture" x1="0" y1="0" x2="0" y2="1">
                                    <stop offset="5%" stopColor="#10b981" stopOpacity={0.1}/>
                                    <stop offset="95%" stopColor="#10b981" stopOpacity={0}/>
                                </linearGradient>
                            </defs>
                            <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                            <XAxis dataKey="time" axisLine={false} tickLine={false} tick={{fontSize: 12, fill: '#94a3b8'}} minTickGap={30} />
                            <YAxis axisLine={false} tickLine={false} tick={{fontSize: 12, fill: '#94a3b8'}} />
                            <Tooltip contentStyle={{borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)'}} />
                            <Area type="monotone" dataKey="moisture" stroke="#10b981" strokeWidth={2} fillOpacity={1} fill="url(#colorMoisture)" />
                        </AreaChart>
                    </ResponsiveContainer>
                </div>
            </div>
        </div>

        {/* Sidebar Controls */}
        <div className="space-y-6">
            
            {/* Plant Magic / Usage Tips Card */}
            <div className="bg-gradient-to-br from-amber-50 to-orange-50 rounded-2xl shadow-sm border border-amber-100 p-6">
                <h3 className="font-bold text-amber-900 mb-4 flex items-center gap-2">
                    <Sparkles size={16} /> Plant Magic
                </h3>
                
                {!usageTip ? (
                    <div className="text-center">
                        <p className="text-sm text-amber-800/70 mb-4">Discover recipes, folklore, or styling tips for your {plant.nickname}.</p>
                        <button 
                            onClick={getUsageTip}
                            disabled={isUsageLoading}
                            className="w-full py-2 bg-white text-amber-600 font-bold text-sm rounded-lg shadow-sm border border-amber-200 hover:bg-amber-50 flex items-center justify-center gap-2"
                        >
                            {isUsageLoading ? <Loader2 className="animate-spin" size={14} /> : "Get Fun Fact ✨"}
                        </button>
                    </div>
                ) : (
                    <div className="animate-in fade-in">
                        <p className="text-sm text-amber-900 italic leading-relaxed">"{usageTip}"</p>
                        <button onClick={() => setUsageTip(null)} className="mt-3 text-xs text-amber-600 font-bold hover:underline">Show another</button>
                    </div>
                )}
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-slate-200 p-6">
                <h3 className="font-bold text-slate-800 mb-4">Watering Controls</h3>
                
                <div className="mb-6">
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-sm font-medium text-slate-700">Auto-Watering</span>
                        <button 
                            onClick={toggleAutoWater}
                            className={`w-11 h-6 flex items-center rounded-full transition-colors ${plant.settings.autoWater ? 'bg-emerald-500' : 'bg-slate-300'}`}
                        >
                            <div className={`w-4 h-4 bg-white rounded-full shadow transform transition-transform ${plant.settings.autoWater ? 'translate-x-6' : 'translate-x-1'}`} />
                        </button>
                    </div>
                    <p className="text-xs text-slate-500">
                        Automatically water when moisture drops below {plant.thresholds.min}%.
                    </p>
                </div>

                <div className="mb-6 space-y-3">
                    <div className="flex justify-between text-xs text-slate-500">
                        <span>Dry ({plant.thresholds.min}%)</span>
                        <span>Wet ({plant.thresholds.max}%)</span>
                    </div>
                    <div className="h-2 bg-slate-100 rounded-full overflow-hidden">
                        <div 
                            className="h-full bg-emerald-500 opacity-20" 
                            style={{width: '100%'}} 
                        />
                        {/* Visual representation of range could go here */}
                    </div>
                </div>

                <button 
                    onClick={handleManualWater}
                    disabled={plant.status.isWatering}
                    className={`w-full py-3 rounded-xl font-bold flex items-center justify-center gap-2 transition-all ${
                        plant.status.isWatering 
                        ? 'bg-blue-50 text-blue-300 cursor-not-allowed' 
                        : 'bg-blue-500 text-white hover:bg-blue-600 shadow-lg hover:shadow-blue-500/30'
                    }`}
                >
                    <Droplets size={18} />
                    {plant.status.isWatering ? 'Watering...' : 'Water Now (5s)'}
                </button>
            </div>

            <div className="bg-white rounded-2xl shadow-sm border border-slate-200 p-6">
                <h3 className="font-bold text-slate-800 mb-4">Device Info</h3>
                <ul className="space-y-3 text-sm">
                    <li className="flex justify-between border-b border-slate-50 pb-2">
                        <span className="text-slate-500">Model</span>
                        <span className="font-medium text-slate-700">ESP32 DevKit V1</span>
                    </li>
                    <li className="flex justify-between border-b border-slate-50 pb-2">
                        <span className="text-slate-500">IP Address</span>
                        <span className="font-medium text-slate-700">192.168.1.104</span>
                    </li>
                    <li className="flex justify-between border-b border-slate-50 pb-2">
                        <span className="text-slate-500">Firmware</span>
                        <span className="font-medium text-slate-700">v2.1.0 (Stable)</span>
                    </li>
                    <li className="flex justify-between">
                        <span className="text-slate-500">Signal</span>
                        <span className="font-medium text-emerald-600 flex items-center gap-1"><Wifi size={14} /> Good (-65dBm)</span>
                    </li>
                </ul>
            </div>
        </div>
      </div>
    </div>
  );
};

const AiDoctorView = () => {
  const [messages, setMessages] = useState([
    { role: 'system', text: "Hello! I'm the AI Plant Doctor. I can help diagnose issues with your plants or answer gardening questions. What's on your mind? ✨" }
  ]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const scrollRef = useRef(null);

  useEffect(() => {
    if (scrollRef.current) scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
  }, [messages]);

  const handleSend = async (e) => {
    e.preventDefault();
    if (!input.trim()) return;

    const userMsg = input;
    setMessages(prev => [...prev, { role: 'user', text: userMsg }]);
    setInput('');
    setLoading(true);

    const prompt = `
      You are an expert plant pathologist and master gardener.
      User Question: "${userMsg}"
      
      Provide a helpful, friendly, and scientifically accurate response. 
      If diagnosing a problem, list potential causes and solutions.
      Keep the response concise (under 100 words).
    `;

    const response = await callGemini(prompt);
    
    setMessages(prev => [...prev, { role: 'ai', text: response }]);
    setLoading(false);
  };

  return (
    <div className="h-full flex flex-col bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
      <div className="bg-indigo-600 p-4 text-white flex items-center gap-3">
        <Sparkles size={20} />
        <div>
          <h2 className="font-bold">AI Plant Doctor</h2>
          <p className="text-indigo-200 text-xs">Powered by Gemini</p>
        </div>
      </div>

      <div className="flex-1 overflow-y-auto p-4 space-y-4" ref={scrollRef}>
        {messages.map((msg, idx) => (
          <div key={idx} className={`flex ${msg.role === 'user' ? 'justify-end' : 'justify-start'}`}>
            <div className={`max-w-[80%] rounded-2xl px-4 py-3 text-sm ${
              msg.role === 'user' 
                ? 'bg-indigo-600 text-white rounded-br-none' 
                : 'bg-slate-100 text-slate-800 rounded-bl-none'
            }`}>
              {msg.text}
            </div>
          </div>
        ))}
        {loading && (
          <div className="flex justify-start">
            <div className="bg-slate-100 rounded-2xl rounded-bl-none px-4 py-3">
              <Loader2 className="animate-spin text-slate-400" size={18} />
            </div>
          </div>
        )}
      </div>

      <form onSubmit={handleSend} className="p-4 border-t border-slate-200 bg-slate-50 flex gap-2">
        <input 
          type="text" 
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Describe symptoms or ask a question..."
          className="flex-1 px-4 py-2 border border-slate-300 rounded-xl focus:ring-2 focus:ring-indigo-500 outline-none"
        />
        <button 
          type="submit" 
          disabled={loading || !input.trim()}
          className="bg-indigo-600 text-white p-3 rounded-xl hover:bg-indigo-700 disabled:opacity-50 transition-colors"
        >
          <Send size={18} />
        </button>
      </form>
    </div>
  );
};

const SensorBig = ({ icon, label, value, unit, color, sub, alert }) => (
    <div className={`p-4 rounded-xl border ${alert ? 'border-red-200 bg-red-50' : 'border-slate-100 bg-slate-50'} flex flex-col items-center justify-center text-center`}>
        <div className={`mb-2 ${color}`}>{React.cloneElement(icon, { size: 24 })}</div>
        <div className="text-2xl font-bold text-slate-800 tracking-tight">
            {value}<span className="text-sm font-normal text-slate-500 ml-0.5">{unit}</span>
        </div>
        <div className="text-xs font-bold text-slate-500 uppercase tracking-wide mt-1">{label}</div>
        <div className={`text-[10px] mt-1 ${alert ? 'text-red-500 font-bold' : 'text-slate-400'}`}>{sub}</div>
    </div>
);

const Waves = ({ size }) => (
    <svg xmlns="http://www.w3.org/2000/svg" width={size || 24} height={size || 24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M2 6c.6.5 1.2 1 2.5 1C7 7 7 5 9.5 5c2.6 0 2.4 2 5 2 2.5 0 2.5-2 5-2 1.3 0 1.9.5 2.5 1"/><path d="M2 12c.6.5 1.2 1 2.5 1 2.5 0 2.5-2 5-2 2.6 0 2.4 2 5 2 2.5 0 2.5-2 5-2 1.3 0 1.9.5 2.5 1"/><path d="M2 18c.6.5 1.2 1 2.5 1 2.5 0 2.5-2 5-2 2.6 0 2.4 2 5 2 2.5 0 2.5-2 5-2 1.3 0 1.9.5 2.5 1"/></svg>
);

/**
 * WIZARD & FORMS
 */
const AddPlantWizard = ({ onCancel, onComplete }) => {
    const [step, setStep] = useState(1);
    const [search, setSearch] = useState("");
    const [results, setResults] = useState([]);
    const [selectedSpecies, setSelectedSpecies] = useState(null);
    const [details, setDetails] = useState({ nickname: "", location: "Living Room", autoWater: false });
    const [isSearching, setIsSearching] = useState(false);
    const [isGeneratingName, setIsGeneratingName] = useState(false);

    // Debounced Search Simulation
    useEffect(() => {
        if(search.length < 2) { setResults([]); return; }
        
        setIsSearching(true);
        const timer = setTimeout(() => {
            const matches = MOCK_PLANT_DB.filter(p => 
                p.common_name.toLowerCase().includes(search.toLowerCase())
            );
            setResults(matches);
            setIsSearching(false);
        }, 500);
        return () => clearTimeout(timer);
    }, [search]);

    const generateMagicName = async () => {
        if (!selectedSpecies) return;
        setIsGeneratingName(true);
        const prompt = `Suggest one creative, funny, or charming nickname for a ${selectedSpecies.common_name} plant. Output ONLY the name.`;
        const name = await callGemini(prompt);
        setDetails(prev => ({ ...prev, nickname: name.replace(/["']/g, "").trim() }));
        setIsGeneratingName(false);
    };

    const handleCreate = () => {
        const newPlant = {
            id: Date.now(),
            nickname: details.nickname || selectedSpecies.common_name,
            species: selectedSpecies.common_name,
            icon: selectedSpecies.image,
            location: details.location,
            thresholds: { 
                min: selectedSpecies.suggested_threshold, 
                max: selectedSpecies.suggested_threshold + 40 
            },
            settings: { autoWater: details.autoWater },
            status: { online: true, isWatering: false },
            device: { name: `ESP32-${Math.floor(Math.random()*9000)+1000}` },
            sensorData: {
                soilMoisture: 50,
                temp: 22,
                humidity: 60,
                light: 500,
                tankLevel: 100
            }
        };
        onComplete(newPlant);
    };

    return (
        <div className="max-w-2xl mx-auto bg-white rounded-2xl shadow-xl overflow-hidden border border-slate-200">
            <div className="bg-emerald-600 p-6 text-white flex justify-between items-center">
                <div>
                    <h2 className="text-xl font-bold">Add New Plant</h2>
                    <p className="text-emerald-100 text-sm">Step {step} of 2</p>
                </div>
                <button onClick={onCancel} className="bg-emerald-700 p-2 rounded-full hover:bg-emerald-800"><X size={20} /></button>
            </div>

            <div className="p-8">
                {step === 1 ? (
                    <div className="space-y-6">
                        <div className="text-center">
                            <h3 className="text-lg font-bold text-slate-800">Smart Search</h3>
                            <p className="text-slate-500">Find your plant species to auto-configure sensors.</p>
                        </div>
                        
                        <div className="relative">
                            <Search className="absolute left-4 top-3.5 text-slate-400" size={20} />
                            <input 
                                type="text" 
                                placeholder="Search plant (e.g., Basil, Tomato, Snake Plant)..."
                                className="w-full pl-12 pr-4 py-3 border border-slate-300 rounded-xl focus:ring-2 focus:ring-emerald-500 outline-none"
                                value={search}
                                onChange={e => setSearch(e.target.value)}
                            />
                            {isSearching && <div className="absolute right-4 top-3.5 w-5 h-5 border-2 border-emerald-500 border-t-transparent rounded-full animate-spin"></div>}
                        </div>

                        <div className="grid grid-cols-1 gap-3 max-h-80 overflow-y-auto">
                            {results.map(plant => (
                                <div 
                                    key={plant.id} 
                                    onClick={() => { setSelectedSpecies(plant); setStep(2); }}
                                    className="flex items-center gap-4 p-4 border border-slate-200 rounded-xl hover:border-emerald-500 hover:bg-emerald-50 cursor-pointer transition-all"
                                >
                                    <span className="text-3xl">{plant.image}</span>
                                    <div>
                                        <h4 className="font-bold text-slate-800">{plant.common_name}</h4>
                                        <div className="flex gap-2 text-xs text-slate-500 mt-1">
                                            <span className="bg-slate-100 px-2 py-0.5 rounded">Water: {plant.moisture_guide}</span>
                                            <span className="bg-slate-100 px-2 py-0.5 rounded">Target: {plant.suggested_threshold}%</span>
                                        </div>
                                    </div>
                                    <div className="ml-auto text-emerald-600">
                                        <Plus size={20} />
                                    </div>
                                </div>
                            ))}
                            {search.length > 2 && results.length === 0 && !isSearching && (
                                <div className="text-center py-8 text-slate-400">
                                    No plants found. Try a different name.
                                </div>
                            )}
                        </div>
                    </div>
                ) : (
                    <div className="space-y-6">
                        <div className="flex items-center gap-4 p-4 bg-emerald-50 rounded-xl border border-emerald-100">
                            <span className="text-4xl">{selectedSpecies.image}</span>
                            <div>
                                <h3 className="font-bold text-emerald-900">{selectedSpecies.common_name}</h3>
                                <p className="text-sm text-emerald-700">{selectedSpecies.scientific_name}</p>
                            </div>
                            <button onClick={() => setStep(1)} className="ml-auto text-sm text-emerald-600 font-medium hover:underline">Change</button>
                        </div>

                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-sm font-bold text-slate-700 mb-1">Nickname</label>
                                <div className="flex gap-2">
                                    <input 
                                        className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-emerald-500 outline-none" 
                                        placeholder="e.g. Kitchen Basil"
                                        value={details.nickname}
                                        onChange={e => setDetails({...details, nickname: e.target.value})}
                                    />
                                    <button 
                                        onClick={generateMagicName}
                                        disabled={isGeneratingName}
                                        className="bg-purple-100 text-purple-600 p-2 rounded-lg hover:bg-purple-200 transition-colors"
                                        title="Generate Magic Name"
                                    >
                                        {isGeneratingName ? <Loader2 className="animate-spin" size={20} /> : <Sparkles size={20} />}
                                    </button>
                                </div>
                            </div>
                             <div>
                                <label className="block text-sm font-bold text-slate-700 mb-1">Room / Location</label>
                                <select 
                                    className="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-emerald-500 outline-none"
                                    value={details.location}
                                    onChange={e => setDetails({...details, location: e.target.value})}
                                >
                                    <option>Living Room</option>
                                    <option>Kitchen</option>
                                    <option>Balcony</option>
                                    <option>Bedroom</option>
                                    <option>Greenhouse</option>
                                </select>
                            </div>
                        </div>

                        <div className="bg-slate-50 p-4 rounded-xl border border-slate-200">
                            <label className="flex items-center justify-between cursor-pointer">
                                <div>
                                    <span className="block font-bold text-slate-700">Enable Auto-Watering</span>
                                    <span className="text-xs text-slate-500">System will water when soil is dry</span>
                                </div>
                                <input 
                                    type="checkbox" 
                                    className="w-5 h-5 text-emerald-600 rounded focus:ring-emerald-500" 
                                    checked={details.autoWater}
                                    onChange={e => setDetails({...details, autoWater: e.target.checked})}
                                />
                            </label>
                        </div>

                        <div className="flex gap-4 pt-4">
                            <button onClick={() => setStep(1)} className="flex-1 py-3 text-slate-600 font-medium hover:bg-slate-50 rounded-lg">Back</button>
                            <button onClick={handleCreate} className="flex-1 bg-emerald-600 text-white font-bold py-3 rounded-lg shadow hover:bg-emerald-700">Complete Setup</button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

const CalibrationScreen = ({ onComplete }) => {
    const [progress, setProgress] = useState(0);
    const [stage, setStage] = useState(0); // 0: dry, 1: wet, 2: complete

    useEffect(() => {
        if(stage === 2) return;
        const timer = setInterval(() => {
            setProgress(prev => {
                if(prev >= 100) {
                    clearInterval(timer);
                    setTimeout(() => setStage(s => s + 1), 500);
                    return 0;
                }
                return prev + 2;
            });
        }, 50);
        return () => clearInterval(timer);
    }, [stage]);

    return (
        <div className="max-w-lg mx-auto mt-10 text-center">
            <h2 className="text-2xl font-bold text-slate-800 mb-6">Sensor Calibration</h2>
            
            <div className="bg-white p-8 rounded-2xl shadow-xl border border-slate-200">
                {stage === 0 && (
                    <div className="animate-in fade-in">
                        <div className="w-20 h-20 bg-amber-100 rounded-full flex items-center justify-center mx-auto mb-4 text-amber-600">
                            <Wind size={32} />
                        </div>
                        <h3 className="text-xl font-bold mb-2">Calibrating: DRY Air</h3>
                        <p className="text-slate-500 mb-6">Please expose the sensor to open air. Do not submerge.</p>
                        <ProgressBar value={progress} />
                        <p className="text-xs text-slate-400 mt-2">Reading baseline capacitance...</p>
                    </div>
                )}

                {stage === 1 && (
                    <div className="animate-in fade-in">
                        <div className="w-20 h-20 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4 text-blue-600">
                            <Droplets size={32} />
                        </div>
                        <h3 className="text-xl font-bold mb-2">Calibrating: WET Soil</h3>
                        <p className="text-slate-500 mb-6">Insert sensor into saturated soil or water cup.</p>
                        <ProgressBar value={progress} color="bg-blue-500" />
                        <p className="text-xs text-slate-400 mt-2">Determining saturation limit...</p>
                    </div>
                )}

                {stage === 2 && (
                    <div className="animate-in zoom-in">
                        <div className="w-20 h-20 bg-emerald-100 rounded-full flex items-center justify-center mx-auto mb-4 text-emerald-600">
                            <Check size={40} />
                        </div>
                        <h3 className="text-xl font-bold mb-2 text-emerald-800">Calibration Complete!</h3>
                        <p className="text-slate-500 mb-6">Your device is now synced with the SmartGarden cloud.</p>
                        <button onClick={onComplete} className="bg-emerald-600 text-white px-8 py-3 rounded-lg font-bold shadow hover:bg-emerald-700">Go to Dashboard</button>
                    </div>
                )}
            </div>
        </div>
    );
};

const ProgressBar = ({ value, color = 'bg-emerald-500' }) => (
    <div className="h-4 w-full bg-slate-100 rounded-full overflow-hidden">
        <div className={`h-full transition-all duration-100 ${color}`} style={{width: `${value}%`}}></div>
    </div>
);
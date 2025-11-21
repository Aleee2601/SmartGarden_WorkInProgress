import React, { useState, useEffect } from 'react';
import { Search, X, Loader, CheckCircle, Droplet, Sun } from 'lucide-react';
import plantService from '../api/plantService';

/**
 * AddPlantWizard - Smart plant creation with auto-search
 *
 * Features:
 * - Debounced search for plant species
 * - Auto-fill moisture thresholds based on plant type
 * - Visual plant selection with images
 * - Smart defaults from external plant database
 */
export default function AddPlantWizard({ onClose, onPlantCreated }) {
  const [step, setStep] = useState(1); // 1: Search, 2: Details, 3: Review
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [selectedPlant, setSelectedPlant] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Form data
  const [formData, setFormData] = useState({
    nickname: '',
    speciesName: '',
    imageUrl: '',
    minMoistureThreshold: 40, // Default
    roomName: '',
    isOutdoor: false
  });

  // Debounced search
  useEffect(() => {
    if (searchQuery.length < 2) {
      setSearchResults([]);
      return;
    }

    const timer = setTimeout(() => {
      performSearch(searchQuery);
    }, 500); // Wait 500ms after user stops typing

    return () => clearTimeout(timer);
  }, [searchQuery]);

  const performSearch = async (query) => {
    setLoading(true);
    setError(null);

    try {
      const results = await plantService.searchPlants(query);
      setSearchResults(results);
    } catch (err) {
      console.error('Search failed:', err);
      setError('Failed to search plants. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleSelectPlant = (plant) => {
    setSelectedPlant(plant);
    setFormData({
      ...formData,
      speciesName: plant.commonName,
      imageUrl: plant.imageUrl,
      minMoistureThreshold: plant.suggestedMoistureThreshold,
      nickname: plant.commonName // Default nickname to species name
    });
    setStep(2); // Move to details step
  };

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'checkbox' ? checked : value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const newPlant = await plantService.createPlantFromSearch(formData);
      onPlantCreated?.(newPlant);
      onClose();
    } catch (err) {
      console.error('Failed to create plant:', err);
      setError(err.message || 'Failed to create plant. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <div className="bg-white rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-hidden flex flex-col">
        {/* Header */}
        <div className="bg-gradient-to-r from-green-500 to-green-600 p-6 text-white">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-bold">
              {step === 1 && '🔍 Search for Your Plant'}
              {step === 2 && '📝 Plant Details'}
              {step === 3 && '✅ Review & Create'}
            </h2>
            <button
              onClick={onClose}
              className="text-white hover:bg-white hover:bg-opacity-20 p-2 rounded-lg transition-all"
            >
              <X className="w-6 h-6" />
            </button>
          </div>
          {/* Progress Steps */}
          <div className="flex items-center justify-between mt-4">
            {[1, 2, 3].map((s) => (
              <div key={s} className="flex items-center">
                <div
                  className={`w-8 h-8 rounded-full flex items-center justify-center ${
                    step >= s ? 'bg-white text-green-600' : 'bg-green-400 text-white'
                  }`}
                >
                  {s}
                </div>
                {s < 3 && (
                  <div
                    className={`h-1 w-16 mx-2 ${
                      step > s ? 'bg-white' : 'bg-green-400'
                    }`}
                  />
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Content */}
        <div className="flex-1 overflow-y-auto p-6">
          {/* Step 1: Search */}
          {step === 1 && (
            <div className="space-y-4">
              <p className="text-gray-600">
                Search for your plant species to get smart auto-settings!
              </p>

              {/* Search Input */}
              <div className="relative">
                <Search className="absolute left-3 top-3 w-5 h-5 text-gray-400" />
                <input
                  type="text"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  placeholder="Search for plants (e.g., Basil, Tomato, Snake Plant)..."
                  className="w-full pl-10 pr-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  autoFocus
                />
              </div>

              {/* Loading State */}
              {loading && (
                <div className="flex items-center justify-center py-8">
                  <Loader className="w-8 h-8 text-green-600 animate-spin" />
                  <span className="ml-2 text-gray-600">Searching...</span>
                </div>
              )}

              {/* Error */}
              {error && (
                <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-700">
                  {error}
                </div>
              )}

              {/* Search Results */}
              {!loading && searchResults.length > 0 && (
                <div className="space-y-2">
                  <h3 className="font-semibold text-gray-800">
                    Found {searchResults.length} plants:
                  </h3>
                  <div className="grid grid-cols-1 gap-3 max-h-96 overflow-y-auto">
                    {searchResults.map((plant, index) => (
                      <button
                        key={index}
                        onClick={() => handleSelectPlant(plant)}
                        className="flex items-center space-x-4 p-4 border border-gray-200 rounded-lg hover:border-green-500 hover:bg-green-50 transition-all text-left"
                      >
                        {/* Plant Image */}
                        <div className="w-20 h-20 bg-gray-100 rounded-lg overflow-hidden flex-shrink-0">
                          {plant.imageUrl ? (
                            <img
                              src={plant.imageUrl}
                              alt={plant.commonName}
                              className="w-full h-full object-cover"
                              onError={(e) => {
                                e.target.style.display = 'none';
                              }}
                            />
                          ) : (
                            <div className="w-full h-full flex items-center justify-center text-3xl">
                              🌱
                            </div>
                          )}
                        </div>

                        {/* Plant Info */}
                        <div className="flex-1">
                          <h4 className="font-semibold text-gray-800">
                            {plant.commonName}
                          </h4>
                          {plant.scientificName && (
                            <p className="text-sm text-gray-500 italic">
                              {plant.scientificName}
                            </p>
                          )}
                          <div className="flex items-center space-x-4 mt-2">
                            <div className="flex items-center space-x-1 text-sm text-gray-600">
                              <Droplet className="w-4 h-4 text-blue-500" />
                              <span>{plant.wateringDescription}</span>
                            </div>
                            {plant.sunlight && (
                              <div className="flex items-center space-x-1 text-sm text-gray-600">
                                <Sun className="w-4 h-4 text-yellow-500" />
                                <span>{plant.sunlight}</span>
                              </div>
                            )}
                          </div>
                          {/* Auto-suggested threshold */}
                          <div className="mt-2 inline-block bg-green-100 text-green-700 px-3 py-1 rounded-full text-xs font-semibold">
                            ⚡ Auto: {plant.suggestedMoistureThreshold}% moisture
                          </div>
                        </div>

                        <CheckCircle className="w-6 h-6 text-green-600" />
                      </button>
                    ))}
                  </div>
                </div>
              )}

              {/* Empty State */}
              {!loading && searchQuery.length >= 2 && searchResults.length === 0 && (
                <div className="text-center py-8 text-gray-500">
                  <p>No plants found for "{searchQuery}"</p>
                  <p className="text-sm mt-2">Try a different search term</p>
                </div>
              )}
            </div>
          )}

          {/* Step 2: Details Form */}
          {step === 2 && (
            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Selected Plant Preview */}
              {selectedPlant && (
                <div className="bg-green-50 border border-green-200 rounded-lg p-4 flex items-center space-x-4">
                  <div className="w-16 h-16 bg-white rounded-lg overflow-hidden">
                    {formData.imageUrl ? (
                      <img
                        src={formData.imageUrl}
                        alt={formData.speciesName}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center text-2xl">
                        🌱
                      </div>
                    )}
                  </div>
                  <div>
                    <h4 className="font-semibold text-gray-800">
                      {formData.speciesName}
                    </h4>
                    <p className="text-sm text-gray-600">
                      Moisture: {formData.minMoistureThreshold}%
                    </p>
                  </div>
                </div>
              )}

              {/* Nickname */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Nickname (Required)
                </label>
                <input
                  type="text"
                  name="nickname"
                  value={formData.nickname}
                  onChange={handleInputChange}
                  required
                  placeholder="e.g., My Basil, Kitchen Tomato"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                />
              </div>

              {/* Room Name */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Room/Location
                </label>
                <input
                  type="text"
                  name="roomName"
                  value={formData.roomName}
                  onChange={handleInputChange}
                  placeholder="e.g., Kitchen, Living Room, Balcony"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                />
              </div>

              {/* Moisture Threshold (Editable) */}
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Moisture Threshold (%)
                </label>
                <div className="flex items-center space-x-2">
                  <input
                    type="number"
                    name="minMoistureThreshold"
                    value={formData.minMoistureThreshold}
                    onChange={handleInputChange}
                    min="0"
                    max="100"
                    className="w-24 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  />
                  <input
                    type="range"
                    name="minMoistureThreshold"
                    value={formData.minMoistureThreshold}
                    onChange={handleInputChange}
                    min="0"
                    max="100"
                    className="flex-1"
                  />
                </div>
                <p className="text-xs text-gray-500 mt-1">
                  ⚡ Auto-suggested: {selectedPlant?.suggestedMoistureThreshold}%
                  (you can adjust)
                </p>
              </div>

              {/* Outdoor/Indoor */}
              <div className="flex items-center">
                <input
                  type="checkbox"
                  name="isOutdoor"
                  checked={formData.isOutdoor}
                  onChange={handleInputChange}
                  className="w-4 h-4 text-green-600 focus:ring-green-500 border-gray-300 rounded"
                />
                <label className="ml-2 text-sm text-gray-700">
                  This is an outdoor plant
                </label>
              </div>

              {/* Error */}
              {error && (
                <div className="bg-red-50 border border-red-200 rounded-lg p-3 text-red-700 text-sm">
                  {error}
                </div>
              )}

              {/* Buttons */}
              <div className="flex items-center justify-between pt-4">
                <button
                  type="button"
                  onClick={() => setStep(1)}
                  className="px-6 py-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-all"
                >
                  ← Back to Search
                </button>
                <button
                  type="submit"
                  disabled={loading}
                  className="px-6 py-3 bg-gradient-to-r from-green-500 to-green-600 text-white rounded-lg font-semibold hover:from-green-600 hover:to-green-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
                >
                  {loading ? (
                    <>
                      <Loader className="w-5 h-5 animate-spin" />
                      <span>Creating...</span>
                    </>
                  ) : (
                    <span>Create Plant 🌱</span>
                  )}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}

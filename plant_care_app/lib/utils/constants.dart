class AppConstants {
  // App info
  static const String appName = 'Plant Care';
  static const String appVersion = '1.0.0';
  
  // Routes
  static const String homeRoute = '/';
  static const String plantListRoute = '/plants';
  static const String plantDetailRoute = '/plant';
  static const String addPlantRoute = '/add-plant';
  static const String editPlantRoute = '/edit-plant';
  static const String settingsRoute = '/settings';
  
  // Care instruction types
  static const List<String> careTypes = [
    'Water',
    'Light',
    'Fertilizer',
    'Humidity',
    'Temperature',
    'Soil',
    'Pruning',
    'Repotting',
    'Pest Control',
    'Other',
  ];
  
  // Common plant locations
  static const List<String> plantLocations = [
    'Living Room',
    'Bedroom',
    'Kitchen',
    'Bathroom',
    'Office',
    'Balcony',
    'Garden',
    'Patio',
    'Hallway',
    'Dining Room',
    'Other',
  ];
  
  // Common watering frequencies
  static const List<String> wateringFrequencies = [
    'Daily',
    'Every 2-3 days',
    'Weekly',
    'Every 2 weeks',
    'Monthly',
    'As needed',
  ];
  
  // Common light requirements
  static const List<String> lightRequirements = [
    'Full sun',
    'Partial sun',
    'Bright indirect light',
    'Medium light',
    'Low light',
    'Shade',
  ];
  
  // Common plant species
  static const List<Map<String, String>> commonPlants = [
    {'name': 'Peace Lily', 'species': 'Spathiphyllum'},
    {'name': 'Snake Plant', 'species': 'Sansevieria trifasciata'},
    {'name': 'Monstera', 'species': 'Monstera deliciosa'},
    {'name': 'Pothos', 'species': 'Epipremnum aureum'},
    {'name': 'Spider Plant', 'species': 'Chlorophytum comosum'},
    {'name': 'Aloe Vera', 'species': 'Aloe barbadensis miller'},
    {'name': 'Rubber Plant', 'species': 'Ficus elastica'},
    {'name': 'ZZ Plant', 'species': 'Zamioculcas zamiifolia'},
    {'name': 'Fiddle Leaf Fig', 'species': 'Ficus lyrata'},
    {'name': 'Boston Fern', 'species': 'Nephrolepis exaltata'},
    {'name': 'Jade Plant', 'species': 'Crassula ovata'},
    {'name': 'African Violet', 'species': 'Saintpaulia'},
    {'name': 'Orchid', 'species': 'Phalaenopsis'},
    {'name': 'English Ivy', 'species': 'Hedera helix'},
    {'name': 'Chinese Money Plant', 'species': 'Pilea peperomioides'},
  ];
}

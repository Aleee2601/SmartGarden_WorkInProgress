import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'screens/home_screen.dart';
import 'screens/plant_list_screen.dart';
import 'screens/plant_detail_screen.dart';
import 'screens/add_edit_plant_screen.dart';
import 'screens/settings_screen.dart';
import 'utils/constants.dart';
import 'utils/theme.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  
  // Set preferred orientations
  await SystemChrome.setPreferredOrientations([
    DeviceOrientation.portraitUp,
    DeviceOrientation.portraitDown,
  ]);
  
  runApp(const PlantCareApp());
}

class PlantCareApp extends StatelessWidget {
  const PlantCareApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: AppConstants.appName,
      theme: AppTheme.lightTheme,
      debugShowCheckedModeBanner: false,
      initialRoute: AppConstants.homeRoute,
      routes: {
        AppConstants.homeRoute: (context) => const HomeScreen(),
        AppConstants.plantListRoute: (context) => const PlantListScreen(),
        AppConstants.addPlantRoute: (context) => const AddEditPlantScreen(),
        AppConstants.settingsRoute: (context) => const SettingsScreen(),
      },
      onGenerateRoute: (settings) {
        if (settings.name == AppConstants.plantDetailRoute) {
          final args = settings.arguments as Map<String, dynamic>;
          final plantId = args['plantId'] as int;
          return MaterialPageRoute(
            builder: (context) => PlantDetailScreen(plantId: plantId),
          );
        } else if (settings.name == AppConstants.editPlantRoute) {
          final args = settings.arguments as Map<String, dynamic>;
          final plant = args['plant'];
          return MaterialPageRoute(
            builder: (context) => AddEditPlantScreen(plant: plant),
          );
        }
        return null;
      },
    );
  }
}

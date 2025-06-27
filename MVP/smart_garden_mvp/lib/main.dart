import 'package:flutter/material.dart';
// import 'screens/login_screen.dart';
import 'screens/dashboard_screen.dart';
import 'screens/plant_details_screen.dart';
import 'models/plant.dart';

void main() {
  runApp(const MainApp());
}

class MainApp extends StatelessWidget {
  const MainApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'SmartGarden',
      debugShowCheckedModeBanner: false,
      theme: ThemeData(
        primarySwatch: Colors.green,
        scaffoldBackgroundColor: Colors.white,
      ),
      home: DashboardScreen(),

     onGenerateRoute: (settings) {
      if (settings.name == '/plant-details') {
        final plant = settings.arguments as Plant;
        return MaterialPageRoute(
          builder: (context) => PlantDetailsScreen(plant: plant),
        );
      }
      return null;
    }  

    );
  }
}

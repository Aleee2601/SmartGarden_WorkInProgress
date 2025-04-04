import 'dart:io';
import 'package:path_provider/path_provider.dart';
import 'package:image_picker/image_picker.dart';
import '../models/plant.dart';
import '../models/care_instruction.dart';
import 'database_service.dart';

class PlantService {
  final DatabaseService _databaseService = DatabaseService();
  final ImagePicker _imagePicker = ImagePicker();

  // Get all plants
  Future<List<Plant>> getPlants() async {
    return await _databaseService.getPlants();
  }

  // Get a single plant by ID
  Future<Plant?> getPlant(int id) async {
    return await _databaseService.getPlant(id);
  }

  // Add a new plant
  Future<int> addPlant(Plant plant) async {
    return await _databaseService.insertPlant(plant);
  }

  // Update an existing plant
  Future<int> updatePlant(Plant plant) async {
    return await _databaseService.updatePlant(plant);
  }

  // Delete a plant
  Future<int> deletePlant(int id) async {
    return await _databaseService.deletePlant(id);
  }

  // Pick an image from gallery
  Future<String?> pickImage() async {
    final XFile? image = await _imagePicker.pickImage(
      source: ImageSource.gallery,
      maxWidth: 800,
      maxHeight: 800,
      imageQuality: 85,
    );

    if (image != null) {
      final Directory appDir = await getApplicationDocumentsDirectory();
      final String imageName = 'plant_${DateTime.now().millisecondsSinceEpoch}.jpg';
      final String imagePath = '${appDir.path}/$imageName';
      
      // Copy the image to the app's documents directory
      await File(image.path).copy(imagePath);
      
      return imagePath;
    }
    
    return null;
  }

  // Take a photo with camera
  Future<String?> takePhoto() async {
    final XFile? image = await _imagePicker.pickImage(
      source: ImageSource.camera,
      maxWidth: 800,
      maxHeight: 800,
      imageQuality: 85,
    );

    if (image != null) {
      final Directory appDir = await getApplicationDocumentsDirectory();
      final String imageName = 'plant_${DateTime.now().millisecondsSinceEpoch}.jpg';
      final String imagePath = '${appDir.path}/$imageName';
      
      // Copy the image to the app's documents directory
      await File(image.path).copy(imagePath);
      
      return imagePath;
    }
    
    return null;
  }

  // Delete an image
  Future<void> deleteImage(String imagePath) async {
    final file = File(imagePath);
    if (await file.exists()) {
      await file.delete();
    }
  }

  // Get default care instructions for common plants
  List<CareInstruction> getDefaultCareInstructions(String species) {
    // This is a simplified version. In a real app, you would have a more comprehensive database.
    switch (species.toLowerCase()) {
      case 'spathiphyllum':
      case 'peace lily':
        return [
          CareInstruction(
            type: 'Water',
            description: 'Keep soil moist but not soggy',
            frequency: 'Once a week',
          ),
          CareInstruction(
            type: 'Light',
            description: 'Indirect light, no direct sunlight',
            frequency: 'Daily',
          ),
          CareInstruction(
            type: 'Fertilizer',
            description: 'Balanced houseplant fertilizer',
            frequency: 'Every 6-8 weeks during growing season',
          ),
        ];
      case 'sansevieria trifasciata':
      case 'snake plant':
        return [
          CareInstruction(
            type: 'Water',
            description: 'Allow soil to dry between waterings',
            frequency: 'Every 2-3 weeks',
          ),
          CareInstruction(
            type: 'Light',
            description: 'Tolerates low light but prefers bright indirect light',
            frequency: 'Daily',
          ),
          CareInstruction(
            type: 'Fertilizer',
            description: 'Cactus or succulent fertilizer',
            frequency: 'Once in spring and summer',
          ),
        ];
      case 'monstera deliciosa':
      case 'swiss cheese plant':
        return [
          CareInstruction(
            type: 'Water',
            description: 'Allow top inch of soil to dry out between waterings',
            frequency: 'Every 1-2 weeks',
          ),
          CareInstruction(
            type: 'Light',
            description: 'Bright indirect light',
            frequency: 'Daily',
          ),
          CareInstruction(
            type: 'Humidity',
            description: 'Mist leaves or use a humidifier',
            frequency: 'Regular misting in dry conditions',
          ),
        ];
      default:
        return [
          CareInstruction(
            type: 'Water',
            description: 'Check soil moisture regularly',
            frequency: 'As needed',
          ),
          CareInstruction(
            type: 'Light',
            description: 'Depends on plant type',
            frequency: 'Daily',
          ),
        ];
    }
  }
}

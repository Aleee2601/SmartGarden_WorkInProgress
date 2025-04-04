import 'dart:io';
import 'package:flutter/material.dart';
import '../models/plant.dart';
import '../services/plant_service.dart';
import '../utils/theme.dart';
import '../widgets/care_instruction_card.dart';
import 'add_edit_plant_screen.dart';

class PlantDetailScreen extends StatefulWidget {
  final int plantId;

  const PlantDetailScreen({
    super.key,
    required this.plantId,
  });

  @override
  PlantDetailScreenState createState() => PlantDetailScreenState();
}

class PlantDetailScreenState extends State<PlantDetailScreen> {
  final PlantService _plantService = PlantService();
  Plant? _plant;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadPlant();
  }

  Future<void> _loadPlant() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final plant = await _plantService.getPlant(widget.plantId);
      setState(() {
        _plant = plant;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error loading plant: $e')),
        );
      }
    }
  }

  Future<void> _deletePlant() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Delete Plant'),
        content: const Text(
          'Are you sure you want to delete this plant? This action cannot be undone.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('CANCEL'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('DELETE'),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      try {
        await _plantService.deletePlant(widget.plantId);
        if (mounted) {
          Navigator.pop(context);
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Error deleting plant: $e')),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: _isLoading ? const Text('Plant Details') : Text(_plant!.name),
        actions: [
          if (!_isLoading && _plant != null)
            IconButton(
              icon: const Icon(Icons.edit),
              onPressed: () {
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => AddEditPlantScreen(
                      plant: _plant,
                    ),
                  ),
                ).then((_) => _loadPlant());
              },
            ),
          if (!_isLoading && _plant != null)
            IconButton(
              icon: const Icon(Icons.delete),
              onPressed: _deletePlant,
            ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _plant == null
              ? const Center(child: Text('Plant not found'))
              : SingleChildScrollView(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Plant image
                      if (_plant!.imagePath != null)
                        Container(
                          height: 200,
                          width: double.infinity,
                          decoration: BoxDecoration(
                            borderRadius: BorderRadius.circular(12),
                            image: DecorationImage(
                              image: FileImage(File(_plant!.imagePath!)),
                              fit: BoxFit.cover,
                            ),
                          ),
                        )
                      else
                        Container(
                          height: 200,
                          width: double.infinity,
                          decoration: BoxDecoration(
                            color: Colors.grey[300],
                            borderRadius: BorderRadius.circular(12),
                          ),
                          child: const Icon(
                            Icons.local_florist,
                            size: 64,
                            color: AppTheme.primaryColor,
                          ),
                        ),
                      const SizedBox(height: 16),

                      // Plant info
                      Text(
                        _plant!.name,
                        style: Theme.of(context).textTheme.displayMedium,
                      ),
                      const SizedBox(height: 4),
                      Text(
                        _plant!.species,
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                              fontStyle: FontStyle.italic,
                              color: Colors.grey[700],
                            ),
                      ),
                      const SizedBox(height: 8),
                      Row(
                        children: [
                          const Icon(
                            Icons.location_on_outlined,
                            size: 16,
                            color: AppTheme.primaryColor,
                          ),
                          const SizedBox(width: 4),
                          Text(
                            _plant!.location,
                            style: Theme.of(context).textTheme.bodyMedium,
                          ),
                        ],
                      ),
                      const SizedBox(height: 24),

                      // Care instructions
                      Text(
                        'Care Instructions',
                        style: Theme.of(context).textTheme.headlineMedium,
                      ),
                      const SizedBox(height: 8),
                      if (_plant!.careInstructions.isEmpty)
                        Card(
                          child: Padding(
                            padding: const EdgeInsets.all(16),
                            child: Column(
                              children: [
                                const Icon(
                                  Icons.info_outline,
                                  size: 48,
                                  color: Colors.grey,
                                ),
                                const SizedBox(height: 8),
                                Text(
                                  'No care instructions added',
                                  style: Theme.of(context)
                                      .textTheme
                                      .bodyLarge
                                      ?.copyWith(
                                        color: Colors.grey[600],
                                      ),
                                ),
                              ],
                            ),
                          ),
                        )
                      else
                        Column(
                          children: _plant!.careInstructions
                              .map(
                                (instruction) => CareInstructionCard(
                                  instruction: instruction,
                                ),
                              )
                              .toList(),
                        ),
                    ],
                  ),
                ),
    );
  }
}

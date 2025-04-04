import 'package:flutter/material.dart';
import '../models/plant.dart';
import '../models/care_instruction.dart';
import '../services/plant_service.dart';
import '../utils/constants.dart';
import '../utils/theme.dart';
import '../widgets/custom_input_fields.dart';

// Define the CareInstructionFormData class at the top of the file
class CareInstructionFormData {
  final String type;
  final TextEditingController descriptionController;
  String frequency;

  CareInstructionFormData({
    required this.type,
    required this.descriptionController,
    required this.frequency,
  });
}

class AddEditPlantScreen extends StatefulWidget {
  final Plant? plant;

  const AddEditPlantScreen({
    super.key,
    this.plant,
  });

  @override
  AddEditPlantScreenState createState() => AddEditPlantScreenState();
}

class AddEditPlantScreenState extends State<AddEditPlantScreen> {
  final _formKey = GlobalKey<FormState>();
  final PlantService _plantService = PlantService();

  final _nameController = TextEditingController();
  final _speciesController = TextEditingController();
  String _location = AppConstants.plantLocations.first;
  String? _imagePath;
  List<CareInstructionFormData> _careInstructions = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _initializeFormData();
  }

  void _initializeFormData() {
    if (widget.plant != null) {
      // Editing an existing plant
      _nameController.text = widget.plant!.name;
      _speciesController.text = widget.plant!.species;
      _location = widget.plant!.location;
      _imagePath = widget.plant!.imagePath;
      
      // Convert care instructions to form data
      _careInstructions = widget.plant!.careInstructions
          .map((instruction) => CareInstructionFormData(
                type: instruction.type,
                descriptionController:
                    TextEditingController(text: instruction.description),
                frequency: instruction.frequency,
              ))
          .toList();
    } else {
      // Adding a new plant, add default water and light instructions
      _careInstructions = [
        CareInstructionFormData(
          type: 'Water',
          descriptionController: TextEditingController(),
          frequency: AppConstants.wateringFrequencies.first,
        ),
        CareInstructionFormData(
          type: 'Light',
          descriptionController: TextEditingController(),
          frequency: AppConstants.wateringFrequencies.first,
        ),
      ];
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _speciesController.dispose();
    for (var instruction in _careInstructions) {
      instruction.descriptionController.dispose();
    }
    super.dispose();
  }

  Future<void> _pickImage() async {
    final imagePath = await _plantService.pickImage();
    if (imagePath != null) {
      setState(() {
        _imagePath = imagePath;
      });
    }
  }

  Future<void> _takePhoto() async {
    final imagePath = await _plantService.takePhoto();
    if (imagePath != null) {
      setState(() {
        _imagePath = imagePath;
      });
    }
  }

  void _removeImage() {
    setState(() {
      _imagePath = null;
    });
  }

  void _addCareInstruction() {
    setState(() {
      _careInstructions.add(
        CareInstructionFormData(
          type: AppConstants.careTypes.first,
          descriptionController: TextEditingController(),
          frequency: AppConstants.wateringFrequencies.first,
        ),
      );
    });
  }

  void _removeCareInstruction(int index) {
    setState(() {
      _careInstructions[index].descriptionController.dispose();
      _careInstructions.removeAt(index);
    });
  }

  Future<void> _savePlant() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isLoading = true;
    });

    try {
      // Convert form data to care instructions
      final careInstructions = _careInstructions
          .map((formData) => CareInstruction(
                id: null,
                plantId: widget.plant?.id,
                type: formData.type,
                description: formData.descriptionController.text,
                frequency: formData.frequency,
              ))
          .toList();

      // Create or update plant
      if (widget.plant == null) {
        // Create new plant
        final newPlant = Plant(
          name: _nameController.text,
          species: _speciesController.text,
          location: _location,
          imagePath: _imagePath,
          careInstructions: careInstructions,
        );

        await _plantService.addPlant(newPlant);
      } else {
        // Update existing plant
        final updatedPlant = widget.plant!.copyWith(
          name: _nameController.text,
          species: _speciesController.text,
          location: _location,
          imagePath: _imagePath,
          careInstructions: careInstructions,
        );

        await _plantService.updatePlant(updatedPlant);
      }

      if (mounted) {
        Navigator.pop(context);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error saving plant: $e')),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.plant != null;
    
    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing ? 'Edit Plant' : 'Add Plant'),
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : Form(
              key: _formKey,
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Image picker
                    CustomImagePicker(
                      imagePath: _imagePath,
                      onPickImage: _pickImage,
                      onTakePhoto: _takePhoto,
                      onRemoveImage: _imagePath != null ? _removeImage : null,
                    ),
                    const SizedBox(height: 24),

                    // Plant info
                    Text(
                      'Plant Information',
                      style: Theme.of(context).textTheme.headlineMedium,
                    ),
                    const SizedBox(height: 16),
                    CustomTextField(
                      label: 'Plant Name',
                      controller: _nameController,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter a name';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),
                    CustomAutocompleteField(
                      label: 'Species',
                      controller: _speciesController,
                      options: AppConstants.commonPlants
                          .map((plant) => plant['species']!)
                          .toList(),
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter a species';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 16),
                    CustomDropdownField(
                      label: 'Location',
                      value: _location,
                      items: AppConstants.plantLocations,
                      onChanged: (value) {
                        if (value != null) {
                          setState(() {
                            _location = value;
                          });
                        }
                      },
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please select a location';
                        }
                        return null;
                      },
                    ),
                    const SizedBox(height: 24),

                    // Care instructions
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          'Care Instructions',
                          style: Theme.of(context).textTheme.headlineMedium,
                        ),
                        ElevatedButton.icon(
                          onPressed: _addCareInstruction,
                          icon: const Icon(Icons.add),
                          label: const Text('Add'),
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppTheme.secondaryColor,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 8),
                    ..._careInstructions.asMap().entries.map(
                          (entry) => _buildCareInstructionField(entry.key),
                        ),
                    const SizedBox(height: 32),

                    // Save button
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton(
                        onPressed: _savePlant,
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                        ),
                        child: Text(
                          isEditing ? 'Update Plant' : 'Add Plant',
                          style: const TextStyle(fontSize: 16),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _buildCareInstructionField(int index) {
    final instruction = _careInstructions[index];
    
    return Column(
      children: [
        CustomCareInstructionField(
          type: instruction.type,
          descriptionController: instruction.descriptionController,
          frequency: instruction.frequency,
          onFrequencyChanged: (value) {
            if (value != null) {
              setState(() {
                instruction.frequency = value;
              });
            }
          },
          onDelete: () => _removeCareInstruction(index),
        ),
        if (index < _careInstructions.length - 1) const SizedBox(height: 8),
      ],
    );
  }
}

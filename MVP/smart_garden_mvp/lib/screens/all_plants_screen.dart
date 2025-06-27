import 'package:flutter/material.dart';
import '../models/plant.dart';
import '../widgets/plant_card.dart';

class AllPlantsScreen extends StatefulWidget {
  final List<Plant> plants;

  const AllPlantsScreen({required this.plants, super.key});

  @override
  State<AllPlantsScreen> createState() => _AllPlantsScreenState();
}

class _AllPlantsScreenState extends State<AllPlantsScreen> {
  late List<Plant> sortedPlants;
  String selectedSort = 'Name A-Z';
  String selectedRoom = 'All';
  String selectedType = 'ALL';

  List<String> roomFilters = ['All', 'Bedroom 1', 'Bedroom 2', 'Living Room', 'Kitchen', 'Bathroom'];
  List<String> typeFilters = ['ALL', 'INDOOR', 'OUTDOOR'];

  @override
  void initState() {
    super.initState();
    sortedPlants = [...widget.plants];
    _sortPlants();
  }

  void _sortPlants() {
    setState(() {
      switch (selectedSort) {
        case 'Name A-Z':
          sortedPlants.sort((a, b) => a.name.compareTo(b.name));
          break;
        case 'Name Z-A':
          sortedPlants.sort((a, b) => b.name.compareTo(a.name));
          break;
        case 'Min Soil ↑':
          sortedPlants.sort((a, b) => a.minSoil.compareTo(b.minSoil));
          break;
        case 'Min Soil ↓':
          sortedPlants.sort((a, b) => b.minSoil.compareTo(a.minSoil));
          break;
        case 'Type':
          sortedPlants.sort((a, b) => a.type.compareTo(b.type));
          break;
      }
    });
  }

  List<Plant> get filteredPlants {
    return sortedPlants.where((p) {
      final roomMatch = selectedRoom == 'All' || p.room == selectedRoom;
      final typeMatch = selectedType == 'ALL' || p.type == selectedType;
      return roomMatch && typeMatch;

    }).toList();
  }

  Widget buildFilterButtons(List<String> options, String selected, void Function(String) onSelected) {
    return Wrap(
      spacing: 8,
      runSpacing: 8,
      children: options.map((option) {
        final isSelected = option == selected;
        return ChoiceChip(
          label: Text(option),
          selected: isSelected,
          selectedColor: Colors.deepPurple.shade100,
          onSelected: (_) => onSelected(option),
          labelStyle: TextStyle(
            color: isSelected ? Colors.deepPurple : Colors.black,
            fontWeight: isSelected ? FontWeight.bold : FontWeight.normal,
          ),
          backgroundColor: Colors.grey.shade200,
        );
      }).toList(),
    );
  }

  @override
  Widget build(BuildContext context) {
    final filtered = filteredPlants;

    return Scaffold(
      appBar: AppBar(
        title: const Text('All Plants'),
        actions: [
          DropdownButton<String>(
            value: selectedSort,
            dropdownColor: Colors.white,
            icon: const Icon(Icons.sort, color: Colors.white),
            underline: const SizedBox(),
            onChanged: (String? newValue) {
              if (newValue != null) {
                selectedSort = newValue;
                _sortPlants();
              }
            },
            items: <String>[
              'Name A-Z',
              'Name Z-A',
              'Min Soil ↑',
              'Min Soil ↓',
              'Type',
            ].map((String value) {
              return DropdownMenuItem<String>(
                value: value,
                child: Text(value, style: const TextStyle(color: Colors.black)),
              );
            }).toList(),
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text("My plants (${filtered.length})", style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
            const SizedBox(height: 12),
            buildFilterButtons(roomFilters, selectedRoom, (value) {
              setState(() => selectedRoom = value);
            }),
            const SizedBox(height: 12),
            buildFilterButtons(typeFilters, selectedType, (value) {
              setState(() => selectedType = value);
            }),
            const SizedBox(height: 16),
            Expanded(
              child: GridView.builder(
                itemCount: filtered.length,
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 2,
                  crossAxisSpacing: 12,
                  mainAxisSpacing: 12,
                  childAspectRatio: 0.75,
                ),
                itemBuilder: (context, index) {
                  final plant = filtered[index];
                  return PlantCard(
                    plant: plant,
                    onDetails: () {
                      Navigator.pushNamed(context, '/plant-details', arguments: plant);
                    },
                  );
                },
              ),
            ),
          ],
        ),
      ),
    );
  }
}

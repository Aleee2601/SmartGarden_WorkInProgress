import 'package:flutter/material.dart';
import '../models/plant.dart';
import '../widgets/plant_card.dart';
import 'all_plants_screen.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  final List<String> rooms = ['All', 'Bedroom 1', 'Bedroom 2', 'Living Room', 'Kitchen', 'Bathroom'];
  String selectedRoom = 'All';
  String selectedType = 'ALL';

  final List<Plant> allPlants = [
    Plant(
      id: 1,
      name: 'Ficus lyrata',
      userId: 1,
      minSoil: 30,
      maxSoil: 60,
      type: 'INDOOR',
      room: 'Bedroom 1',
    ),
    Plant(
      id: 2,
      name: 'Zamioculcas',
      userId: 1,
      minSoil: 20,
      maxSoil: 50,
      type: 'INDOOR',
      room: 'Living Room',
    ),
    Plant(
      id: 3,
      name: 'Palm',
      userId: 1,
      minSoil: 40,
      maxSoil: 70,
      type: 'OUTDOOR',
      room: 'Kitchen',
    ),
    Plant(
      id: 4,
      name: 'Snake Plant',
      userId: 1,
      minSoil: 25,
      maxSoil: 55,
      type: 'INDOOR',
      room: 'Bathroom',
    ),
    Plant(
      id: 5,
      name: 'Bamboo Palm',
      userId: 1,
      minSoil: 35,
      maxSoil: 65,
      type: 'OUTDOOR',
      room: 'Bedroom 2',
    ),
  ];

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final cardWidth = screenWidth / 3.2;

    final List<Plant> filteredPlants = allPlants.where((p) {
      final roomMatch = selectedRoom == 'All' || p.room == selectedRoom;
      final typeMatch = selectedType == 'ALL' || p.type == selectedType;
      return roomMatch && typeMatch;
    }).toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('SmartGarden'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              "My plants (${filteredPlants.length})",
              style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),

            const SizedBox(height: 16),

            // Room filter
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: rooms.map((room) {
                  final isSelected = selectedRoom == room;
                  return Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 4.0),
                    child: ChoiceChip(
                      label: Text(room),
                      selected: isSelected,
                      onSelected: (_) {
                        setState(() {
                          selectedRoom = room;
                        });
                      },
                    ),
                  );
                }).toList(),
              ),
            ),

            const SizedBox(height: 12),

            // Type filter
            Row(
              children: ['ALL', 'INDOOR', 'OUTDOOR'].map((type) {
                final isSelected = selectedType == type;
                return Padding(
                  padding: const EdgeInsets.only(right: 8.0),
                  child: ChoiceChip(
                    label: Text(type),
                    selected: isSelected,
                    onSelected: (_) {
                      setState(() {
                        selectedType = type;
                      });
                    },
                  ),
                );
              }).toList(),
            ),

            const SizedBox(height: 20),

           // Slider with margin perfect left-right
              SizedBox(
                height: 320,
                child: ListView.builder(
                  scrollDirection: Axis.horizontal,
                  physics: const BouncingScrollPhysics(),
                  itemCount: filteredPlants.length,
                  padding: const EdgeInsets.symmetric(horizontal: 16), // margini globale
                  itemBuilder: (context, index) {
                    final plant = filteredPlants[index];
                    return Container(
                      margin: EdgeInsets.only(
                        right: index == filteredPlants.length - 1 ? 0 : 12, // spațiu doar între carduri
                      ),
                      width: cardWidth,
                      child: PlantCard(
                        plant: plant,
                        onDetails: () {
                          Navigator.pushNamed(context, '/plant-details', arguments: plant);
                        },
                      ),
                    );
                  },
                ),
              ),

            const SizedBox(height: 12),

            // See more button
            Align(
              alignment: Alignment.centerRight,
              child: TextButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => AllPlantsScreen(plants: allPlants),
                    ),
                  );
                },
                child: const Text('See more'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

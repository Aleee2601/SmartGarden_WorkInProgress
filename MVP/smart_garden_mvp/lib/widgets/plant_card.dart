import 'package:flutter/material.dart';
import '../models/plant.dart';
import '../widgets/watering_button.dart';

class PlantCard extends StatelessWidget {
  final Plant plant;
  final VoidCallback onDetails;

  const PlantCard({required this.plant, required this.onDetails, super.key});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        title: Text(plant.name),
        subtitle: Text("Range: ${plant.minSoil}% - ${plant.maxSoil}%"),
        trailing: WateringButton(plantId: plant.id),
        onTap: onDetails,
      ),
    );
  }
}

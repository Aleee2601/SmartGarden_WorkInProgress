import 'package:flutter/material.dart';

class SensorDisplay extends StatelessWidget {
  final String title;
  final double value;
  final String unit;

  const SensorDisplay({super.key, required this.title, required this.value, required this.unit});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text("$title: ${value.toStringAsFixed(1)} $unit", style: TextStyle(fontSize: 16)),
        SizedBox(height: 8),
      ],
    );
  }
}

import 'package:flutter/material.dart';
import '../models/plant.dart';

class PlantDetailsScreen extends StatelessWidget {
  final Plant plant;

  const PlantDetailsScreen({super.key, required this.plant});

  @override
  Widget build(BuildContext context) {
    final height = MediaQuery.of(context).size.height;
    final width = MediaQuery.of(context).size.width;

    double waterTank = 10;
    double light = 78;
    double temperature = 24;
    double soilMoisture = 46;

    return Scaffold(
      body: Column(
        children: [
          // Secțiunea fixă de sus (verde)
          Container(
            height: height * 0.5,
            color: Colors.green.shade700,
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: [
                // Imagine plantă
                Container(
                  height: double.infinity,
                  width: width * 0.4,
                  decoration: BoxDecoration(
                    color: Colors.black12,
                    borderRadius: BorderRadius.circular(24),
                  ),
                  alignment: Alignment.center,
                  child: const Text(
                    'Plant Image',
                    style: TextStyle(color: Colors.black45),
                  ),
                ),
                const SizedBox(width: 16),
                // Detalii plantă + alertă
                Expanded(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        plant.name,
                        style: const TextStyle(
                            fontSize: 22,
                            color: Colors.white,
                            fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 4),
                      const Text("26 weeks",
                          style: TextStyle(color: Colors.white70)),
                      const SizedBox(height: 12),
                      _sensorRow(Icons.water_drop, '19%', 'Humidity'),
                      _sensorRow(Icons.eco, '86%', 'Fertilizer'),
                      _sensorRow(Icons.timer, '36 min', 'Next watering'),
                      const SizedBox(height: 24),
                      Container(
                        padding: const EdgeInsets.all(12),
                        decoration: BoxDecoration(
                          color: Colors.red.shade100,
                          borderRadius: BorderRadius.circular(12),
                        ),
                        child: const Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.warning_amber, color: Colors.red),
                            SizedBox(width: 8),
                            Flexible(
                              child: Text(
                                'Please fill the water tank!',
                                style: TextStyle(
                                    color: Colors.red,
                                    fontWeight: FontWeight.bold),
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),

          // Secțiunea scrollabilă de jos (albă)
          Expanded(
            child: Container(
              decoration: const BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.vertical(top: Radius.circular(32)),
              ),
              child: ListView(
                padding: const EdgeInsets.all(24),
                children: [
                  // Statistici senzori
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      _valueStat('Water tank', '${waterTank.toInt()}%',
                          Icons.water_drop, Colors.red),
                      _valueStat('Light', '${light.toInt()}°',
                          Icons.light_mode, Colors.orange),
                      _valueStat('Temper.', '${temperature.toInt()}°C',
                          Icons.thermostat, Colors.blue),
                      _valueStat('Moisture', '${soilMoisture.toInt()}%',
                          Icons.grass, Colors.green),
                    ],
                  ),
                  const SizedBox(height: 20),

                  // Udare automată
                  const Text('Automatic watering',
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      ElevatedButton(
                        onPressed: () {},
                        style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.green),
                        child: const Text('ON'),
                      ),
                      const SizedBox(width: 12),
                      OutlinedButton(
                        onPressed: () {},
                        child: const Text('OFF'),
                      ),
                    ],
                  ),
                  const SizedBox(height: 24),

                  // Grafic placeholder
                  const Text('Weekly Stats',
                      style: TextStyle(fontWeight: FontWeight.bold)),
                  const SizedBox(height: 12),
                  Container(
                    height: 100,
                    width: double.infinity,
                    decoration: BoxDecoration(
                      color: Colors.grey.shade200,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    alignment: Alignment.center,
                    child: const Text('Graph Placeholder'),
                  ),
                  const SizedBox(height: 80), // scroll extra
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  // 🔧 Widget mic pentru senzor
  static Widget _sensorRow(IconData icon, String value, String label) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 6),
      child: Row(
        children: [
          Icon(icon, color: Colors.white, size: 20),
          const SizedBox(width: 6),
          Text(value,
              style:
                  const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
          const SizedBox(width: 4),
          Text(label, style: const TextStyle(color: Colors.white70)),
        ],
      ),
    );
  }

  // 🔢 Widget pentru fiecare statistică (jos)
  static Widget _valueStat(
      String label, String value, IconData icon, Color color) {
    return Column(
      children: [
        Icon(icon, color: color),
        const SizedBox(height: 4),
        Text(
          value,
          style:
              TextStyle(color: color, fontSize: 18, fontWeight: FontWeight.bold),
        ),
        Text(label, style: const TextStyle(fontSize: 12)),
      ],
    );
  }
}

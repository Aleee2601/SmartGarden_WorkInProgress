import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
// import '../services/db_service.dart'; // Pentru salvare în backend

class WateringButton extends StatelessWidget {
  final int plantId;

  const WateringButton({super.key, required this.plantId});

  Future<void> waterPlant() async {
    await http.post(
      Uri.parse('http://192.168.0.123/water'),
      headers: {'Content-Type': 'text/plain'},
      body: 'WATER ON',
    );

    // TODO: Salvează udarea și în backend dacă vrei:
    // await DbService.postWateringLog(plantId, token);
  }

  @override
  Widget build(BuildContext context) {
    return IconButton(
      icon: Icon(Icons.water_drop),
      onPressed: waterPlant,
      color: Colors.blue,
    );
  }
}

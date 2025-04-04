import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:async';

void main() => runApp(SmartGardenApp());

class SmartGardenApp extends StatefulWidget {
  @override
  _SmartGardenAppState createState() => _SmartGardenAppState();
}

class _SmartGardenAppState extends State<SmartGardenApp> {
  int moisture = 0;
  double temperature = 0;
  double humidity = 0;

  final String espUrl = "http://192.168.1.100"; // Replace with your ESP32 IP
  Timer? _timer;

  Future<void> fetchData() async {
    try {
      final response = await http.get(Uri.parse("$espUrl/status"));
      if (response.statusCode == 200) {
        final Map<String, dynamic> data = json.decode(response.body);
        if (data.containsKey('moisture') &&
            data.containsKey('temperature') &&
            data.containsKey('humidity')) {
          setState(() {
            moisture = data['moisture'];
            temperature = data['temperature'].toDouble();
            humidity = data['humidity'].toDouble();
          });
        } else {
          print("Error: Incomplete data received from ESP32");
        }
      } else {
        print("Error: Failed to fetch data from ESP32");
      }
    } catch (e) {
      print("Error fetching data: $e");
    }
  }

  Future<void> waterPlant() async {
    try {
      await http.post(Uri.parse("$espUrl/water"));
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text("Watering triggered!")));
    } catch (e) {
      print("Error sending watering request: $e");
    }
  }

  @override
  void initState() {
    super.initState();
    fetchData();
    _timer = Timer.periodic(Duration(seconds: 5), (timer) {
      fetchData();
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(title: Text("SmartGarden")),
        body: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            children: [
              Text("Moisture: $moisture"),
              Text("Temperature: ${temperature.toStringAsFixed(1)}°C"),
            ],
          ),
        ),
      ),
    );
  }
}

import 'package:flutter/material.dart';
import '../services/db_service.dart';
import '../models/watering_log.dart';

class WateringHistoryScreen extends StatefulWidget {
  final int plantId;
  final String token;

  const WateringHistoryScreen({super.key, required this.plantId, required this.token});

  @override
  _WateringHistoryScreenState createState() => _WateringHistoryScreenState();
}

class _WateringHistoryScreenState extends State<WateringHistoryScreen> {
  List<WateringLog> logs = [];

  @override
  void initState() {
    super.initState();
    loadLogs();
  }

  void loadLogs() async {
    final result = await DbService.getWateringLogs(widget.plantId, widget.token);
    setState(() => logs = result);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Istoric Udări')),
      body: ListView.builder(
        itemCount: logs.length,
        itemBuilder: (context, index) {
          final log = logs[index];
          return ListTile(
            title: Text("Udare #${log.id}"),
            subtitle: Text("Data: ${log.timestamp.toLocal()}"),
          );
        },
      ),
    );
  }
}

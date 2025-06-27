import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/plant.dart';
import '../models/watering_log.dart';

class DbService {
  static const baseUrl = 'http://localhost:5000/api';

  static Future<List<Plant>> getPlants(String token) async {
    final response = await http.get(
      Uri.parse('$baseUrl/plants'),
      headers: {'Authorization': 'Bearer $token'},
    );
    if (response.statusCode == 200) {
      final List<dynamic> jsonList = jsonDecode(response.body);
      return jsonList.map((json) => Plant.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load plants');
    }
  }

  static Future<List<WateringLog>> getWateringLogs(int plantId, String token) async {
    final response = await http.get(
      Uri.parse('$baseUrl/watering-log?plantId=$plantId'),
      headers: {'Authorization': 'Bearer $token'},
    );
    if (response.statusCode == 200) {
      final List<dynamic> jsonList = jsonDecode(response.body);
      return jsonList.map((json) => WateringLog.fromJson(json)).toList();
    } else {
      throw Exception('Failed to load logs');
    }
  }
}

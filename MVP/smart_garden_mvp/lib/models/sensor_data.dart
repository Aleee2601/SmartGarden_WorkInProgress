class SensorData {
  final double temperature;
  final double humidity;
  final int soil;

  SensorData({required this.temperature, required this.humidity, required this.soil});

  factory SensorData.fromJson(Map<String, dynamic> json) {
    return SensorData(
      temperature: (json['temperature'] as num).toDouble(),
      humidity: (json['humidity'] as num).toDouble(),
      soil: json['soil'],
    );
  }
}

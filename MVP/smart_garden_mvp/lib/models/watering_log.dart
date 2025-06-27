class WateringLog {
  final int id;
  final int plantId;
  final DateTime timestamp;

  WateringLog({
    required this.id,
    required this.plantId,
    required this.timestamp,
  });

  factory WateringLog.fromJson(Map<String, dynamic> json) {
    return WateringLog(
      id: json['id'],
      plantId: json['plantId'],
      timestamp: DateTime.parse(json['timestamp']),
    );
  }
}

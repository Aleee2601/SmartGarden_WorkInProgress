class Plant {
  final int id;
  final String name;
  final int userId;
  final int minSoil;
  final int maxSoil;
  final String type; // 'INDOOR' sau 'OUTDOOR'
  final String room; // ex: 'Bedroom 1', 'Living Room'

  Plant({
    required this.id,
    required this.name,
    required this.userId,
    required this.minSoil,
    required this.maxSoil,
    required this.type,
    required this.room,
  });

  factory Plant.fromJson(Map<String, dynamic> json) {
    return Plant(
      id: json['id'],
      name: json['name'],
      userId: json['userId'],
      minSoil: json['minSoil'],
      maxSoil: json['maxSoil'],
      type: json['type'],
      room: json['room'],
    );
  }
}

class CareInstruction {
  final int? id;
  final int? plantId;
  final String type; // water, light, fertilize, etc.
  final String description;
  final String frequency;

  CareInstruction({
    this.id,
    this.plantId,
    required this.type,
    required this.description,
    required this.frequency,
  });

  // Convert a CareInstruction into a Map
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'plantId': plantId,
      'type': type,
      'description': description,
      'frequency': frequency,
    };
  }

  // Create a CareInstruction from a Map
  factory CareInstruction.fromMap(Map<String, dynamic> map) {
    return CareInstruction(
      id: map['id'],
      plantId: map['plantId'],
      type: map['type'],
      description: map['description'],
      frequency: map['frequency'],
    );
  }

  // Create a copy of this CareInstruction with the given fields replaced with the new values
  CareInstruction copyWith({
    int? id,
    int? plantId,
    String? type,
    String? description,
    String? frequency,
  }) {
    return CareInstruction(
      id: id ?? this.id,
      plantId: plantId ?? this.plantId,
      type: type ?? this.type,
      description: description ?? this.description,
      frequency: frequency ?? this.frequency,
    );
  }
}

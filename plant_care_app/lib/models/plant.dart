import 'package:intl/intl.dart';
import 'care_instruction.dart';

class Plant {
  final int? id;
  final String name;
  final String species;
  final String location;
  final String? imagePath;
  final DateTime dateAdded;
  final List<CareInstruction> careInstructions;

  Plant({
    this.id,
    required this.name,
    required this.species,
    required this.location,
    this.imagePath,
    DateTime? dateAdded,
    List<CareInstruction>? careInstructions,
  }) : 
    dateAdded = dateAdded ?? DateTime.now(),
    careInstructions = careInstructions ?? [];

  // Convert a Plant into a Map
  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'name': name,
      'species': species,
      'location': location,
      'imagePath': imagePath,
      'dateAdded': DateFormat('yyyy-MM-dd').format(dateAdded),
    };
  }

  // Create a Plant from a Map
  factory Plant.fromMap(Map<String, dynamic> map, List<CareInstruction> instructions) {
    return Plant(
      id: map['id'],
      name: map['name'],
      species: map['species'],
      location: map['location'],
      imagePath: map['imagePath'],
      dateAdded: DateFormat('yyyy-MM-dd').parse(map['dateAdded']),
      careInstructions: instructions,
    );
  }

  // Create a copy of this Plant with the given fields replaced with the new values
  Plant copyWith({
    int? id,
    String? name,
    String? species,
    String? location,
    String? imagePath,
    DateTime? dateAdded,
    List<CareInstruction>? careInstructions,
  }) {
    return Plant(
      id: id ?? this.id,
      name: name ?? this.name,
      species: species ?? this.species,
      location: location ?? this.location,
      imagePath: imagePath ?? this.imagePath,
      dateAdded: dateAdded ?? this.dateAdded,
      careInstructions: careInstructions ?? this.careInstructions,
    );
  }
}

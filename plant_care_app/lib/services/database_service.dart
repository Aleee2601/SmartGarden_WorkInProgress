import 'dart:async';
import 'package:path/path.dart';
import 'package:sqflite/sqflite.dart';
import 'package:path_provider/path_provider.dart';
import '../models/plant.dart';
import '../models/care_instruction.dart';

class DatabaseService {
  static final DatabaseService _instance = DatabaseService._internal();
  static Database? _database;

  // Singleton pattern
  factory DatabaseService() => _instance;

  DatabaseService._internal();

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDatabase();
    return _database!;
  }

  Future<Database> _initDatabase() async {
    final documentsDirectory = await getApplicationDocumentsDirectory();
    final path = join(documentsDirectory.path, 'plant_care.db');
    return await openDatabase(
      path,
      version: 1,
      onCreate: _createDb,
    );
  }

  Future<void> _createDb(Database db, int version) async {
    // Create plants table
    await db.execute('''
      CREATE TABLE plants(
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        species TEXT NOT NULL,
        location TEXT NOT NULL,
        imagePath TEXT,
        dateAdded TEXT NOT NULL
      )
    ''');

    // Create care instructions table
    await db.execute('''
      CREATE TABLE care_instructions(
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        plantId INTEGER NOT NULL,
        type TEXT NOT NULL,
        description TEXT NOT NULL,
        frequency TEXT NOT NULL,
        FOREIGN KEY (plantId) REFERENCES plants (id) ON DELETE CASCADE
      )
    ''');

    // Insert some default plants
    await _insertDefaultPlants(db);
  }

  Future<void> _insertDefaultPlants(Database db) async {
    // Insert some default plants with care instructions
    final plant1Id = await db.insert('plants', {
      'name': 'Peace Lily',
      'species': 'Spathiphyllum',
      'location': 'Living Room',
      'imagePath': null,
      'dateAdded': DateTime.now().toString(),
    });

    await db.insert('care_instructions', {
      'plantId': plant1Id,
      'type': 'Water',
      'description': 'Keep soil moist but not soggy',
      'frequency': 'Once a week',
    });

    await db.insert('care_instructions', {
      'plantId': plant1Id,
      'type': 'Light',
      'description': 'Indirect light, no direct sunlight',
      'frequency': 'Daily',
    });

    final plant2Id = await db.insert('plants', {
      'name': 'Snake Plant',
      'species': 'Sansevieria trifasciata',
      'location': 'Bedroom',
      'imagePath': null,
      'dateAdded': DateTime.now().toString(),
    });

    await db.insert('care_instructions', {
      'plantId': plant2Id,
      'type': 'Water',
      'description': 'Allow soil to dry between waterings',
      'frequency': 'Every 2-3 weeks',
    });

    await db.insert('care_instructions', {
      'plantId': plant2Id,
      'type': 'Light',
      'description': 'Tolerates low light but prefers bright indirect light',
      'frequency': 'Daily',
    });
  }

  // CRUD operations for plants

  Future<int> insertPlant(Plant plant) async {
    final db = await database;
    final plantId = await db.insert('plants', plant.toMap());
    
    // Insert care instructions
    for (var instruction in plant.careInstructions) {
      await db.insert('care_instructions', {
        ...instruction.toMap(),
        'plantId': plantId,
      });
    }
    
    return plantId;
  }

  Future<List<Plant>> getPlants() async {
    final db = await database;
    final plantsData = await db.query('plants');
    
    return Future.wait(plantsData.map((plantData) async {
      final careInstructionsData = await db.query(
        'care_instructions',
        where: 'plantId = ?',
        whereArgs: [plantData['id']],
      );
      
      final careInstructions = careInstructionsData
          .map((data) => CareInstruction.fromMap(data))
          .toList();
      
      return Plant.fromMap(plantData, careInstructions);
    }).toList());
  }

  Future<Plant?> getPlant(int id) async {
    final db = await database;
    final plantsData = await db.query(
      'plants',
      where: 'id = ?',
      whereArgs: [id],
    );
    
    if (plantsData.isEmpty) {
      return null;
    }
    
    final careInstructionsData = await db.query(
      'care_instructions',
      where: 'plantId = ?',
      whereArgs: [id],
    );
    
    final careInstructions = careInstructionsData
        .map((data) => CareInstruction.fromMap(data))
        .toList();
    
    return Plant.fromMap(plantsData.first, careInstructions);
  }

  Future<int> updatePlant(Plant plant) async {
    final db = await database;
    
    // Update plant
    final result = await db.update(
      'plants',
      plant.toMap(),
      where: 'id = ?',
      whereArgs: [plant.id],
    );
    
    // Delete existing care instructions
    await db.delete(
      'care_instructions',
      where: 'plantId = ?',
      whereArgs: [plant.id],
    );
    
    // Insert new care instructions
    for (var instruction in plant.careInstructions) {
      await db.insert('care_instructions', {
        ...instruction.toMap(),
        'plantId': plant.id,
      });
    }
    
    return result;
  }

  Future<int> deletePlant(int id) async {
    final db = await database;
    
    // Delete care instructions (should cascade due to foreign key)
    await db.delete(
      'care_instructions',
      where: 'plantId = ?',
      whereArgs: [id],
    );
    
    // Delete plant
    return await db.delete(
      'plants',
      where: 'id = ?',
      whereArgs: [id],
    );
  }
}

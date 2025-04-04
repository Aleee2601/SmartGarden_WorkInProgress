import 'package:flutter/material.dart';
import '../models/care_instruction.dart';
import '../utils/theme.dart';

class CareInstructionCard extends StatelessWidget {
  final CareInstruction instruction;
  final VoidCallback? onEdit;
  final VoidCallback? onDelete;

  const CareInstructionCard({
    super.key,
    required this.instruction,
    this.onEdit,
    this.onDelete,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 8),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    _getIconForType(),
                    const SizedBox(width: 8),
                    Text(
                      instruction.type,
                      style: Theme.of(context).textTheme.headlineMedium,
                    ),
                  ],
                ),
                if (onEdit != null || onDelete != null)
                  Row(
                    children: [
                      if (onEdit != null)
                        IconButton(
                          icon: const Icon(Icons.edit, color: AppTheme.primaryColor),
                          onPressed: onEdit,
                          tooltip: 'Edit',
                        ),
                      if (onDelete != null)
                        IconButton(
                          icon: const Icon(Icons.delete, color: AppTheme.errorColor),
                          onPressed: onDelete,
                          tooltip: 'Delete',
                        ),
                    ],
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              instruction.description,
              style: Theme.of(context).textTheme.bodyLarge,
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                const Icon(
                  Icons.schedule,
                  size: 16,
                  color: AppTheme.primaryColor,
                ),
                const SizedBox(width: 4),
                Text(
                  'Frequency: ${instruction.frequency}',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _getIconForType() {
    IconData iconData;
    
    switch (instruction.type.toLowerCase()) {
      case 'water':
        iconData = Icons.water_drop;
        break;
      case 'light':
        iconData = Icons.wb_sunny;
        break;
      case 'fertilizer':
        iconData = Icons.spa;
        break;
      case 'humidity':
        iconData = Icons.water;
        break;
      case 'temperature':
        iconData = Icons.thermostat;
        break;
      case 'soil':
        iconData = Icons.landscape;
        break;
      case 'pruning':
        iconData = Icons.content_cut;
        break;
      case 'repotting':
        iconData = Icons.swap_horiz;
        break;
      case 'pest control':
        iconData = Icons.bug_report;
        break;
      default:
        iconData = Icons.info;
    }
    
    return Icon(
      iconData,
      color: AppTheme.primaryColor,
      size: 24,
    );
  }
}

// This is a basic Flutter widget test.
//
// To perform an interaction with a widget in your test, use the WidgetTester
// utility in the flutter_test package. For example, you can send tap and scroll
// gestures. You can also use WidgetTester to find child widgets in the widget
// tree, read text, and verify that the values of widget properties are correct.

import 'package:flutter_test/flutter_test.dart';

import 'package:plant_care_app/main.dart';

void main() {
  testWidgets('App launches successfully', (WidgetTester tester) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(const PlantCareApp());

    // Verify that the app title is displayed
    expect(find.text('Welcome to Plant Care'), findsOneWidget);
    
    // Verify that quick actions are displayed
    expect(find.text('Quick Actions'), findsOneWidget);
    expect(find.text('Add Plant'), findsOneWidget);
    expect(find.text('View All'), findsOneWidget);
    
    // Verify that recent plants section is displayed
    expect(find.text('Recent Plants'), findsOneWidget);
  });
}

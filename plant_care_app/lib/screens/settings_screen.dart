import 'package:flutter/material.dart';
import '../utils/constants.dart';
import '../utils/theme.dart';

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  SettingsScreenState createState() => SettingsScreenState();
}

class SettingsScreenState extends State<SettingsScreen> {
  bool _darkMode = false;
  bool _notifications = true;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Settings'),
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // App info section
          _buildSectionHeader('App Info'),
          _buildInfoCard(
            title: 'Version',
            value: AppConstants.appVersion,
            icon: Icons.info_outline,
          ),
          const SizedBox(height: 24),

          // Appearance section
          _buildSectionHeader('Appearance'),
          _buildSwitchTile(
            title: 'Dark Mode',
            subtitle: 'Switch between light and dark theme',
            value: _darkMode,
            icon: Icons.dark_mode,
            onChanged: (value) {
              setState(() {
                _darkMode = value;
              });
              // In a real app, this would update the theme
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Dark mode is not implemented in this demo'),
                ),
              );
            },
          ),
          const SizedBox(height: 24),

          // Notifications section
          _buildSectionHeader('Notifications'),
          _buildSwitchTile(
            title: 'Enable Notifications',
            subtitle: 'Get reminders for plant care',
            value: _notifications,
            icon: Icons.notifications,
            onChanged: (value) {
              setState(() {
                _notifications = value;
              });
              // In a real app, this would update notification settings
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Notifications are not implemented in this demo'),
                ),
              );
            },
          ),
          const SizedBox(height: 24),

          // About section
          _buildSectionHeader('About'),
          _buildListTile(
            title: 'About Plant Care',
            subtitle: 'Learn more about this app',
            icon: Icons.help_outline,
            onTap: () {
              _showAboutDialog();
            },
          ),
          _buildListTile(
            title: 'Privacy Policy',
            subtitle: 'Read our privacy policy',
            icon: Icons.privacy_tip_outlined,
            onTap: () {
              // In a real app, this would open the privacy policy
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Privacy policy is not available in this demo'),
                ),
              );
            },
          ),
          _buildListTile(
            title: 'Terms of Service',
            subtitle: 'Read our terms of service',
            icon: Icons.description_outlined,
            onTap: () {
              // In a real app, this would open the terms of service
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Terms of service are not available in this demo'),
                ),
              );
            },
          ),
        ],
      ),
    );
  }

  Widget _buildSectionHeader(String title) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Text(
        title,
        style: Theme.of(context).textTheme.headlineMedium,
      ),
    );
  }

  Widget _buildInfoCard({
    required String title,
    required String value,
    required IconData icon,
  }) {
    return Card(
      child: ListTile(
        leading: Icon(icon, color: AppTheme.primaryColor),
        title: Text(title),
        trailing: Text(
          value,
          style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                fontWeight: FontWeight.bold,
              ),
        ),
      ),
    );
  }

  Widget _buildSwitchTile({
    required String title,
    required String subtitle,
    required bool value,
    required IconData icon,
    required ValueChanged<bool> onChanged,
  }) {
    return Card(
      child: SwitchListTile(
        title: Text(title),
        subtitle: Text(subtitle),
        value: value,
        onChanged: onChanged,
        secondary: Icon(icon, color: AppTheme.primaryColor),
      ),
    );
  }

  Widget _buildListTile({
    required String title,
    required String subtitle,
    required IconData icon,
    required VoidCallback onTap,
  }) {
    return Card(
      child: ListTile(
        leading: Icon(icon, color: AppTheme.primaryColor),
        title: Text(title),
        subtitle: Text(subtitle),
        trailing: const Icon(Icons.arrow_forward_ios, size: 16),
        onTap: onTap,
      ),
    );
  }

  void _showAboutDialog() {
    showDialog(
      context: context,
      builder: (context) => AboutDialog(
        applicationName: AppConstants.appName,
        applicationVersion: AppConstants.appVersion,
        applicationIcon: const Icon(
          Icons.local_florist,
          size: 48,
          color: AppTheme.primaryColor,
        ),
        children: [
          const SizedBox(height: 16),
          const Text(
            'Plant Care is a simple app to help you keep track of your plants and their care needs.',
          ),
          const SizedBox(height: 8),
          const Text(
            'This app was created as a demo for Flutter development.',
          ),
        ],
      ),
    );
  }
}

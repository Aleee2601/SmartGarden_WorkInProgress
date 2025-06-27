import 'package:flutter/material.dart';
import '../services/auth_service.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  _LoginScreenState createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  String error = '';

  void login() async {
    final email = emailController.text;
    final password = passwordController.text;

    final user = await AuthService.login(email, password);
    if (user != null) {
      print('Login successful. Token: ${user.token}');
      // TODO: Salvează token, navighează către Dashboard etc.
    } else {
      setState(() => error = 'Login failed.');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Login')),
      body: Padding(
        padding: const EdgeInsets.all(20.0),
        child: Column(
          children: [
            TextField(controller: emailController, decoration: InputDecoration(labelText: 'Email')),
            TextField(controller: passwordController, decoration: InputDecoration(labelText: 'Password'), obscureText: true),
            SizedBox(height: 20),
            ElevatedButton(onPressed: login, child: Text('Login')),
            if (error.isNotEmpty) Text(error, style: TextStyle(color: Colors.red)),
          ],
        ),
      ),
    );
  }
}

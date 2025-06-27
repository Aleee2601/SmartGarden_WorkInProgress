class User {
  final int id;
  final String email;
  final String token;

  User({required this.id, required this.email, required this.token});

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'],
      email: json['email'],
      token: json['token'],
    );
  }
}

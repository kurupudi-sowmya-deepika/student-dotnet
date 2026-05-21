# Student API

A .NET 10 REST API for managing students with JWT-based authentication and role-based authorization. Uses SQLite for storage and Swagger UI for interactive testing.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A terminal (PowerShell, Command Prompt, or VS Code integrated terminal)

---

## Getting Started

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd student-dotnet
```

### 2. Restore dependencies

```bash
cd StudentApi
dotnet restore
```

### 3. Apply database migrations

The app uses SQLite. Run the migration to create the `students.db` file:

```bash
dotnet ef database update
```

> If `dotnet ef` is not found, install it globally:
> ```bash
> dotnet tool install --global dotnet-ef
> ```

### 4. Run the API

```bash
dotnet run
```

The API starts at:
- HTTP:  `http://localhost:5112`
- HTTPS: `https://localhost:7015`

---

## Swagger UI (Interactive Docs)

Open your browser and go to:

```
http://localhost:5112/swagger
```

You can test all endpoints directly from the browser. Protected endpoints require a JWT token — click the **Authorize** button and paste your token.

---

## API Endpoints

### Auth (no token required)

| Method | Endpoint          | Description              |
|--------|-------------------|--------------------------|
| POST   | `/auth/register`  | Register a new user      |
| POST   | `/auth/login`     | Login and get JWT token  |

**Register request body:**
```json
{
  "username": "john",
  "email": "john@example.com",
  "password": "MyPassword123"
}
```

**Login request body:**
```json
{
  "email": "john@example.com",
  "password": "MyPassword123"
}
```

Both return an `AuthResponse` containing a JWT token. Copy this token to use in protected requests.

---

### Students (JWT token required)

| Method | Endpoint           | Role Required | Description            |
|--------|--------------------|---------------|------------------------|
| GET    | `/student`         | Any           | Get all students       |
| GET    | `/student/{id}`    | Any           | Get a student by ID    |
| POST   | `/student`         | Admin         | Create a new student   |
| PUT    | `/student`         | Admin         | Update a student       |
| DELETE | `/student`         | Admin         | Delete a student       |

**Student object:**
```json
{
  "id": 1,
  "name": "Jane Doe",
  "age": 21,
  "course": "Computer Science"
}
```

**How to pass the token:**

Add this header to your requests:
```
Authorization: Bearer <your-jwt-token>
```

---

## Project Structure

```
StudentApi/
├── Controllers/
│   ├── AuthController.cs      # Register & Login endpoints
│   └── StudentController.cs   # CRUD endpoints for students
├── Data/
│   └── AppDbContext.cs        # EF Core database context (SQLite)
├── Models/
│   ├── Student.cs             # Student entity
│   ├── User.cs                # User entity
│   └── DTOs/
│       ├── RegisterDto.cs
│       ├── LoginDto.cs
│       └── AuthResponseDto.cs
├── Services/
│   ├── StudentService.cs      # In-memory student data logic
│   ├── IAuthService.cs        # Auth service interface
│   └── AuthService.cs         # JWT token generation & BCrypt password hashing
├── appsettings.json           # Connection string & JWT config
└── Program.cs                 # App setup: middleware, DI, JWT auth
```

---

## Configuration

JWT settings are in [StudentApi/appsettings.json](StudentApi/appsettings.json):

```json
"JwtSettings": {
  "SecretKey": "StudentApiSuperSecretKey2024!@#XYZ_MustBe32Chars",
  "Issuer": "StudentApi",
  "Audience": "StudentApiUsers",
  "ExpiryMinutes": "60"
}
```

> For production, move `SecretKey` to environment variables or a secrets manager. Never commit real secrets to source control.

---

## Tech Stack

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Framework    | ASP.NET Core 10                     |
| Database     | SQLite via Entity Framework Core 10 |
| Auth         | JWT Bearer tokens                   |
| Password     | BCrypt (BCrypt.Net-Next)            |
| API Docs     | Swagger / Swashbuckle 10            |

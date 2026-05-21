# Student & User Management System (Full Stack)

A modern full-stack web application for managing students, featuring a **React (Vite) frontend** and an **ASP.NET Core 10 (Web API) backend**. The system implements secure JWT-based authentication and role-based authorization using an SQLite database.

---

## Project Structure

This repository is split into two primary components:

*   **[StudentApi](file:///c:/Sowmya/Practice/Project/student-dotnet/StudentApi/)**: The ASP.NET Core 10 Web API backend. Handled by EF Core 10 with an SQLite database.
*   **[student-frontend](file:///c:/Sowmya/Practice/Project/student-dotnet/student-frontend/)**: The React frontend built using Vite, Axios, and React Router.

---

## Prerequisites

Before starting, ensure you have the following installed on your machine:
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Node.js](https://nodejs.org/) (version 18+ recommended)
*   A terminal (PowerShell, Command Prompt, or VS Code integrated terminal)
*   *(Optional)* An SQLite Database client (e.g. [DB Browser for SQLite](https://sqlitebrowser.org/) or VS Code SQLite Viewer extension)

---

## How to Run the Code

Follow these steps to run both the backend API and the frontend application.

### 1. Run the Backend API

1.  Open your terminal and navigate to the `StudentApi` directory:
    ```bash
    cd StudentApi
    ```
2.  Restore the NuGet packages:
    ```bash
    dotnet restore
    ```
3.  **Create & Apply Migrations**: 
    If the database is not initialized, run the EF Core migrations to create the database file `students.db`:
    ```bash
    # Install the dotnet-ef tool globally if you don't have it:
    dotnet tool install --global dotnet-ef
    
    # Create the migration (if not already created):
    dotnet ef migrations add InitialCreate
    
    # Apply migrations to create the SQLite database:
    dotnet ef database update
    ```
4.  Run the backend server:
    ```bash
    dotnet run
    ```
    *   The API starts at:
        *   **HTTP**: `http://localhost:5112`
        *   **HTTPS**: `https://localhost:7015`
    *   Interactive Swagger documentation: `http://localhost:5112/swagger`

---

### 2. Run the Frontend React Application

1.  Open a new terminal window and navigate to the `student-frontend` directory:
    ```bash
    cd student-frontend
    ```
2.  Install the npm dependencies:
    ```bash
    npm install
    ```
3.  Start the development server:
    ```bash
    npm run dev
    ```
    *   The frontend starts at: `http://localhost:5173`
    *   Vite is configured with a proxy, meaning any calls to `/auth` and `/student` will be automatically forwarded to the backend API running at `http://localhost:5112`.

---

## How to Generate the Authorization Token

The Student API is protected. Every request to the `/student` endpoints (except viewing endpoints if anonymous access was allowed, though currently all `/student` endpoints require authorization) must include a valid JSON Web Token (JWT) in the headers:
```http
Authorization: Bearer <your-jwt-token>
```

Here are three ways to generate and use your authorization token.

### Method 1: Using the Frontend Interface (Easiest)

1.  Start both frontend and backend projects.
2.  Go to `http://localhost:5173` in your browser.
3.  Click **Register** to create a new user. Enter a username, email, and password.
4.  After registering, go to **Login** and enter your registered email and password.
5.  On successful login, the frontend will automatically:
    *   Retrieve the JWT token from the API response.
    *   Save it in your browser's `localStorage` (as `token`).
    *   Attach the token to the `Authorization` header of all subsequent API requests.

---

### Method 2: Using Swagger UI (Interactive Testing)

1.  Start the backend API and navigate to `http://localhost:5112/swagger` in your browser.
2.  **Register a User**:
    *   Expand `POST /auth/register`.
    *   Click **Try it out**.
    *   Enter user details in the request body, for example:
        ```json
        {
          "username": "tester",
          "email": "tester@example.com",
          "password": "Password123!"
        }
        ```
    *   Click **Execute**.
3.  **Log in to get the Token**:
    *   Expand `POST /auth/login`.
    *   Click **Try it out**.
    *   Enter the credentials you just registered:
        ```json
        {
          "email": "tester@example.com",
          "password": "Password123!"
        }
        ```
    *   Click **Execute**.
    *   In the **Response body**, copy the value of the `"token"` field (exclude the surrounding quotation marks):
        ```json
        {
          "token": "eyJhbGciOiJIUzI1NiIsIn...",
          "email": "tester@example.com",
          "username": "tester",
          "role": "User"
        }
        ```
4.  **Authorize Swagger**:
    *   Scroll to the top of the Swagger page and click the green **Authorize** button.
    *   Paste the copied token into the text box (without any `"Bearer"` prefix, just the raw token string).
    *   Click **Authorize** and then **Close**.
    *   You can now test all protected endpoints (such as `GET /student`) directly in Swagger!

---

### Method 3: Using cURL or Postman

You can send direct HTTP POST requests to generate a token using cURL:

1.  **Register a User**:
    ```bash
    curl -X POST "http://localhost:5112/auth/register" \
      -H "Content-Type: application/json" \
      -d "{\"username\": \"testuser\", \"email\": \"user@example.com\", \"password\": \"Password123!\"}"
    ```
2.  **Login to Retrieve Token**:
    ```bash
    curl -X POST "http://localhost:5112/auth/login" \
      -H "Content-Type: application/json" \
      -d "{\"email\": \"user@example.com\", \"password\": \"Password123!\"}"
    ```
3.  **Use Token in Protected Requests**:
    Copy the returned `token` and pass it in subsequent headers:
    ```bash
    curl -X GET "http://localhost:5112/student" \
      -H "Authorization: Bearer <your-jwt-token>"
    ```

---

## Role-Based Access Control (RBAC)

The system supports two user roles: `User` and `Admin`.

| Endpoint | Method | Role Required | Description |
| :--- | :--- | :--- | :--- |
| `/auth/register` | POST | None | Create a user account |
| `/auth/login` | POST | None | Authenticate and retrieve JWT token |
| `/auth/forgot-password` | POST | None | Reset password for an existing user account |
| `/student` | GET | `User` or `Admin` | View all students |
| `/student/{id}`| GET | `User` or `Admin` | View a single student |
| `/student` | POST | `Admin` | Add a new student |
| `/student` | PUT | `Admin` | Update an existing student |
| `/student` | DELETE | `Admin` | Remove a student |

### How to Assign the `Admin` Role for Testing

All users created through the `/auth/register` endpoint get the `User` role by default. If you try to add, edit, or delete students in Swagger or the Frontend UI with a default account, you will receive a `403 Forbidden` response. 

To grant a user the `Admin` role:

1.  Open the SQLite database `students.db` (located in the `StudentApi` directory after running database update) using any SQLite client or editor.
2.  Run the following SQL update command:
    ```sql
    UPDATE Users SET Role = 'Admin' WHERE Email = 'your-email@example.com';
    ```
3.  **Crucial Step**: Log in again. The backend encodes the role in the JWT token at login. A new login generates a new JWT token containing the `Admin` claim.
4.  Use this new token to perform administrative operations.

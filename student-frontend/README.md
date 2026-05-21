# Student Management System - Frontend Client

This is the React frontend application built using **React 19**, **Vite**, **Axios**, and **React Router**. It provides a user interface to register, log in, view student lists, and perform CRUD operations if authorized as an admin.

## Quick Start

1.  **Install dependencies**:
    ```bash
    npm install
    ```
2.  **Start the Vite development server**:
    ```bash
    npm run dev
    ```
    The application will run at `http://localhost:5173`.

## Main Configuration

-   **API Proxying**: Vite is configured in [vite.config.js](file:///c:/Sowmya/Practice/Project/student-dotnet/student-frontend/vite.config.js) to forward `/auth` and `/student` endpoints to the backend API running at `http://localhost:5112`.
-   **Authentication State**: Handled via React Context in [AuthContext.jsx](file:///c:/Sowmya/Practice/Project/student-dotnet/student-frontend/src/context/AuthContext.jsx) which stores the logged-in user details and JWT token in `localStorage`.
-   **Axios Interceptor**: The config in [axios.js](file:///c:/Sowmya/Practice/Project/student-dotnet/student-frontend/src/api/axios.js) automatically injects the stored JWT token as a Bearer authorization token on outgoing requests to `/student`.

For full setup instructions, including backend setup and database migrations, please refer to the main repository [README.md](file:///c:/Sowmya/Practice/Project/student-dotnet/README.md).

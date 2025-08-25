# Live Stock and Chat Application

This project is a real-time Live Stock and Chat application built with ASP.NET Core for the backend and Angular for the frontend, utilizing SignalR for real-time communication.

## Technologies Used

### Backend (ASP.NET Core)
- **ASP.NET Core 9.0:** Web API framework.
- **SignalR:** Real-time communication library.
- **ASP.NET Core Identity:** User authentication and management.
- **Entity Framework Core:** ORM for database interaction.
- **SQL Server Express:** Database.

### Frontend (Angular)
- **Angular 17:** Frontend framework.
- **SignalR Client:** For real-time communication with the backend.
- **AdminLTE:** Admin dashboard template for UI styling.
- **Bootstrap:** CSS framework.
- **Font Awesome:** Icon library.

## How to Run the Project

### Prerequisites
- .NET SDK 9.0 or later
- Node.js and npm (or Yarn)
- Angular CLI (install globally: `npm install -g @angular/cli`)
- SQL Server Express (or any compatible SQL Server instance)

### Backend Setup
1.  **Navigate to the Backend Directory:**
    ```bash
    cd StockMarket.Api
    ```
2.  **Update Database (if not already done):**
    The application is configured to use SQL Server Express. Ensure your SQL Server Express instance is running.
    The connection string is located in `StockMarket.Api/appsettings.json`.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=.\\SQLEXPRESS;Database=StockMarketDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```
    Apply the database migrations:
    ```bash
    dotnet ef database update --project StockMarket.Api
    ```
    This will create the `StockMarketDb` database and apply the necessary tables for ASP.NET Core Identity.

3.  **Run the Backend Application:**
    ```bash
    dotnet run
    ```
    The API will typically run on `https://localhost:7108`. Check your console output for the exact URL.

### Frontend Setup
1.  **Navigate to the Frontend Directory:**
    ```bash
    cd StockMarket.Client
    ```
2.  **Install Dependencies:**
    ```bash
    npm install
    ```
3.  **Run the Frontend Application:**
    ```bash
    ng serve
    ```
    The Angular application will typically run on `http://localhost:4200`. Open your browser to this URL.

## Test User Credentials

Upon first run of the backend application, the following test users will be seeded into the database:

-   **User 1:**
    -   Email: `user1@example.com`
    -   Password: `Password123!`
-   **User 2:**
    -   Email: `user2@example.com`
    -   Password: `Password123!`
-   **User 3:**
    -   Email: `user3@example.com`
    -   Password: `Password123!`
-   **User 4:**
    -   Email: `user4@example.com`
    -   Password: `Password123!`

Feel free to register new users through the application's registration page as well.

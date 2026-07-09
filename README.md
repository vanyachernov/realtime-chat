# RealtimeChat

A real-time messaging application demonstrating a clean, modern .NET architecture using SignalR, WPF, and CQRS/MediatR patterns.

## 📸 UI Screenshot

> **[TODO: Insert UI Screenshot Here]**
> *(Capture a screenshot of the WPF client showing the dark-themed message list and the connection status bar, and place the image link here, e.g., `![Chat UI](docs/ui-screenshot.png)`)*

## 🏗 Architecture

The solution follows **Clean Architecture** principles to ensure separation of concerns and testability:

- **Domain (`RealtimeChat.Domain`)**: Contains the core business entities (`User`, `Message`, `ChatRoom`) and repository interfaces. It has no external dependencies.
- **Application (`RealtimeChat.Application`)**: Contains the business use cases. We use the **CQRS (Command Query Responsibility Segregation)** pattern implemented via **MediatR**.
  - **Why CQRS/MediatR?** It decouples the API/SignalR endpoints from the business logic. Each feature is encapsulated in its own Command/Query handler, making the codebase highly cohesive, easy to test, and naturally extensible. It also allows us to easily add cross-cutting concerns (like logging and validation) using MediatR Pipeline Behaviors.
- **Infrastructure (`RealtimeChat.Infrastructure`)**: Implements the repository interfaces using EF Core and MySQL, mapping the domain models to the database schema (using `snake_case` naming conventions).
- **WebApi (`RealtimeChat.WebApi`)**: The entry point for the backend. It provides REST endpoints for initial data fetching (e.g., message history) and a SignalR Hub (`ChatHub`) for real-time bi-directional communication.
- **Client (`RealtimeChat.Client.Wpf`)**: A WPF desktop client using the MVVM pattern. It connects to the WebApi via HTTP (for history) and SignalR (for real-time events) and features a resilient, auto-reconnecting communication layer.

## 🛠 Technologies

- **.NET 10**
- **ASP.NET Core** (Web API, SignalR)
- **WPF** (Windows Presentation Foundation) with MVVM
- **Entity Framework Core 9** (MySQL with `Pomelo.EntityFrameworkCore.MySql`)
- **MediatR** (CQRS, Pipeline Behaviors)
- **FluentValidation** (Request validation)
- **Serilog** (File & Console logging)
- **Docker** (MySQL containerization)

## 🚀 How to Run

### 1. Start the Database
Ensure you have Docker installed and running. Start the MySQL database:
```bash
docker-compose up -d
```

### 2. Apply Migrations
Apply the EF Core migrations to create the database schema:
```bash
dotnet ef database update --project src/RealtimeChat.Infrastructure --startup-project src/RealtimeChat.WebApi
```

### 3. Run the Backend (WebApi)
Start the WebApi server. It will listen on `http://localhost:5050` (and `https://localhost:5051`).
```bash
cd src/RealtimeChat.WebApi
dotnet run
```

### 4. Run the Client (WPF)
In a separate terminal (or from your IDE), start the WPF client. You can run multiple instances of the client to test real-time messaging!
```bash
cd src/RealtimeChat.Client.Wpf
dotnet run
```

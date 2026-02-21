# FxRateHub

<div align="center">

![FxRateHub Logo](https://img.shields.io/badge/FxRateHub-Foreign%20Exchange%20Rate%20API-blue)

**A comprehensive, open-source Foreign Exchange Rate API service**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-17+-DD0031?logo=angular)](https://angular.io/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![API Status](https://img.shields.io/badge/API-Active-brightgreen)]()

[Features](#features) • [Architecture](#architecture) • [Getting Started](#getting-started) • [API Documentation](#api-documentation) • [Contributing](#contributing)

</div>

---

## Overview

FxRateHub is a modern, scalable Foreign Exchange Rate API built with .NET 8.0 and Angular. It provides real-time currency exchange rates, currency conversion, and historical data through a RESTful API. The platform supports multiple authentication methods, rate limiting, and includes both public and authenticated endpoints.

## Features

### Core Features
- 🔄 **Real-time Exchange Rates** - Live currency exchange rates for 165+ currencies
- 💱 **Currency Conversion** - Convert amounts between any supported currencies
- 📊 **Historical Data** - Track exchange rate changes over time
- 🔐 **Multiple Authentication Methods** - JWT tokens and API Key authentication
- 📈 **Rate Limiting** - Configurable daily request limits per API key
- 🌐 **Public API** - Basic endpoints available without authentication

### Technical Features
- ⚡ **Clean Architecture** - Separation of concerns with Domain, Application, Infrastructure, and API layers
- 📝 **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- 🗄️ **Entity Framework Core** - ORM with SQL Server support
- 📋 **Swagger/OpenAPI** - Interactive API documentation
- 🔍 **Serilog Logging** - Structured logging with file and console sinks
- 🔄 **Background Jobs** - Automated exchange rate synchronization
- 🌍 **CORS Support** - Configured for Angular frontend integration

### User Features
- 👤 **User Registration & Login** - Email/password authentication
- 🔑 **Google OAuth** - Sign in with Google
- 🗝️ **API Key Management** - Generate, regenerate, and monitor API keys
- 👨‍💼 **Admin Dashboard** - User management, statistics, and settings

---

## Architecture

FxRateHub follows Clean Architecture principles with distinct layers:

```
┌─────────────────────────────────────────────────────────────┐
│                      FxRateHub.API                          │
│                   (Presentation Layer)                       │
│  Controllers, Middleware, Swagger, Authentication           │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  FxRateHub.Application                      │
│                   (Application Layer)                        │
│  CQRS (Commands/Queries), DTOs, Interfaces, Behaviors       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 FxRateHub.Infrastructure                    │
│                   (Infrastructure Layer)                     │
│  DbContext, Repositories, External Services, Migrations     │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    FxRateHub.Domain                         │
│                     (Domain Layer)                           │
│  Entities, Enums, Value Objects, Domain Exceptions          │
└─────────────────────────────────────────────────────────────┘
```

### Project Structure

```
FxRateHub/
├── src/
│   ├── FxRateHub.API/                 # Web API project
│   │   ├── Controllers/               # API controllers
│   │   │   ├── AdminController.cs     # Admin endpoints
│   │   │   ├── ApiKeyController.cs    # API key management
│   │   │   ├── AuthController.cs      # Authentication
│   │   │   ├── PublicController.cs    # Public endpoints
│   │   │   ├── UserController.cs      # User profile
│   │   │   └── V1/
│   │   │       └── ExchangeRatesController.cs  # V1 API
│   │   ├── Middleware/                # Custom middleware
│   │   │   ├── ApiKeyAuthMiddleware.cs
│   │   │   └── ExceptionHandlingMiddleware.cs
│   │   └── Program.cs                 # Application entry point
│   │
│   ├── FxRateHub.Application/         # Application layer
│   │   ├── Common/                    # Shared components
│   │   │   ├── Interfaces/            # Service interfaces
│   │   │   ├── Models/                # Shared models
│   │   │   └── Exceptions/            # Custom exceptions
│   │   └── Features/                  # Feature folders (CQRS)
│   │       ├── Admin/
│   │       ├── ApiKeys/
│   │       ├── Auth/
│   │       ├── ExchangeRates/
│   │       └── User/
│   │
│   ├── FxRateHub.Domain/              # Domain layer
│   │   ├── Entities/                  # Domain entities
│   │   │   ├── User.cs
│   │   │   ├── ApiKey.cs
│   │   │   ├── ExchangeRate.cs
│   │   │   └── ...
│   │   ├── Enums/                     # Enumerations
│   │   └── Common/                    # Base classes
│   │
│   └── FxRateHub.Infrastructure/      # Infrastructure layer
│       ├── Persistence/               # Database context
│       ├── Migrations/                # EF Core migrations
│       ├── ExternalServices/          # External API clients
│       ├── BackgroundJobs/            # Background services
│       └── Configuration/             # Options classes
│
├── fx-rate-hub-ui/                    # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/
│   │   │   ├── features/
│   │   │   ├── layout/
│   │   │   └── shared/
│   │   └── environments/
│   └── package.json
│
├── API_DOCUMENTATION.md               # Complete API documentation
├── FxRateHub.sln                      # Solution file
└── README.md                          # This file
```

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (or SQL Server Express)
- [Node.js 18+](https://nodejs.org/) (for Angular frontend)
- [Angular CLI](https://angular.io/cli) (optional)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/FxRateHub.git
   cd FxRateHub
   ```

2. **Configure the database connection**
   
   Update `src/FxRateHub.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=FxRateHub;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
     }
   }
   ```

3. **Configure JWT settings**
   ```json
   {
     "Jwt": {
       "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
       "Issuer": "FxRateHub",
       "Audience": "FxRateHubUsers",
       "ExpirationDays": 7
     }
   }
   ```

4. **Configure external services**
   ```json
   {
     "Google": {
       "ClientId": "your-google-client-id.apps.googleusercontent.com"
     },
     "CurrencyFreaks": {
       "ApiKey": "your-currencyfreaks-api-key"
     }
   }
   ```

5. **Run database migrations**
   ```bash
   cd src/FxRateHub.API
   dotnet ef database update
   ```

6. **Run the API**
   ```bash
   dotnet run
   ```

   The API will be available at:
   - HTTP: `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/api-docs`

### Frontend Setup

1. **Navigate to the Angular project**
   ```bash
   cd fx-rate-hub-ui
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure environment**
   
   Update `src/environments/environment.ts`:
   ```typescript
   export const environment = {
     production: false,
     apiUrl: 'http://localhost:5000/api'
   };
   ```

4. **Run the development server**
   ```bash
   ng serve
   ```

   The frontend will be available at `http://localhost:4200`

---

## API Documentation

### Quick Reference

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/public/rates` | GET | None | Get all exchange rates |
| `/api/public/convert` | GET | None | Convert currency |
| `/api/public/currencies` | GET | None | Get available currencies |
| `/api/auth/register` | POST | None | Register new user |
| `/api/auth/login` | POST | None | Login user |
| `/api/auth/google` | POST | None | Google OAuth login |
| `/api/user/profile` | GET | JWT | Get user profile |
| `/api/user/dashboard` | GET | JWT | Get dashboard data |
| `/api/apikey/generate` | POST | JWT | Generate API key |
| `/api/v1/rates` | GET | API Key | Get exchange rates (V1) |
| `/api/v1/convert` | GET | API Key | Convert currency (V1) |
| `/api/admin/users` | GET | JWT (Admin) | Get all users |
| `/api/admin/statistics` | GET | JWT (Admin) | Get dashboard stats |

### Authentication

**JWT Token (User Endpoints):**
```http
Authorization: Bearer <your_jwt_token>
```

**API Key (V1 Endpoints):**
```http
X-API-Key: <your_api_key>
```

### Example Requests

**Get Exchange Rates (Public):**
```bash
curl -X GET "https://api.fxratehub.com/api/public/rates?baseCurrency=USD"
```

**Convert Currency (with API Key):**
```bash
curl -X GET "https://api.fxratehub.com/api/v1/convert?from=USD&to=EUR&amount=100" \
  -H "X-API-Key: your_api_key"
```

**Login:**
```bash
curl -X POST "https://api.fxratehub.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com", "password": "password123"}'
```

For complete API documentation, see [API_DOCUMENTATION.md](API_DOCUMENTATION.md).

---

## Configuration

### App Settings

| Setting | Description | Default |
|---------|-------------|---------|
| `Jwt:SecretKey` | JWT signing key | Required |
| `Jwt:ExpirationDays` | Token expiration | 7 days |
| `RateLimiting:DailyLimit` | API requests per day | 1000 |
| `Cors:AllowedOrigins` | CORS origins | `["http://localhost:4200"]` |

### Environment Variables

For production, use environment variables:

```bash
# Database
ConnectionStrings__DefaultConnection="Server=prod-server;Database=FxRateHub;..."

# JWT
Jwt__SecretKey="your-production-secret-key"
Jwt__Issuer="FxRateHub"
Jwt__Audience="FxRateHubUsers"

# External Services
Google__ClientId="your-google-client-id"
CurrencyFreaks__ApiKey="your-api-key"
```

---

## Database Schema

### Core Tables

- **Users** - User accounts with authentication info
- **ApiKeys** - API keys for programmatic access
- **ExchangeRates** - Current exchange rates
- **ExchangeRateHistory** - Historical rate snapshots
- **ApiUsageLogs** - API request tracking
- **SyncLogs** - Exchange rate sync history
- **AppSettings** - Application configuration

### Entity Relationships

```
User (1) ─────── (1) ApiKey
  │                    │
  │                    │
  │                    └─── ApiUsageLog (∞)
  │
  └─── UserRole (enum)
       └─── AuthProvider (enum)

ExchangeRate (1) ─────── (∞) ExchangeRateHistory
```

---

## Deployment

### Docker Support

```dockerfile
# Dockerfile example
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "src/FxRateHub.API/FxRateHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FxRateHub.API.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=FxRateHub;...
    depends_on:
      - db
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrongPassword123!
    ports:
      - "1433:1433"
```

### Azure Deployment

1. Create an Azure App Service
2. Configure connection strings in App Settings
3. Deploy using Visual Studio or Azure CLI

```bash
az webapp deployment source config-zip \
  --resource-group FxRateHubRG \
  --name fxratehub-api \
  --src publish.zip
```

---

## Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Categories

- Unit Tests - Domain entities, application logic
- Integration Tests - API endpoints, database operations
- E2E Tests - Full workflow testing

---

## Contributing

We welcome contributions! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make your changes**
   - Follow coding conventions
   - Add tests for new features
   - Update documentation
4. **Commit your changes**
   ```bash
   git commit -m "Add: your feature description"
   ```
5. **Push to your branch**
   ```bash
   git push origin feature/your-feature-name
   ```
6. **Create a Pull Request**

### Coding Standards

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Write XML documentation for public APIs
- Keep methods small and focused
- Write unit tests for business logic

---

## Roadmap

- [ ] WebSocket support for real-time rates
- [ ] Historical rate charts
- [ ] Rate alerts and notifications
- [ ] More OAuth providers (GitHub, Microsoft)
- [ ] GraphQL API
- [ ] Mobile app SDKs
- [ ] Rate forecasting with ML

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- [CurrencyFreaks API](https://currencyfreaks.com/) for exchange rate data
- [.NET Community](https://dotnet.microsoft.com/) for the amazing framework
- [Angular Team](https://angular.io/) for the frontend framework

---

## Support

- 📧 Email: support@fxratehub.com
- 🐛 Issues: [GitHub Issues](https://github.com/yourusername/FxRateHub/issues)
- 📖 Documentation: [API Documentation](API_DOCUMENTATION.md)

---

<div align="center">

**Made with ❤️ by the FxRateHub Team**

[⬆ Back to Top](#fxratehub)

</div>

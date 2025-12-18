# KontoApi

A personal finance management API built with .NET 9

## Overview

KontoApi is a RESTful API for managing personal finances, including:

- User authentication with JWT tokens
- Budget management
- Transaction tracking (income, expenses, transfers)
- Bank statement import

## Tech Stack

- **.NET 9** - Runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **xUnit + Moq** - Testing

## Project Structure

```bash
src/
├── Api/            # Controllers, middleware, validators
├── Application/    # Use cases, DTOs, interfaces
├── Domain/         # Entities, value objects
└── Infrastructure/ # Repositories, external services
tests/
└── KontoApi.Tests/ # Unit tests
```

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Node.js](https://nodejs.org/) (for commit linting)

### Setup

1. Clone the repository
2. Configure the database connection in `src/Infrastructure/KontoDbContext.cs`
3. Set up JWT secret:

   ```bash
   cd src/Api
   dotnet user-secrets set "Jwt:Key" "your-secret-key-min-32-chars"
   ```

4. Install commit hooks:

   ```bash
   npm install
   ```

### Run

```bash
dotnet run --project src/Api
```

The API will be available at `http://localhost:5076` (or `https://localhost:7049`) with Swagger UI at `/swagger`.

### Test

```bash
dotnet test
```

## API Endpoints

See interactive documentation and try out requests in [Swagger UI](http://localhost:5001/swagger)

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines

## License

Apache 2.0, see [LICENSE](LICENSE) for details

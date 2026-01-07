# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

GameScrubsV2 is a tournament bracket management system with three main components:

1. **Backend API** (`GameScrubsV2/`) - ASP.NET Core minimal API with SignalR
2. **Frontend Web App** (`GameScrubsV2.Web/`) - Vue.js 3 SPA with Tailwind CSS
3. **Integration Tests** (`GameScrubsV2.IntegrationTests/`) - xUnit integration test suite

## Common Commands

### Backend (.NET)
```bash
# Build solution
dotnet build GameScrubsV2.sln

# Run API server  
dotnet run --project GameScrubsV2

# Run integration tests
cd GameScrubsV2.IntegrationTests
dotnet test

# Run single test
dotnet test --filter "TestMethodName"

# Format code
Scripts/Format.cmd
# OR
dotnet format GameScrubsV2.sln --severity info

# Database migrations
Scripts/AddNewMigration.cmd "MigrationName"
Scripts/UpdateDatabase.cmd
Scripts/RevertToMigration.cmd "MigrationName"
```

### Frontend (Vue.js)
```bash
cd GameScrubsV2.Web

# Install dependencies
npm install

# Development server
npm run dev

# Build for production
npm run build

# Type checking
npm run type-check

# Lint and format
npm run lint
npm run format
```

## Architecture Overview

### Backend Design Pattern
The API uses **minimal API endpoints** with a clean architecture:

- **Endpoints**: Feature-based organization (`Endpoints/{Feature}/{Action}.cs`)
- **Models**: EF Core entities with ASP.NET Identity integration
- **Repositories**: Data access layer with custom repository pattern
- **Services**: Business logic and external service integration
- **Hubs**: SignalR real-time communication

### Endpoint Registration
All endpoints are registered in `EndpointRegistration.cs` using extension methods. Each endpoint is a static class with extension methods on route groups.

### Authentication & Authorization
- JWT Bearer token authentication
- ASP.NET Identity for user management
- Rate limiting configured per endpoint group

### Database
- Entity Framework Core with SQL Server
- Code-first migrations in `Migrations/` directory
- Custom migration scripts in `Scripts/` directory

### JSON Serialization
The application uses custom JSON serialization options defined in `Common/Json/SerializerOptions.cs`:
- Includes `JsonStringEnumConverter` for enum handling
- Custom `GuidConverter` for GUID serialization
- Web defaults for property naming

**Important**: All test files must use `DefaultJsonSerializerOptions` instead of basic `JsonSerializerOptions` to ensure enum serialization works correctly.

### Configuration Management
Configuration classes in `Configurations/` directory:
- `TokenSettings` - JWT configuration
- `RateLimitingSettings` - Rate limiting toggle

### Real-time Communication
SignalR hub (`BracketHub.cs`) provides real-time updates for:
- Bracket score updates
- Tournament status changes
- Player additions/removals

## Integration Testing

### Test Structure
- Base class: `IntegrationTestBase` provides common test utilities
- Test factory: `IntegrationTestFactory` configures test environment with:
  - In-memory SQL Server database using Testcontainers
  - Disabled rate limiting (`RateLimiting.Enabled = false`)
  - Custom JSON serialization options

### Critical Testing Requirements
1. **JSON Serialization**: All test files must include:
   ```csharp
   using static GameScrubsV2.Common.Json.SerializerOptions;
   ```
   And use `DefaultJsonSerializerOptions` for `JsonSerializer.Deserialize` calls.

2. **Rate Limiting**: Tests run with rate limiting disabled via `appsettings.Testing.json`

3. **Test Data**: Use `IntegrationTestBase.CreateUserAndGetToken()` for authentication setup

### Common Test Patterns
- Create test data using HTTP calls to the API
- Use FluentAssertions for assertions
- Each test class inherits from `IntegrationTestBase`
- Tests are isolated with fresh database containers

## Frontend Architecture

### Technology Stack
- Vue.js 3 with Composition API
- TypeScript
- Tailwind CSS 4.0
- Pinia for state management
- Vue Router for routing
- SignalR for real-time updates

### Project Structure
- `components/` - Reusable Vue components
- `views/` - Page-level components
- `services/` - API communication and SignalR
- `stores/` - Pinia state management
- `models/` - TypeScript interfaces and enums
- `router/` - Vue Router configuration

### Key Services
- `bracketService.ts` - Tournament bracket API calls
- `signalrService.ts` - Real-time communication
- `apiErrorHandler.ts` - Centralized error handling

## Development Notes

### Rate Limiting
- Production: Enabled via `appsettings.json` (`RateLimiting.Enabled = true`)
- Testing: Disabled via `appsettings.Testing.json` (`RateLimiting.Enabled = false`)
- Configuration managed in `Program.cs` with conditional setup

### Logging
- Serilog configured for structured logging
- Console and file outputs
- Log files in `Logs/` directory with daily rolling

### Database Connection
- Development: Uses `DefaultConnection` from appsettings
- Production: Uses connection string from configuration
- Integration tests: Uses Testcontainers SQL Server

### Docker Support
- Dockerfile present for containerized deployment
- Docker Compose configuration in `compose.yaml`

### Code Style
- EditorConfig settings enforced
- Automatic formatting via `dotnet format`
- ESLint and Prettier for frontend code
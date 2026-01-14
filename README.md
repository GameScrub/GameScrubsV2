# GameScrubsV2 🏆

A modern tournament bracket management system built with ASP.NET Core and Vue.js, designed to streamline competitive gaming tournaments with real-time updates and comprehensive bracket management.

## 🚀 Features

### Tournament Management
- **Multi-format Brackets**: Support for single elimination, double elimination, and round-robin tournaments
- **Real-time Updates**: Live bracket updates via SignalR for seamless tournament experience
- **Player Management**: Add, remove, and reorder players with drag-and-drop functionality
- **Score Tracking**: Comprehensive scoring system with placement management
- **Tournament Status Control**: Manage bracket lifecycle from setup to completion

### Technical Features
- **Modern Architecture**: Clean minimal API design with feature-based organization
- **Authentication & Security**: JWT-based authentication with ASP.NET Identity
- **Rate Limiting**: Configurable request throttling for API protection
- **PWA Support**: Progressive web app capabilities for mobile-first experience
- **Comprehensive Testing**: Full integration test suite with Testcontainers

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core (.NET 10.0) with Minimal APIs
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: JWT Bearer tokens with ASP.NET Identity
- **Real-time**: SignalR for live bracket updates
- **Logging**: Serilog with structured logging
- **Testing**: xUnit with Testcontainers for integration tests

### Frontend
- **Framework**: Vue.js 3 with Composition API and TypeScript
- **Styling**: Tailwind CSS 4.0 with responsive design
- **State Management**: Pinia for reactive state handling
- **Build Tools**: Vite with hot module replacement
- **Icons**: Tabler Icons for consistent UI elements

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (v20.19.0+ or v22.12.0+)
- [SQL Server](https://www.microsoft.com/sql-server) (for production) or Docker (for development)

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/GameScrub/GameScrubsV2.git
cd GameScrubsV2
```

### 2. Backend Setup
```bash
# Navigate to backend directory
cd GameScrubsV2

# Restore dependencies
dotnet restore

# Update database
Scripts/UpdateDatabase.cmd
# OR manually: dotnet ef database update

# Run the API
dotnet run
```

The API will be available at `https://localhost:7000` with Swagger UI at `/swagger`.

### 3. Frontend Setup
```bash
# Navigate to frontend directory
cd GameScrubsV2.Web

# Install dependencies
npm install

# Start development server
npm run dev
```

The web application will be available at `http://localhost:5173`.

### 4. Using Docker (Alternative)
```bash
# Build and run with Docker Compose
docker-compose up --build
```

## 🔧 Development

### Backend Development
```bash
# Build solution
dotnet build GameScrubsV2.sln

# Run integration tests
cd GameScrubsV2.IntegrationTests
dotnet test

# Format code
Scripts/Format.cmd

# Add new migration
Scripts/AddNewMigration.cmd "MigrationName"
```

### Frontend Development
```bash
cd GameScrubsV2.Web

# Development server with hot reload
npm run dev

# Type checking
npm run type-check

# Lint and format
npm run lint
npm run format

# Build for production
npm run build
```

## 📖 API Documentation

### Authentication Endpoints
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/logout` - Logout user
- `GET /api/auth/user` - Get user details

### Bracket Management
- `POST /api/brackets/search` - Search brackets with pagination
- `GET /api/brackets/{id}` - Get bracket by ID
- `POST /api/brackets` - Create new bracket
- `PUT /api/brackets` - Update bracket
- `DELETE /api/brackets/{id}` - Delete bracket
- `POST /api/brackets/change-status` - Update bracket status
- `GET /api/brackets/positions/{type}` - Get bracket positions by type

### Player Management
- `POST /api/players` - Add player to bracket
- `GET /api/players/{bracketId}` - Get all players in bracket
- `PUT /api/players/reorder` - Reorder players
- `DELETE /api/players/{bracketId}/{playerId}` - Remove player

### Placement & Scoring
- `GET /api/brackets/{id}/placements` - Get bracket placements
- `POST /api/brackets/{id}/placements/score` - Set placement score
- `GET /api/brackets/{id}/placements/scores` - Get placement scores

### Reports
- `GET /api/reports/recent-activity` - Get recent tournament activity

## 🏗 Architecture

### Backend Architecture
```
GameScrubsV2/
├── Common/                 # Shared utilities and extensions
├── Configurations/         # Configuration classes
├── Endpoints/             # Feature-based API endpoints
│   ├── Auth/             # Authentication endpoints
│   ├── Bracket/          # Bracket management
│   ├── Player/           # Player management
│   ├── Placement/        # Scoring and placements
│   └── Report/           # Reporting endpoints
├── Enums/                # Application enums
├── Hubs/                 # SignalR hubs
├── Models/               # EF Core entities
├── Repositories/         # Data access layer
└── Services/             # Business logic services
```

### Frontend Architecture
```
GameScrubsV2.Web/src/
├── components/           # Reusable Vue components
├── composables/          # Composition API utilities
├── models/              # TypeScript interfaces
├── partials/            # Layout components
├── router/              # Vue Router configuration
├── services/            # API communication
├── stores/              # Pinia state management
├── types/               # TypeScript type definitions
├── utils/               # Utility functions
└── views/               # Page components
```

## 🧪 Testing

### Running Tests
```bash
# Backend integration tests
cd GameScrubsV2.IntegrationTests
dotnet test

# Run specific test
dotnet test --filter "TestMethodName"

# Frontend tests (if available)
cd GameScrubsV2.Web
npm run test
```

### Test Architecture
- **Integration Tests**: Use Testcontainers for isolated SQL Server instances
- **Authentication**: Helper methods for JWT token generation
- **Database**: Each test run gets a fresh database container
- **Configuration**: Rate limiting disabled for testing

## 🚀 Deployment

### Docker Deployment
```bash
# Build production image
docker build -t gamescrubsv2 .

# Run with Docker Compose
docker-compose -f compose.yaml up -d
```

### Manual Deployment
1. Configure production database connection string
2. Set up SSL certificates
3. Configure CORS settings for production domain
4. Build frontend: `npm run build`
5. Publish backend: `dotnet publish -c Release`

## 📝 Configuration

### Backend Configuration
Key settings in `appsettings.json`:
- `ConnectionStrings:DefaultConnection` - Database connection
- `TokenSettings` - JWT configuration
- `CorsSettings:AllowedOrigins` - CORS configuration
- `RateLimiting:Enabled` - Rate limiting toggle

### Frontend Configuration
Environment variables in `.env`:
- `VITE_API_BASE_URL` - Backend API URL

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature-name`
3. Make changes and add tests
4. Run tests: `dotnet test` and `npm run lint`
5. Commit changes: `git commit -m "Description"`
6. Push to branch: `git push origin feature-name`
7. Create a Pull Request

### Code Style
- Follow existing patterns and conventions
- Use provided formatting scripts
- Ensure all tests pass
- Add integration tests for new endpoints

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [API Documentation](https://localhost:7000/swagger) (when running locally)
- [Vue.js Documentation](https://vuejs.org/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
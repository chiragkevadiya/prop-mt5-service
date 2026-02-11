# CLAUDE.md

## Project Overview

**Prop MT5 Connection Service** — A self-hosted Windows Service for MetaTrader 5 proprietary trading management. It provides a REST API for managing MT5 trading accounts, acting as an intermediary between client applications and MT5 broker servers — handling account management, liquidation monitoring, trading operations, and email notifications.

## Tech Stack

- **Language:** C# / .NET Framework 4.8
- **Web Framework:** ASP.NET Web API 5.2.9 with OWIN/Katana self-hosted HTTP (port 8086)
- **Service Host:** Topshelf 4.3.0 (Windows Service)
- **ORM:** Entity Framework Core 3.1.32
- **Database:** PostgreSQL (via Npgsql 4.1.14)
- **Email:** MailKit 4.14.0 / MimeKit 4.14.0
- **Logging:** Serilog 4.3.0
- **JSON:** Newtonsoft.Json 13.0.3
- **Auth:** System.IdentityModel.Tokens.Jwt / Microsoft.IdentityModel 5.5.0
- **MT5 SDK:** MetaQuotes.MT5ManagerAPI & MetaQuotes.MT5CommonAPI (binary DLLs in `Assets/`)

## Build & Run

```bash
# Build the solution
msbuild PropMT5ConnectionService.sln /p:Configuration=Release

# Run as console app (for development)
bin\Release\PropMT5ConnectionService.exe

# Install as Windows Service (via Topshelf)
PropMT5ConnectionService.exe install
PropMT5ConnectionService.exe start
```

NuGet packages are managed via `packages.config` (95 packages). Restore with `nuget restore` before building.

**Note:** No test framework is configured in this project.

## Architecture

```
HTTP Request --> OWIN Pipeline (port 8086)
  --> Custom Headers Middleware
  --> CustomMiddleware
  --> ASP.NET Web API (api/{controller}/{id})
  --> Controllers --> Services/Helpers --> MT5 Manager API / PostgreSQL DB
```

- **Entry point:** `Program.cs` -> `StartTopshelf()` -> `WebServer` class hosts the OWIN pipeline
- **Background jobs:** Liquidation check runs on a separate thread (60-min interval) in `WebServer.cs`
- **Two MT5 connections:** Live (`Mt5LiveClient`) and Demo (`Mt5DemoClient`)

## Directory Structure

| Directory | Purpose |
|-----------|---------|
| `Controllers/` | 39 API endpoint controllers (`Live*`, `Demo*`, `Account*`, etc.) |
| `Services/` | Business logic (`LiquidationService`, `EmailService`, `HttpClientService`) |
| `Helpers/` | Utility classes (~20 files), constants/enums in `Constant.cs` |
| `Models/` | EF Core entity models (`UserMaster`, `PropAccountMaster`, etc.) |
| `ViewModels/` | DTOs organized by feature (`ChallengeSettlement/`, `LeaderBoard/`, etc.) |
| `Data/` | `PropTradingDBContext.cs` -- EF Core DbContext |
| `Mt5Client/` | MT5 manager connection classes (`Mt5LiveClient`, `Mt5DemoClient`) |
| `Middleware/` | OWIN middleware (`CustomMiddleware.cs`) |
| `Utilities/` | Static utilities (`ConnectionString`, `DateFormatConverter`, `PasswordGenerator`) |
| `ApiResponse/` | API response wrapper models |
| `Assets/` | MT5 Manager API binary DLLs |
| `Extensions/` | Extension methods (`JsonExtensions`) |

## Naming Conventions

- **Namespace root:** `PropMT5ConnectionService` (consistent across all files)
- **Controllers:** `[Environment?][Feature]Controller.cs` -- e.g., `LiveAccountController`, `DemoAccountController`, `LeaderboardController`
  - Live environment: `Live*Controller` (e.g., `LiveTradingHistoryController`, `LivePasswordChangeController`)
  - Demo environment: `Demo*Controller` (e.g., `DemoAccountStatusController`, `DemoGroupUpdateController`)
  - Shared/general: No prefix (e.g., `LeaderboardController`, `LiquidationController`, `AccountTransferController`)
- **Entity models:** `*Master.cs` for core entities, `*History.cs` for tracking -- e.g., `UserMaster`, `UserChallengeHistory`
- **Base classes:** `BaseEntity`, `BaseGUID`, `BaseEntityCreatedModifiedDeleted`
- **ViewModels/DTOs:** `*VM.cs` with `Mt5` prefix for MT5-specific -- e.g., `Mt5TradingDataVM`, `Mt5LiveAccountVM`, `AccountPerformanceVM`
- **Services:** `*Service.cs` with `I*Service.cs` interfaces
- **Helpers:** `*Helper.cs`, `*Operations.cs`, or `*Factory.cs` -- e.g., `Mt5ManagerFactory`, `MT5AccountOperations`
- **Utilities:** Descriptive names -- `PasswordGenerator`, `DateFormatConverter`, `LeaderboardCalculator`
- **Enums:** PascalCase, defined in `Helpers/Constant.cs`

## Key Files

- `Program.cs` -- Service bootstrap, MT5 credential configuration, DI setup
- `WebServer.cs` -- OWIN host setup, background job runner
- `Startup.cs` -- OWIN middleware pipeline & Web API route configuration
- `Helpers/Constant.cs` -- All enums and constants (large file)
- `Data/PropTradingDBContext.cs` -- Database context and entity mappings
- `Services/LiquidationService.cs` -- Account liquidation monitoring logic
- `Services/EmailService.cs` -- Email template rendering and sending
- `Mt5Client/Mt5LiveClient.cs` -- Live MT5 manager connection
- `Mt5Client/Mt5DemoClient.cs` -- Demo MT5 manager connection

## External Dependencies

- **MT5 Manager API:** Requires DLLs initialized from `C:\dll_dot\MT5Libs` at runtime
- **PostgreSQL:** Connection string configured via `ConnectionString` static class
- **SMTP:** Email sending via MailKit (configured in EmailService)

## Known Issues

- Background liquidation job is currently disabled in code
- Some commented-out code blocks remain in `Program.cs` and `WebServer.cs`

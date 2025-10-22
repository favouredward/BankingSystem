BankingSystem API

The BankingSystem API is a secure, layered solution built on .NET 9, implementing a Clean Architecture (Domain, Application, Infrastructure, API). It features a robust, transaction-based banking core, secured by JWT authentication and managed by MediatR.

?? Setup & Installation

1. Prerequisites

.NET 9 SDK (or later)

SQL Server (LocalDB or Docker instance)

Redis (Required for Caching/Session, e.g., via Docker)

2. Configure Connection Strings

Update the appsettings.json file in the BankingSystem.Api project with your local configurations:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BankingSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "RedisConnection": "localhost:6379"
  },
  "JwtSettings": {
    "Secret": "[YOUR_LONG_SECRET_KEY_HERE_MIN_16_CHARS]", 
    "Issuer": "BankingSystemAPI",
    "Audience": "BankingSystemClient"
  }
}


3. Build and Database Migration

From the solution root directory (C:\Users\LENOVO\source\BankingSystem>):

Restore & Build:

dotnet restore
dotnet build


Apply Database Migrations (This creates the Identity, Accounts, and Transactions tables):

dotnet ef database update --project BankingSystem.Infrastructure --startup-project BankingSystem.Api


4. Run the API

dotnet run --project BankingSystem.Api


The API will typically start on http://localhost:5000 (HTTP) or https://localhost:5001 (HTTPS). Navigate to the Swagger UI at /swagger to test the endpoints.

?? API Endpoints & Usage

All banking endpoints require a Bearer Token obtained via the /api/Auth/login endpoint.

1. Authentication (Public Endpoints)

Endpoint

Method

DTO Input

Description

/api/Auth/register

POST

RegisterUserDto

Creates a new user identity and returns a JWT token immediately.

/api/Auth/login

POST

LoginDto

Authenticates the user and returns a valid JWT token.

Success Response (AuthResponseDto):

{
  "token": "eyJhbGciOi...",
  "userId": "guid-of-user",
  "email": "user@example.com"
}


2. Account Management (Secured Endpoints)

All endpoints below require the Authorization: Bearer <token> header.

Endpoint

Method

DTO Input

Description

/api/Account

POST

CreateAccountDto

Creates a new banking account linked to the authenticated user.

/api/Account/{accountId}

GET

(None)

Retrieves details and recent transaction history for the specified account.

/api/Account/deposit

POST

DepositDto

Deposits funds into the specified account number.

/api/Account/withdrawal

POST

WithdrawalDto

Withdraws funds from the specified account number, subject to balance check.

/api/Account/transfer

POST

TransferDto

Transfers funds between a source account (owned by user) and a destination account.

/api/Account/history/{accountId}

GET

(None)

Retrieves the full transaction history for the specified account.
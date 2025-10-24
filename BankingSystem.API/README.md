:

🏦 Banking System API

A Secure, Scalable .NET 8 Banking Backend with CQRS, JWT Authentication, and EF Core

🚀 Overview

The Banking System API simulates core banking operations — account creation, deposits, withdrawals, and transfers — using a clean, modular architecture built on .NET 8.

It implements:

CQRS Pattern using MediatR

JWT Authentication with ASP.NET Core Identity

EF Core + SQL Server for persistence

Serilog for structured logging

Redis for caching (registered for performance)

Mock External Payment Integration (easily extendable)

🧱 Project Architecture
BankingSystem.sln
│
├── BankingSystem.API           → Controllers, DI setup, Swagger, JWT, Serilog
├── BankingSystem.Application   → Commands, Queries, Handlers, Interfaces
├── BankingSystem.Domain        → Entities, DTOs, Enums, Business Rules
├── BankingSystem.Infrastructure→ EF Core DbContext, Repositories, Services
└── BankingSystem.Tests         → Unit tests (xUnit)

🧠 Core Technologies

.NET 8 / ASP.NET Core Web API

Entity Framework Core + SQL Server

ASP.NET Identity + JWT Authentication

Serilog (logging)

Redis (caching)

MediatR (CQRS + decoupling)

Swagger / OpenAPI (API documentation)

⚙️ Setup Instructions
1️⃣ Clone the Repository
git clone https://github.com/favouredward/BankingSystem.git
cd BankingSystem

2️⃣ Configure the Database

Update the connection string in appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=BankingSystemDB;Trusted_Connection=True;TrustServerCertificate=True"
}

3️⃣ Configure JWT Settings

In appsettings.json:

"JwtSettings": {
  "Secret": "ReplaceThisWithA256BitSecretKey",
  "Issuer": "BankingSystemAPI",
  "Audience": "BankingSystemClient",
  "ExpiryMinutes": 60
}

4️⃣ Run EF Core Migrations
cd BankingSystem.API
dotnet ef migrations add InitialCreate -p ../BankingSystem.Infrastructure -s .
dotnet ef database update -p ../BankingSystem.Infrastructure -s .

5️⃣ Run the Application
dotnet run --project BankingSystem.API


The API will start at:
👉 https://localhost:5001 or http://localhost:5000

🔐 Authentication

The API uses JWT Bearer tokens with ASP.NET Core Identity.

Register
POST /api/auth/register

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}


Login
POST /api/auth/login

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}


Response:

{
  "token": "JWT_TOKEN_HERE",
  "userId": "GUID",
  "email": "user@example.com"
}


Use the token in Swagger or Postman:
Authorization: Bearer <JWT_TOKEN>

💳 API Endpoints
Operation	Method	Route	Auth	Description
Create Account	POST	/api/accounts	✅	Creates new account for logged-in user
Deposit	POST	/api/accounts/deposit	✅	Deposit funds into your account
Withdraw	POST	/api/accounts/withdraw	✅	Withdraw funds securely
Transfer	POST	/api/accounts/transfer	✅	Transfer funds between accounts
Get Account Details	GET	/api/accounts/{accountId}	✅	Retrieve account info (ownership enforced)
Get Transaction History	GET	/api/accounts/{accountId}/transactions	✅	View recent transactions
Register	POST	/api/auth/register	❌	Register a new user
Login	POST	/api/auth/login	❌	Authenticate and receive JWT
🧠 Example Request: Deposit
Request
POST /api/accounts/deposit
Authorization: Bearer <token>
Content-Type: application/json

{
  "accountNumber": "1234567890",
  "amount": 200.50
}

Response
{
  "message": "Deposit successful."
}

🧰 Testing

To run unit tests (xUnit):

dotnet test


📌 Recommendation: Add test files for:

DepositCommandHandlerTests

WithdrawalCommandHandlerTests

TransferCommandHandlerTests

CreateAccountCommandHandlerTests

🧩 Design Patterns Used
Pattern	Purpose
CQRS (Command Query Responsibility Segregation)	Separates write/read operations
Repository Pattern	Decouples data access logic
Dependency Injection	Promotes modularity and testability
Mediator Pattern (MediatR)	Centralized request handling
Decorator (Logging/Validation)	Extendable middleware behaviors
📊 Logging and Monitoring

Serilog configured for console logging.

Middleware logs all requests and exceptions.

Extendable to use Seq, Azure Application Insights, or Elasticsearch.

⚡ Performance Enhancements

Redis ready for caching account lookups.

Async/await used throughout for scalability.

Transaction-safe EF operations ensure data integrity.

🛠️ Future Improvements

Add role-based admin panel (Admin, Customer)

Use Redis for caching transactions

Integrate real payment gateway (Paystack/Stripe)

Implement background jobs for monthly statement generation

👨‍💻 Author

Edward Favour
📧 [favouredward2511@gmail.com
]
🔗 GitHub Profile
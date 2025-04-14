# 💱 Currency Converter API

A robust, scalable, and secure ASP.NET Core Web API for currency conversion using the open-source [Frankfurter API](https://www.frankfurter.dev/). It supports real-time conversion, historical rates with pagination, and includes JWT authentication with role-based authorization.

---

## 🚀 Features

- 🔄 Currency Conversion (Real-Time)
- 📈 Historical Exchange Rates (with pagination)
- 🔐 JWT Bearer Authentication
- 🛡️ Role-Based Authorization
- 📦 Clean Architecture with SOLID principles
- 🧪 NUnit Unit Tests + Moq
- 📑 Swagger (OpenAPI) Integration
- ☁️ HTTP Client Integration for diffrent provider

---

## 🛠️ Tech Stack

- ASP.NET Core 8.0
- System.Text.Json
- HttpClientFactory
- NUnit & Moq
- Swagger / Swashbuckle
- Frankfurter API (https://www.frankfurter.dev/)
- JWT Authentication & Role Authorization

---
### 🚨 Frankfurter API

No API key needed. Public and free to use.

---

### 🚨 Assumption Made

- Frankfurter API is reliable and publicly accessible with no authentication needed.
- Currency data returned from the API fits our business need directly (no transformation needed).
- Roles are managed in-code for simplicity. (No external identity provider or DB for users yet)
- Pagination for historical data wraps all in a single page as Frankfurter does not support native pagination.
- Unit tests cover core business logic, not external API behavior (mocked).

---

## 📥 Setup Instructions

### 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022+ or VS Code
- Git

#### 🧾 1. Clone the Repository

```bash
git clone https://github.com/your-username/Bamboo-card-currency-converter.git
cd Bamboo-card-currency-converter
```

#### ⚙️ 2. Update Configuration
Modify appsettings.json inside the main project:

```json
"JwtSettings": {
  "Key": "your-very-secure-secret-key",
  "Issuer": "BambooCard",
  "Audience": "BambooUsers",
  "ExpiryMinutes": 60
}
```
✅ You can define role constants in a separate file (e.g., Roles.cs) and refer to them throughout your project.

#### ▶️ 3. Run the Project
```bash
cd Bamboo-card-currency-convertor
dotnet build
dotnet run
```
Navigate to:

```bash
https://localhost:{port}/swagger
```
Use Swagger UI to test APIs.

#### 🧪 4. Run Unit Tests
```bash
cd Bamboo-card-currency-convertor.UnitTests
dotnet test
```
## 🔐 Authentication & Authorization
- Auth is based on JWT Bearer Tokens
- Use /api/auth/login (or your implemented login route) to obtain a token
- Click 🔐 Authorize in Swagger and enter:

```php
Bearer <your-token>
```
Roles can be defined and managed separately and assigned dynamically or hardcoded for simplicity.

## 📄 Swagger UI
```bash
https://localhost:{port}/swagger
```
Supports Bearer token auth and testing secured routes. 


## 👤 Author

Kritesh R Sharma

[GitHub](https://github.com/kriteshsharma14/)

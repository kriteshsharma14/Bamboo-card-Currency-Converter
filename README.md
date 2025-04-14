# 💱 Bamboo Card Currency Converter API

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

## 📦 Setup Instructions

### 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022+ or VS Code
- (Optional) Postman for testing API

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

### 📁 Clone the Repo

```bash
git clone https://github.com/your-username/Bamboo-card-currency-converter.git
cd Bamboo-card-currency-converter



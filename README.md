# Freelance Marketplace System

A web-based platform that connects **Freelancers** with **Clients**.

Built with **ASP.NET Core MVC (.NET 10)** and **PostgreSQL**, with the database running in **Docker**.

---

## Tech Stack

- **Backend Framework:** ASP.NET Core 10.0 (MVC)
- **ORM:** Entity Framework Core 10 (Code First)
- **Database:** PostgreSQL 17
- **Infrastructure:** Docker Desktop / Docker Compose

---

## Project Architecture

- `Controllers/`: Handles HTTP requests and coordinates responses.
- `Services/`: Contains business logic and service interfaces.
- `Data/`: Contains database context and Entity Framework Core configuration.
- `Models/`: Contains application entities and domain models.
- `Migrations/`: Contains Entity Framework Core database migrations.

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Quanleeahii/freelance-marketplace-mvc.git
cd freelance-marketplace-mvc
```

### 2. Start the database

```bash
docker compose up -d
```

### 3. Apply database migrations

```bash
dotnet ef database update --project FreelanceMarketplace
```
### 4. Run the application

```bash
cd FreelanceMarketplace
dotnet run
```

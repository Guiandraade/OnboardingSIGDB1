# OnboardingSIGDB1

<div align="center">

![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core%208-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![Angular](https://img.shields.io/badge/Angular%2010-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=for-the-badge&logo=typescript&logoColor=white)

![Backend](https://img.shields.io/badge/Backend-Approved-brightgreen?style=for-the-badge)
![Frontend](https://img.shields.io/badge/Frontend-Approved-brightgreen?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-blue?style=for-the-badge)

</div>

---

## About the Project

This project is part of the technical onboarding process, focused on building a personnel management system (employees, companies, and positions) while demonstrating software architecture expertise using the .NET 8 ecosystem. The goal is to build a robust Web API following the standards required by the development team, paired with an Angular frontend.

---

## Tech Stack

### Backend

| Technology | Version | Purpose |
|---|---|---|
| .NET | 8 | Main framework |
| Entity Framework Core | 8 | ORM for data access |
| SQL Server | 2019+ | Relational database |
| AutoMapper | 13+ | Entity/DTO mapping |
| FluentValidation | 11+ | Input data validation |
| Swagger / Swashbuckle | 6.6+ | Interactive API documentation |

### Frontend

| Technology | Version | Purpose |
|---|---|---|
| Angular | 10.2.4 | SPA framework |
| Angular CLI | 10.2.4 | Build and scaffolding tool |
| TypeScript | 4.0.8 | Main language |
| RxJS | 6.6.7 | Reactive programming |
| Node.js | 14.15.x | JavaScript runtime |

---

## Solution Structure

```text
OnboardingSIGDB1/                         # Monorepo root
├── backend/                              # .NET 8 Web API
│   ├── src/
│   │   ├── OnboardingSIGDB1.API/         # Controllers, middleware, model binders
│   │   ├── OnboardingSIGDB1.Domain/      # Entities, interfaces, notifications, domain services
│   │   ├── OnboardingSIGDB1.Data/        # DbContext, Fluent API mappings, migrations, repositories
│   │   └── OnboardingSIGDB1.IOC/         # Dependency injection configuration
│   ├── tests/
│   │   └── OnboardingSIGDB1.UnitTests/   # Unit tests (100% mutation coverage via Stryker)
│   └── OnboardingSIGDB1.slnx             # Solution file
├── frontend/                             # Angular 10 SPA
│   ├── src/
│   │   └── app/
│   └── angular.json
├── README.md
└── .gitignore
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| **API** | Application entry point — controllers, middleware, model binders |
| **Domain** | Business core — entities, interfaces, notifications, domain services |
| **Data** | Persistence — DbContext, Fluent API mappings, migrations, repositories, Unit of Work |
| **IOC** | Dependency injection — decouples the API from concrete implementations |

---

## Development Roadmap

**Backend**
- [x] Phase 1 — Initial project setup
- [x] Phase 2 — DbContext and entity mappings
- [x] Phase 3 — Dependency injection (IOC layer)
- [x] Phase 4 — Generic repositories and Unit of Work
- [x] Phase 5 — Domain services and validations
- [x] Phase 6 — Controllers and API endpoints
- [x] Phase 7 — Unit tests with 100% mutation coverage (Stryker)

**Frontend**
- [x] Phase 8 — Angular project setup
- [x] Phase 9 — Screen implementation

---

## Getting Started

### Prerequisites

Make sure you have the following tools installed before proceeding:

| Tool | Version | Download / Install |
|---|---|---|
| .NET SDK | 8 | [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0) |
| SQL Server | 2019+ | [microsoft.com](https://www.microsoft.com/sql-server/sql-server-downloads) |
| Node.js | 14.15.x | [nodejs.org](https://nodejs.org/) |
| Angular CLI | 10.2.4 | `npm install -g @angular/cli@10.2.4` |
| Git | latest | [git-scm.com](https://git-scm.com/) |
| dotnet-ef | latest | `dotnet tool install --global dotnet-ef` |

---

### Backend Setup

**1. Clone the repository**

```bash
git clone https://github.com/Guiandraade/OnboardingSIGDB1.git
cd OnboardingSIGDB1
```

**2. Configure the connection string via User Secrets**

> Run from inside `backend/src/OnboardingSIGDB1.API`

```bash
cd backend/src/OnboardingSIGDB1.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OnboardingSIGDB1;Trusted_Connection=True;TrustServerCertificate=True;"
```

> **SQL Authentication** — if using username/password instead of Windows Auth:
> ```
> Server=localhost;Database=OnboardingSIGDB1;User Id=your_user;Password=your_password;TrustServerCertificate=True;
> ```

**3. Apply database migrations**

> Run from the `backend/` folder

```bash
cd ../../..
dotnet ef database update --project src/OnboardingSIGDB1.Data --startup-project src/OnboardingSIGDB1.API
```

**4. Run the API**

```bash
dotnet run --project src/OnboardingSIGDB1.API
```

**5. Access Swagger UI**

```
https://localhost:5001/swagger
```

---

### Frontend Setup

**1. Install dependencies**

> Run from the `frontend/` folder

```bash
cd frontend
npm install
```

**2. Run the development server**

```bash
ng serve
```

**3. Access the app**

```
http://localhost:4200
```

> The frontend communicates with the backend at `https://localhost:5001`. Make sure the API is running before starting the frontend.

---

## Architecture & Design Patterns

### Validation — FluentValidation
Validation rules are fully decoupled from domain entities, keeping the domain layer clean and reusable. Each input model has its own dedicated validator class.

### Persistence — Unit of Work
Centralized transaction management coordinating multiple repositories within a single atomic operation, ensuring data consistency.

### Persistence — Fluent API (EntityTypeConfiguration)
All entity-to-table mapping is done explicitly through dedicated configuration classes, avoiding data annotation pollution in the domain layer.

### Mapping — AutoMapper
Reduces boilerplate code when converting between domain entities and DTOs, keeping controllers and services focused on business logic.

### Error Handling — Notifications Pattern
Validation and business rule violations are accumulated through a notification context rather than thrown as exceptions, enabling multiple errors to be returned in a single response.

### Security — User Secrets
Sensitive configuration (e.g., connection strings) is stored using .NET User Secrets during development, keeping credentials out of source control.

---

## Contributing

1. Fork the repository
2. Create a feature branch:

```bash
git checkout -b feature/your-feature-name
```

3. Commit your changes following [Conventional Commits](https://www.conventionalcommits.org/):

```bash
git commit -m "feat(scope): short description of the change"
```

**Common commit types:**

| Type | When to use |
|---|---|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation changes only |
| `refactor` | Code restructuring without behavior change |
| `chore` | Build, tooling, or dependency updates |
| `test` | Adding or updating tests |

4. Push the branch and open a Pull Request:

```bash
git push origin feature/your-feature-name
```

---

## License

This project is licensed under the MIT License — you are free to use, modify, and distribute this code as long as you give proper credit. See the [LICENSE](LICENSE) file for details.


# OnboardingSIGDB1

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?style=for-the-badge&logo=typescript&logoColor=white)
![Status Backend](https://img.shields.io/badge/Backend-Aprovado-brightgreen?style=for-the-badge)
![Status Frontend](https://img.shields.io/badge/Frontend-Em%20Desenvolvimento-yellow?style=for-the-badge)

## 🚀 OnboardingSIGDB1

PT-BR: Este projeto faz parte do processo de Onboarding técnico, focado em demonstrar domínio de arquitetura de software utilizando o ecossistema .NET 8. O objetivo é construir uma Web API robusta seguindo os padrões exigidos pelo time de desenvolvimento.

EN-US: This project is part of the technical onboarding process, focused on demonstrating software architecture expertise using the .NET 8 ecosystem. The goal is to build a robust Web API following the standards required by the development team.

## 📋 Sobre o Projeto / About the Project

### Backend

| Tecnologias / Technologies | Versão / Version | Finalidade / Purpose |
|------------|--------|------------|
| .NET | 8 | Framework principal / Main framework |
| Entity Framework Core | 8 | ORM para acesso a dados / ORM for data access |
| SQL Server | 2019+ | Banco de dados relacional / Relational database |
| AutoMapper | 13+ | Mapeamento entre entidades e DTOs / Mapping between entities and DTOs |
| FluentValidation | 11+ | Validação de dados de entrada / Input data validation |
| Swagger | 6.6+ | Documentação interativa da API / Interactive API documentation |

### Frontend

| Tecnologias / Technologies | Versão / Version | Finalidade / Purpose |
|------------|--------|------------|
| Angular | 10.2.4 | Framework SPA / SPA Framework |
| Angular CLI | 10.2.4 | Ferramenta de build e scaffolding / Build and scaffolding tool |
| TypeScript | 4.0.8 | Linguagem principal / Main language |
| RxJS | 6.6.7 | Programacao reativa / Reactive programming |
| Node.js | 14.15.x | Runtime JavaScript / JavaScript runtime |

---


## 🏗️ Estrutura da Solução / Solution Structure

## **OnboardingSIGDB1.API**

**PT-BR**: Porta de entrada da aplicação. Contém Controllers, Middleware e DTOs.

**EN-US**: Application entry point. Contains Controllers, Middleware configurations, and DTOs.

## **OnboardingSIGDB1.Domain**

**PT-BR**: O coração do sistema. Contém Entidades, Interfaces, Notifications e Domain Services.

**EN-US**: The core of the system. Contains Business Entities, Interfaces, Notifications, and Domain Services.

## **OnboardingSIGDB1.Data**

**PT-BR**: Camada de persistência. Contém DbContext, Mappings (Fluent API), Migrations, Repositórios e Unit of Work.

**EN-US**: Persistence layer. Contains DbContext, Mappings, Migrations, and implementation of Repositories and Unit of Work.

## **OnboardingSIGDB1.IOC**

**PT-BR**: Centraliza toda a configuração de DI (Dependency Injection), desacoplando a API das implementações concretas.

**EN-US**: Centralizes all Dependency Injection configuration, decoupling the API from concrete implementations.

---

```text
OnboardingSIGDB1/                         # Monorepo
├── backend/                              # API .NET 8
│   ├── src/
│   │   ├── OnboardingSIGDB1.API/         # Controllers, Middleware, DTOs
│   │   ├── OnboardingSIGDB1.Domain/      # Entities, Interfaces, Notifications, Domain Service
│   │   ├── OnboardingSIGDB1.Data/        # DbContext, mappings, migrations, repositories
│   │   └── OnboardingSIGDB1.IOC/         # DI configuration and services
│   ├── tests/
│   │   └── OnboardingSIGDB1.UnitTests/   # Unit tests (100% mutation coverage via Stryker)
│   └── OnboardingSIGDB1.slnx             # Solution
├── frontend/                             # Angular 10.2.4 (Em desenvolvimento)
│   ├── src/
│   │   └── app/
│   └── angular.json
├── README.md
└── .gitignore
```

## 🗺️ Roadmap de Desenvolvimento / Development Roadmap

**Backend**
- [x] **Fase 1**: Configuração inicial / Initial Setup
- [x] **Fase 2**: DbContext e Mappings para Empresa / DbContext and Mappings for Company
- [x] **Fase 3**: Configuração de DI no IOC / Dependency Injection in IOC
- [x] **Fase 4**: Repositórios Genéricos e Unit of Work / Generic Repositories and Unit of Work
- [x] **Fase 5**: Domain Services e Validações / Domain Services and Validations
- [x] **Fase 6**: Controllers e Endpoints / Controllers and Endpoints
- [x] **Fase 7**: Testes e refinamentos (100% cobertura de mutação) / Tests and refinements (100% mutation coverage)

**Frontend**
- [x] **Fase 8**: Setup do projeto Angular / Angular project setup
- [ ] **Fase 9**: Implementação das telas / Screen implementation
---

## 🚀 Como Executar / How to Run

### Pré-requisitos / Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (Express, Developer ou superior)
- [Node.js 14.15.x](https://nodejs.org/)
- [Angular CLI 10.2.4](https://github.com/angular/angular-cli)
- [Git](https://git-scm.com/)

### Backend

1. **Clone o repositório / Clone the repo**

   ```bash
   git clone https://github.com/Guiandraade/OnboardingSIGDB1.git
   cd OnboardingSIGDB1/backend

2. **Configure a string de conexão / Configure connection string**

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OnboardingSIGDB1;Trusted_Connection=True;TrustServerCertificate=True;"

3. **Aplique as migrações / Apply migrations**

   ```bash
   dotnet ef database update

4. **Execute a API / Run the API**

   ```bash
   dotnet run --project src/OnboardingSIGDB1.API

5. **Acesse o Swagger / Access Swagger UI**

   ```
   https://localhost:5001/swagger

---

### Frontend

1. **Instale as dependências / Install dependencies**

   ```bash
   cd OnboardingSIGDB1/frontend
   npm install

2. **Execute o servidor de desenvolvimento / Run dev server**

   ```bash
   ng serve

3. **Acesse a aplicação / Access the app**

   ```
   http://localhost:4200

## 📌 Boas Práticas / Best Practices

### ✅ Validação / Validation

- **FluentValidation**
  
  **PT-BR**: Regras de validação desacopladas das entidades, garantindo um domínio mais limpo e reutilizável.

  **EN-US**: Validation rules decoupled from entities, ensuring a cleaner and more reusable domain.

---

### 🔄 Persistência e Transações / Persistence and Transactions

- **Unit of Work**
  
  **PT-BR**: Gerenciamento centralizado de transações envolvendo múltiplos repositórios.

  **EN-US**: Centralized transaction management involving multiple repositories.

- **EntityTypeConfiguration (Fluent API)**
 
  **PT-BR**: Configuração das entidades de forma explícita e organizada, evitando poluição nas classes de domínio.

  **EN-US**: Configuring entities explicitly and in an organized manner, avoiding pollution in the domain classes.

---

### ⚙️ Padrões e Produtividade / Standards and Productivity

- **AutoMapper**
  
  **PT-BR**: Redução de código boilerplate no mapeamento entre entidades e DTOs.

  **EN-US**: Reducing boilerplate code in the mapping between entities and DTOs.

- **Notifications Pattern**
  
  **PT-BR**: Acúmulo de erros de validação e regras de negócio sem uso excessivo de exceções.

  **EN-US**: Accumulation of validation errors and business rules without excessive use of exceptions.

---

### 🔐 Segurança / Security

- **User Secrets**
   
 **PT-BR**: Armazenamento seguro de credenciais e configurações sensíveis durante o desenvolvimento.

 **EN-US**: Secure storage of sensitive credentials and settings during development.

---

## 🤝 Contribuição / Contribution

1. Faça um fork do repositório / Fork the repositor
2. Crie uma branch para sua feature / Create a branch for your feature:
   
   ```bash
   git checkout -b feature/minha-feature

## 📄 Licença / License

Este projeto está sob a licença MIT / This project is under the MIT license.

**PT-BR**: Isso significa que você pode usar, modificar e distribuir este código livremente, desde que mantenha os devidos créditos.Para mais detalhes, consulte o arquivo [LICENSE](LICENSE).

**EN-US**: This means you can use, modify, and distribute this code freely, as long as you give proper credit. For more details, see the [LICENSE](LICENSE) file.

---

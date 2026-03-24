# OnboardingSIGDB1

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=for-the-badge)

## 🚀 OnboardingSIGDB1

PT-BR: Este projeto faz parte do processo de Onboarding técnico, focado em demonstrar domínio de arquitetura de software utilizando o ecossistema .NET 8. O objetivo é construir uma Web API robusta seguindo os padrões exigidos pelo time de desenvolvimento.

EN-US: This project is part of the technical onboarding process, focused on demonstrating software architecture expertise using the .NET 8 ecosystem. The goal is to build a robust Web API following the standards required by the development team.

## 📋 Sobre o Projeto / About the Project

| Tecnologias / Technologies | Versão / Version | Finalidade / Purpose |
|------------|--------|------------|
| .NET | 8 | Framework principal / Main framework |
| Entity Framework Core | 8 | ORM para acesso a dados / ORM for data access |
| SQL Server | 2019+ | Banco de dados relacional / Relational database |
| AutoMapper | 13+ | Mapeamento entre entidades e DTOs / Mapping between entities and DTOs |
| FluentValidation | 11+ |Validação de dados de entrada / Input data validation |
| Swagger | 6.6+ | Documentação interativa da API / Interactive API documentation |

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
OnboardingSIGDB1/
├── src/
│   ├── OnboardingSIGDB1.API/          # Controllers, Middleware, DTOs
│   ├── OnboardingSIGDB1.Domain/       # Entities, Interfaces, Notifications, Domain Service
│   ├── OnboardingSIGDB1.Data/         # Dbcontext, mappings, migrations, repositories
│   └── OnboardingSIGDB1.IOC/          # DI configuration and services
├── OnboardingSIGDB1.sln               # Solution
├── README.md
└── .gitignore                          
```

## 🗺️ Roadmap de Desenvolvimento / Development Roadmap

- [x] **Fase 1**: Configuração inicial / Initial Setup:**
- [x] **Fase 2**: DbContext e Mappings para Empresa / DbContext and Mappings for Company
- [x] **Fase 3**: Configuração de DI no IOC / Dependency Injection in IOC
- [ ] **Fase 4**: Repositórios Genéricos e Unit of Work / Generic Repositories and Unit of Work
- [ ] **Fase 5**: Domain Services e Validações / Domain Services and Validations
- [ ] **Fase 6**: Controllers e Endpoints / Controllers and Endpoints
- [ ] **Fase 7**: Testes e refinamentos / Tests and refinements
---

## 🚀 Como Executar / How to Run

### Pré-requisitos / Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (Express, Developer ou superior)
- [Git](https://git-scm.com/)

### Passos / Steps:

1. **Clone o repositório / Clone the repo**

   ```bash
   git clone https://github.com/Guiandraade/OnboardingSIGDB1.git
   cd OnboardingSIGDB1

2. **Configure a string de conexão / Configure connection string**

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OnboardingSIGDB1;Trusted_Connection=True;TrustServerCertificate=True;"

3. **Aplique as migrações / Apply migrations**

   ```bash
   dotnet ef database update

4. **Execute a API / Run the API**
   
   ```bash
   dotnet run

5. Acesse o Swagger / Access Swagger UI**
  
    ```bash
    https://localhost:{porta}/swagger

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

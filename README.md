# OnboardingSIGDB1

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework](https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow?style=for-the-badge)

## 🧩 Sobre o Projeto

Este é o projeto de onboarding para o programa de formação de trainees. O objetivo é desenvolver uma API RESTful com .NET 8, aplicando boas práticas de desenvolvimento como DDD, UnitOfWork, FluentValidation e o padrão de Notifications.

O sistema gerencia empresas, funcionários e cargos, com relacionamentos complexos e regras de negócio específicas.

## 📋 Requisitos Técnicos

| Tecnologia | Versão | Finalidade |
|------------|--------|------------|
| .NET | 8 | Framework principal |
| Entity Framework Core | 8 | ORM para acesso a dados |
| SQL Server | 2019+ | Banco de dados relacional |
| AutoMapper | 13+ | Mapeamento entre entidades e DTOs |
| FluentValidation | 11+ | Validação de dados de entrada |
| Swagger | 6.6+ | Documentação interativa da API |

---

## 🏗️ Estrutura do Projeto

```text
OnboardingSIGDB1/                         
├── src/                                  
│   ├── OnboardingSIGDB1.Domain/                # Regras de negócio e entidades
│   ├── OnboardingSIGDB1.Application/           # Casos de uso, DTOs e serviços
│   ├── OnboardingSIGDB1.Infrastructure/        # Persistência de dados e integrações externas
│   └── OnboardingSIGDB1.API/                   # Camada de entrada (controllers/endpoints)
├── OnboardingSIGDB1.sln                        # Arquivo da solution
├── README.md                             
└── .gitignore                             
```

## 🗺️ Roadmap de Desenvolvimento

- [x] **Setup inicial**: Configuração do projeto, estrutura de pastas e dependências
- [ ] **Fase 1**: Entidades e configurações de banco (Empresa, Funcionario, Cargo)
- [ ] **Fase 2**: DTOs e AutoMapper
- [ ] **Fase 3**: FluentValidation e Notifications Pattern
- [ ] **Fase 4**: Repositórios e UnitOfWork
- [ ] **Fase 5**: Serviços e regras de negócio
- [ ] **Fase 6**: Controllers e endpoints
- [ ] **Fase 7**: Testes e refinamentos

---

## 🚀 Como executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (Express, Developer ou superior)
- [Git](https://git-scm.com/)

### Passos:

1. **Clone o repositório**

   ```bash
   git clone https://github.com/Guiandraade/OnboardingSIGDB1.git
   cd OnboardingSIGDB1

2. **Configure a string de conexão (User Secrets)**

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=OnboardingSIGDB1;Trusted_Connection=True;TrustServerCertificate=True;"

3. **Aplique as migrações**

   ```bash
   dotnet ef database update

4. **Execute a API**
   
   ```bash
   dotnet run

5. **Acesse o Swagger UI**
  
    ```bash
    https://localhost:{porta}/swagger

## 📌 Boas Práticas Aplicadas

Este projeto segue um conjunto de boas práticas amplamente adotadas no desenvolvimento backend com .NET, visando **manutenibilidade, escalabilidade e organização de código**.

---

## 🧠 Arquitetura

API → Application → Domain  
        ↓  
   Infrastructure

---

### ✅ Validação

- **FluentValidation**  
  Regras de validação desacopladas das entidades, garantindo um domínio mais limpo e reutilizável.

---

### 🔄 Persistência e Transações

- **Unit of Work**  
  Gerenciamento centralizado de transações envolvendo múltiplos repositórios.

- **EntityTypeConfiguration (Fluent API)**  
  Configuração das entidades de forma explícita e organizada, evitando poluição nas classes de domínio.

---

### ⚙️ Padrões e Produtividade

- **AutoMapper**  
  Redução de código boilerplate no mapeamento entre entidades e DTOs.

- **Notifications Pattern**  
  Acúmulo de erros de validação e regras de negócio sem uso excessivo de exceções.

---

### 🔐 Segurança

- **User Secrets**  
  Armazenamento seguro de credenciais e configurações sensíveis durante o desenvolvimento.

---

## 🤝 Contribuição

Contribuições são bem-vindas! Para colaborar com o projeto:

1. Faça um fork do repositório  
2. Crie uma branch para sua feature:
   
   ```bash
   git checkout -b feature/minha-feature

## 📄 Licença

Este projeto está sob a licença MIT.  

Isso significa que você pode usar, modificar e distribuir este código livremente, desde que mantenha os devidos créditos.

Para mais detalhes, consulte o arquivo [LICENSE](LICENSE).

---

Este é um projeto de estudo para o programa de formação de trainees. Sugestões e feedbacks são bem-vindos!

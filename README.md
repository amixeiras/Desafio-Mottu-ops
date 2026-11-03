# Desafio Backend - Sistema de Manutenção de Motos e Locação

Aplicação desenvolvida em **.NET 8** utilizando arquitetura **Domain-Driven Design (DDD)** e **Clean Architecture**, seguindo rigorosamente a especificação do Swagger do desafio técnico.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8 (C#)**
- **Entity Framework Core 8** com **PostgreSQL** (Npgsql)
- **MassTransit & RabbitMQ** para mensageria assíncrona orientada a eventos
- **Docker & Docker Compose** para orquestração de containers
- **xUnit & Moq** para testes unitários e de integração
- **Swagger / OpenAPI** com documentação interativa

---

## 🏛️ Arquitetura do Projeto (DDD)

A solução está estruturada em camadas bem definidas e desacopladas:

```text
DesafioBackend/
├── src/
│   ├── DesafioBackend.Domain/              # Regras de negócio puras, entidades, VOs, eventos e interfaces
│   │   ├── Common/                         # BaseEntity, IAggregateRoot, IDomainEvent, ValueObject
│   │   ├── Entities/                       # Motorcycle, DeliveryDriver, Rental, MotorcycleNotification
│   │   ├── Enums/                          # DriverLicenseType (A, B, AB), RentalPlanType (7, 15, 30, 45, 50)
│   │   ├── Events/                         # MotorcycleRegisteredEvent
│   │   ├── Exceptions/                     # DomainException
│   │   └── Repositories/                   # Interfaces dos repositórios e Unit of Work
│   │
│   ├── DesafioBackend.Application/         # Casos de uso, orquestração e DTOs
│   │   ├── Common/                         # Interfaces de Storage, Mensageria e Exceções
│   │   ├── DTOs/                           # Requests e Responses conforme contrato Swagger
│   │   └── Services/                       # MotorcycleService, DeliveryDriverService, RentalService
│   │
│   ├── DesafioBackend.Infrastructure/      # Persistência, Mensageria e Armazenamento
│   │   ├── Persistence/                    # AppDbContext, Mapeamentos EF Core, Repositórios
│   │   ├── Messaging/                      # MassTransit com RabbitMQ e Consumers
│   │   └── Storage/                        # LocalStorageService (armazenamento de arquivos de CNH)
│   │
│   ├── DesafioBackend.Web/                 # Interface Web Administrativa (ASP.NET Core MVC)
│   │   ├── Controllers/                    # AdminController, AuthController, HomeController
│   │   ├── Services/                       # ApiClient, AuthService, JwtTokenHandler
│   │   ├── Views/                          # Dashboards e telas de login/gestão
│   │   └── Dockerfile                      # Containerização da aplicação Web
│   │
│   └── DesafioBackend.Api/                 # Entrada HTTP (REST API)
│       ├── Controllers/                    # MotosController, EntregadoresController, LocacaoController, AuthController, EntregasController
│       ├── Middlewares/                    # GlobalExceptionMiddleware (respostas de erro padronizadas)
│       └── Dockerfile                      # Build multi-stage otimizado
│
└── tests/
    ├── DesafioBackend.UnitTests/           # Testes unitários de domínio e serviços
    └── DesafioBackend.IntegrationTests/    # Testes de integração de endpoints
```

---

## 🚀 Como Executar

### 1. Com Docker Compose (Recomendado)

Certifique-se de que o Docker está instalado e em execução. Na raiz do repositório, execute:

```bash
docker compose up -d --build
```

Os serviços inicializarão automaticamente:
- **PostgreSQL**: porta host `5433` (container `5432`)
- **RabbitMQ**: portas `5672` (mensagens) e `15672` (painel gerencial web)
- **API**: porta `5000` (http://localhost:5000)
- **Web (Painel Administrativo)**: porta `5001` (http://localhost:5001)

As migrações do banco de dados são aplicadas **automaticamente** na inicialização da API.

Acesse as aplicações em:
👉 **Swagger da API:** [http://localhost:5000](http://localhost:5000)  
👉 **Painel Administrativo Web:** [http://localhost:5001](http://localhost:5001) *(login padrão: usuário `admin`, senha `admin`)*  
👉 **Painel RabbitMQ:** [http://localhost:15672](http://localhost:15672) *(usuário: `guest`, senha: `guest`)*

> 💡 **Guia Completo de Credenciais e Depuração no Visual Studio**: Consulte o arquivo [`SERVICOS_E_DEBUG.md`](./SERVICOS_E_DEBUG.md) para detalhes completos de portas, connection strings, usuários, senhas e passo a passo para depurar com F5 ou anexar ao Docker.

Para parar os serviços:
```bash
docker compose down
```

---

### 2. Execução Local (.NET CLI)

1. Inicie as dependências externas (PostgreSQL e RabbitMQ):
```bash
docker compose up -d db rabbitmq
```

2. Execute as migrações e inicie a API:
```bash
dotnet run --project src/DesafioBackend.Api
```

---

## 🧪 Executando os Testes

Execute a suíte completa de testes unitários e de integração com:

```bash
dotnet test DesafioBackend.sln
```

---

## 📋 Funcionalidades Implementadas

### Motos (`/motos`)
- `POST /motos`: Cadastro de nova moto com validação de placa única.
  - Publicação de evento de mensageria via RabbitMQ.
  - Consumidor dedicado que detecta motos do ano `2024` e grava notificação no banco de dados.
- `GET /motos`: Consulta de motos cadastradas com filtro opcional por placa (`?placa=XYZ`).
- `GET /motos/{id}`: Consulta detalhada de moto por identificador.
- `PUT /motos/{id}/placa`: Atualização da placa da moto (com validação de duplicidade).
- `DELETE /motos/{id}`: Remoção de motos incorretas (bloqueado caso possua locações registradas).

### Entregadores (`/entregadores`)
- `POST /entregadores`: Cadastro de entregador com CNPJ único, CNH única e categorias válidas (`A`, `B`, `A+B`).
- `POST /entregadores/{id}/cnh`: Upload da foto da CNH nos formatos `PNG` ou `BMP` salvos em serviço de storage desacoplado (não armazenado no banco).

### Locação (`/locacao`)
- `POST /locacao`: Criação de locação de moto para entregador.
  - Validação de que a data de início deve ser o primeiro dia após a data de criação.
  - Validação de que apenas entregadores com habilitação categoria `A` ou `A+B` podem alugar motos.
  - Planos disponíveis:
    - 7 dias (R$ 30,00/dia)
    - 15 dias (R$ 28,00/dia)
    - 30 dias (R$ 22,00/dia)
    - 45 dias (R$ 20,00/dia)
    - 50 dias (R$ 18,00/dia)
- `GET /locacao/{id}`: Consulta dos detalhes da locação por identificador.
- `PUT /locacao/{id}/devolucao`: Informa a data de devolução e calcula o valor final da locação:
  - Devolução antecipada: aplica multa de 20% (plano 7 dias) ou 40% (plano 15 dias) sobre as diárias não efetivadas.
  - Devolução tardia: aplica acréscimo de R$ 50,00 por diária adicional.


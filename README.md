# Coworking Booking

API de reservas para espaços de **coworking**, construída como projeto de portfólio para aplicar na prática **Clean Architecture**, **Domain-Driven Design (DDD)**, **cache distribuído**, **mensageria assíncrona** e **persistência transacional** com MongoDB.

## Funcionalidades

- **Cadastro de workspaces**: criação com tipo, preço por hora, recursos e geolocalização (GeoJSON); slug gerado automaticamente a partir do nome.
- **Consulta por slug** com cache Redis (cache-aside + stale-while-revalidate).
- **Disponibilidade recorrente**: definição de janela de disponibilidade com recorrência `DAILY` ou `WEEKLY` e timezone. Os slots de calendário são gerados de forma assíncrona por um Worker via fila SQS.
- **Gestão de status** do workspace (`draft`, `available`, `reserved`, `maintenance`, `unavailable`).
- **Reservas** em um slot de calendário, com validação de sobreposição e cálculo de preço total.
- **Limpeza agendada** (Hangfire) dos slots de calendário inativos.

## Stack

| Área | Tecnologia |
|---|---|
| Runtime | .NET 10 / ASP.NET Core |
| Persistência | MongoDB 8 (replica set `rs0`, transações) |
| Cache e locks | Redis |
| Mensageria | AWS SQS (LocalStack em ambiente local) |
| Jobs agendados | Hangfire (storage em Redis) |
| Observabilidade | Serilog + Seq, health checks |
| Validação | FluentValidation |
| Documentação da API | OpenAPI + Scalar |
| Testes | NUnit + Moq |

## Destaques técnicos

- **Clean Architecture + DDD**: agregados e value objects com invariantes no domínio, casos de uso com `Result<T>` e infraestrutura isolada atrás de interfaces (ports & adapters).
- **Cache-Aside + Stale-While-Revalidate**: leituras servidas do Redis; entradas antigas continuam sendo servidas enquanto um Worker revalida o cache em segundo plano via SQS.
- **Mutex distribuído em Redis**: `SET NX` com TTL e liberação atômica via script Lua (compare-and-delete), usado para garantir que só uma instância execute as migrations.
- **Processamento assíncrono**: geração de slots de calendário recorrentes em Worker, a partir de eventos publicados em fila.
- **Transações MongoDB** (replica set) para substituir slots de forma atômica, com soft delete e limpeza agendada via Hangfire.

Detalhes e diagramas em [docs/architecture.md](docs/architecture.md).

## Estrutura do projeto

```
backend/
├── CoworkingBooking.Api                # Controllers REST, OpenAPI, health checks, Hangfire
├── CoworkingBooking.Application        # Casos de uso, DTOs, validadores, mapeadores, cache
├── CoworkingBooking.Core               # Domínio: entidades, value objects, enums, interfaces de repositório
├── CoworkingBooking.Infrastructure     # MongoDB, Redis, publishers SQS, migrations, cron jobs
├── CoworkingBooking.Contracts          # Contratos das mensagens trafegadas nas filas
├── CoworkingBooking.Shared             # Result/Error, ApiResponse, GeoJson, utilitários
├── CoworkingBooking.Workers            # Consumidores SQS (BackgroundService)
├── CoworkingBooking.Application.Tests  # Testes unitários dos casos de uso
└── docker-compose.yml                  # Infra local
```

Detalhes em [docs/architecture.md](docs/architecture.md).

## Como rodar localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker e Docker Compose
- OpenSSL (para gerar o keyfile do MongoDB)

### 1. Gerar o keyfile do replica set MongoDB

O arquivo não é versionado e precisa existir antes de subir os containers:

```bash
openssl rand -base64 756 > backend/docker/mongo-keyfile
```

### 2. Subir a infraestrutura

Sobe MongoDB (3 nós), Redis, Seq, LocalStack (SQS) e as ferramentas de administração. As filas SQS são criadas automaticamente pelo script `docker/localstack/init-aws.sh`.

```bash
cd backend
docker compose up -d
```

### 3. Rodar a API e o Worker

A API e o Worker são processos separados; ambos precisam estar rodando para o fluxo de disponibilidade funcionar.

```bash
cd backend
dotnet run --project CoworkingBooking.Api
dotnet run --project CoworkingBooking.Workers   # em outro terminal
```

No VS Code, use a configuração de debug composta **"Api + Workers"** (`.vscode/launch.json`).

Na inicialização, a API executa as migrations pendentes do MongoDB (índices e backfills).

### Imagens Docker (opcional)

```bash
cd backend
docker build -f CoworkingBooking.Api/Dockerfile     -t cwb-api     .
docker build -f CoworkingBooking.Workers/Dockerfile -t cwb-workers .
```

## Endereços úteis

| Serviço | URL |
|---|---|
| API | http://localhost:5058 |
| Documentação interativa (Scalar) | http://localhost:5058/docs |
| OpenAPI JSON | http://localhost:5058/openapi/v1.json |
| Dashboard Hangfire | http://localhost:5058/jobs |
| Health checks | http://localhost:5058/health/live · http://localhost:5058/health/ready |
| Seq (logs) | http://localhost:8081 |
| SQS Admin | http://localhost:3999 |
| Redis Insight | http://localhost:5540 |

Scalar e OpenAPI ficam disponíveis apenas no ambiente `Development`.

## Testes

```bash
cd backend
dotnet test
```

## Documentação

- [Arquitetura](docs/architecture.md): camadas, padrões, fluxos assíncronos, cache e jobs.
- [Domínio](docs/domain.md): entidades, regras de negócio, enums e modelo de dados.
- [API](docs/api.md): endpoints com exemplos de requisição e resposta.
- [Infraestrutura](docs/infrastructure.md): serviços Docker, filas, configuração.

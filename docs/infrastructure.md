# Infraestrutura

Toda a infraestrutura local é definida em `backend/docker-compose.yml`.

## Serviços

| Serviço | Imagem | Portas | Função |
|---|---|---|---|
| `mongo1`, `mongo2`, `mongo3` | `mongo:8.0` | 27017, 27018, 27019 | Replica set `rs0` (necessário para transações). O healthcheck do `mongo1` inicializa o replica set. |
| `redis` | `redis:8.10.2` | 6379 | Cache, lock distribuído e storage do Hangfire. Configuração e usuários ACL em `docker/redis/redis.conf` (AOF habilitado). |
| `redis-insight` | `redis/redisinsight` | 5540 | Interface web para o Redis. |
| `localstack` | `localstack/localstack` | 4566 | Emulação do AWS SQS (`SERVICES=sqs`, região `us-east-1`, persistência habilitada). |
| `sqs-admin` | `pacovk/sqs-admin` | 3999 | Interface web para inspecionar as filas. |
| `seq` | `datalust/seq` | 5341 (ingestão), 8081 (UI) | Agregação e busca de logs estruturados. |

### MongoDB

- Os nós se autenticam entre si com o keyfile `backend/docker/mongo-keyfile`, que deve ser gerado localmente (veja o [README](../README.md#1-gerar-o-keyfile-do-replica-set-mongodb)).
- Os membros do replica set são registrados como `host.docker.internal:<porta>`, permitindo que a API rodando no host se conecte ao replica set.
- Coleções: `workspaces`, `workspaces_calendar` e `_migrations`.

### Filas SQS

Criadas na inicialização do LocalStack pelo script `backend/docker/localstack/init-aws.sh`. Os nomes ficam centralizados em `CoworkingBooking.Shared/Enums/Queues.cs`.

| Fila | Publicada por | Consumida por | Mensagem |
|---|---|---|---|
| `workspace-availability` | Api (`UpdateAvailabilityUseCase`) | Worker (`UpdatedWorkspaceAvailabilityConsumer`) | `UpdatedWorkspaceAvailabilityEvent` |
| `refresh-cache` | Api (`RedisCacheRepository`) | Worker (`RefreshCacheConsumer`) | `RefreshCacheEvent` |

## Configuração

Configurações por ambiente ficam em `appsettings.Development.json` de `CoworkingBooking.Api` e `CoworkingBooking.Workers`. Arquivos `*.local.json` são ignorados pelo Git e podem ser usados para valores locais.

| Chave | Descrição |
|---|---|
| `Application:Name` | Nome da aplicação, adicionado aos logs. |
| `Serilog` | Níveis de log e enrichers. |
| `Seq:ServerUrl` | URL de ingestão do Seq (padrão `http://localhost:5341`). |
| `MongoDbSettings:ConnectionString` | Connection string do MongoDB. |
| `MongoDbSettings:DatabaseName` | Nome do banco (`CoworkingBookingDb`). |
| `RedisSettings:ConnectionString` | Connection string do Redis (com usuário ACL). |
| `AWS:Region` | Região AWS (`us-east-1`). |
| `AWS:ServiceURL` | Endpoint do SQS; localmente `http://localhost:4566` (LocalStack). |
| `AWS:Profile`, `AWS:AccessKey`, `AWS:SecretKey` | Credenciais AWS. |

## Execução

### Portas da aplicação

Definidas em `CoworkingBooking.Api/Properties/launchSettings.json`:

- HTTP: `http://localhost:5058`
- HTTPS: `https://localhost:7143`

### VS Code

- `.vscode/launch.json`: configurações **Api**, **Workers** e a composta **Api + Workers**.
- `.vscode/tasks.json`: tarefas `build-api`, `build-workers` e `build-all`.

### Docker

Os Dockerfiles usam build multi-stage (`sdk:10.0` para build) e devem ser executados com `backend/` como contexto:

| Dockerfile | Imagem final | Observações |
|---|---|---|
| `CoworkingBooking.Api/Dockerfile` | `aspnet:10.0` | Expõe a porta 8080 e roda com usuário não-root. |
| `CoworkingBooking.Workers/Dockerfile` | `runtime:10.0` | `DOTNET_ENVIRONMENT=Production`. |

## Observabilidade

- **Logs**: Serilog com saída para console e Seq, enriquecidos com máquina, processo, thread, detalhes de exceção e nome da aplicação.
- **Health checks** (Api):
  - `/health/live`: indica que o processo está no ar, sem checar dependências.
  - `/health/ready`: verifica conectividade com MongoDB e Redis.
- **Jobs**: dashboard do Hangfire em `/jobs`.

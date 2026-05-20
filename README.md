# NexusPay

Plataforma de pagamentos construída com .NET, gRPC e SQL Server.

## Arquitetura

- **NexusPay.Api** — API REST (ASP.NET Core Minimal APIs)
- **NexusPay.Client** — gRPC clients consumidos pela API
- **NexusPay.Contracts** — Contratos Protobuf compartilhados
- **NexusPay.Server** — Serviços gRPC (autenticação, usuários, transações)
- **NexusPay.Data** — Acesso a dados via ADO.NET + Stored Procedures
- **NexusPay.Shared** — Models, helpers e extensões compartilhadas

## Pré-requisitos

- .NET 9
- Docker Desktop

## Como rodar

### 1. Suba o banco de dados

```bash
docker-compose up -d
```

### 2. Execute os scripts SQL

Execute na ordem dentro do CloudBeaver (`http://localhost:8978`) ou SSMS:

### 3. Configure o ambiente

Verifique os `appsettings.json` de `NexusPay.Api` e `NexusPay.Server` com as configurações corretas de conexão e JWT.

### 4. Rode os projetos

```bash
# Terminal 1 — gRPC Server
cd NexusPay.Server && dotnet run

# Terminal 2 — API
cd NexusPay.Api && dotnet run
```

## Banco de Dados

Acesse o CloudBeaver em `http://localhost:8978` para gerenciar o banco via browser.

| Campo | Valor |
|---|---|
| Driver | SQL Server |
| Host | `sqlserver` |
| Port | `1433` |
| Database | `NexusPay` |
| Username | `sa` |
| Password | `NexusPay@123` |

## Autenticação

A API utiliza JWT Bearer Token. Após o login, inclua o token no header:
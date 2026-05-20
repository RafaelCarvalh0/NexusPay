# NexusPay 💳

> Plataforma de pagamentos de alta performance construída com arquitetura orientada a serviços, comunicação via gRPC, autenticação JWT e gerenciamento de sessão via Redis.

---

## Por que gRPC?

A comunicação entre a API REST e os serviços internos utiliza **gRPC** ao invés de REST convencional por razões técnicas deliberadas:

- **Performance** — gRPC usa HTTP/2 com multiplexação de streams e serialização binária via Protocol Buffers, chegando a ser até 10x mais rápido que JSON/REST em comunicação interna
- **Contrato forte** — os arquivos `.proto` definem um contrato tipado e versionável entre os serviços, eliminando ambiguidades
- **Geração de código** — clients e servers são gerados automaticamente a partir do `.proto`, zero código manual de integração
- **Streaming nativo** — suporte a streaming bidirecional para operações em tempo real

---

## Arquitetura

```
Cliente HTTP
     │
     ▼
┌─────────────────────┐
│   NexusPay.Api      │  ASP.NET Core Minimal APIs + JWT Auth
│   (REST :5000)      │  + TokenValidationMiddleware (Redis)
└────────┬────────────┘
         │ gRPC (HTTP/2 + Protobuf)
         ▼
┌─────────────────────┐
│  NexusPay.Server    │  Serviços gRPC internos
│  (gRPC :7199)       │
└────────┬────────────┘
         │ ADO.NET + Stored Procedures     Redis (Sessões)
         ▼                                      │
┌─────────────────────┐             ┌───────────┴──────────┐
│   SQL Server        │             │   Redis              │
│   Docker (:1433)    │             │   Docker (:6379)     │
└─────────────────────┘             └──────────────────────┘
```

### Projetos

| Projeto | Responsabilidade |
|---|---|
| `NexusPay.Api` | API REST — endpoints, autenticação, validação |
| `NexusPay.Client` | gRPC clients consumidos pela API |
| `NexusPay.Contracts` | Contratos Protobuf (`.proto`) compartilhados |
| `NexusPay.Server` | Serviços gRPC — lógica de negócio |
| `NexusPay.Data` | Acesso a dados via ADO.NET + Stored Procedures |
| `NexusPay.Shared` | Models, helpers e extensões compartilhadas |

---

## Stack

- **.NET 10** — Runtime e framework
- **ASP.NET Core Minimal APIs** — Endpoints REST enxutos e performáticos
- **gRPC / Protocol Buffers** — Comunicação interna entre serviços
- **SQL Server 2022** — Banco de dados relacional
- **ADO.NET** — Acesso a dados sem ORM, via Stored Procedures
- **FluentValidation** — Validação de entrada declarativa
- **BCrypt.Net** — Hash seguro de senhas
- **JWT Bearer** — Autenticação stateless
- **Redis** — Gerenciamento de sessão e blacklist de tokens
- **Scalar / OpenAPI** — Documentação interativa de endpoints
- **Docker** — Containerização do banco de dados, Redis e CloudBeaver
- **CloudBeaver** — Interface web para gerenciamento do banco

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## Como rodar

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/nexuspay.git
cd nexuspay
```

### 2. Suba os containers

```bash
docker-compose up -d
```

Isso sobe o **SQL Server**, **Redis** e **CloudBeaver** automaticamente.

### 3. Configure a conexão no CloudBeaver

Acesse `http://localhost:8978` e conecte ao SQL Server:

| Campo | Valor |
|---|---|
| Driver | SQL Server |
| Host | `sqlserver` |
| Port | `1433` |
| Database | `NexusPay` |
| Username | `sa` |
| Password | `NexusPay@123` |

### 4. Execute os scripts SQL na ordem

Execute todos os arquivos da pasta `Database/` na seguinte ordem:

1. `Tables/`
2. `StoredProcedures/`
3. `Seeds/Development/` ← apenas em desenvolvimento

### 5. Configure o ambiente

Verifique os `appsettings.json` de `NexusPay.Api` e `NexusPay.Server`:

```json
{
  "ConnectionStrings": {
    "SQL": "Server=localhost,1433; Database=NexusPay; User Id=sa; Password=NexusPay@123; TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "NexusPay",
    "Audience": "NexusPayUsers",
    "Key": "sua-chave-secreta-minimo-32-caracteres",
    "ExpirationMinutes": 60
  }
}
```

> ⚠️ A `Key` JWT deve ser **idêntica** nos dois projetos.

### 6. Rode os projetos

```bash
# Terminal 1 — gRPC Server
cd NexusPay.Server && dotnet run

# Terminal 2 — API REST
cd NexusPay.Api && dotnet run
```

Acesse a documentação interativa da API em `http://localhost:5000/scalar/v1`

---

## Autenticação e Sessão

A API utiliza **JWT Bearer Token** com gerenciamento de sessão via **Redis**. Após o login, inclua o token em todas as requisições autenticadas:

```
Authorization: Bearer {token}
```

### Comportamento de sessão

- **Novo login com sessão ativa** — o token anterior é automaticamente invalidado. Apenas uma sessão ativa por usuário é permitida
- **Logout** — invalida o token imediatamente no Redis, independente do tempo de expiração
- **Token revogado** — qualquer requisição com token inválido retorna `401 Unauthorized`

### Endpoints de autenticação

**Login:**
```http
POST /auth/login
Content-Type: application/json

{
  "email": "default.user@gmail.com",
  "password": "1234"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGci...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "userId": "6b6e433e-...",
  "userName": "Default User",
  "role": "User"
}
```

**Logout:**
```http
POST /auth/logout
Authorization: Bearer {token}
```

### Credenciais do usuário default (desenvolvimento)

| Campo | Valor |
|---|---|
| Email | `default.user@gmail.com` |
| Senha | `1234` |

---

## Estrutura do banco de dados

```
Database/
├── Tables/
├── StoredProcedures/
└── Seeds/
      └── Development/
```

> Scripts versionados com prefixo `V{n}__` — nunca edite um script já commitado, sempre crie uma nova versão.

---

## Segurança

- Senhas armazenadas com **BCrypt** (hash + salt automático)
- Tokens JWT com expiração configurável e `ClockSkew = Zero`
- **Sessão única por usuário** — novo login invalida automaticamente a sessão anterior
- **Blacklist de tokens no Redis** — invalidação imediata no logout, TTL automático
- **TokenValidationMiddleware** — verifica blacklist do Redis em toda requisição autenticada
- CORS configurado por origem explícita
- Validação de entrada em todas as rotas via FluentValidation
- Erros internos nunca expostos ao cliente — tratados pelo `GlobalExceptionHandler`

---

## Licença

MIT

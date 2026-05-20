# NexusPay 💳

> Plataforma de pagamentos de alta performance construída com arquitetura orientada a serviços, comunicação via gRPC e autenticação JWT.

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
│   (REST :5000)      │
└────────┬────────────┘
         │ gRPC (HTTP/2 + Protobuf)
         ▼
┌─────────────────────┐
│  NexusPay.Server    │  Serviços gRPC internos
│  (gRPC :7199)       │
└────────┬────────────┘
         │ ADO.NET + Stored Procedures
         ▼
┌─────────────────────┐
│   SQL Server        │  Docker Container
│   (:1433)           │
└─────────────────────┘
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
- **Docker** — Containerização do banco de dados
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

### 2. Suba o banco de dados e CloudBeaver

```bash
docker-compose up -d
```

Aguarde os containers subirem e acesse o CloudBeaver em `http://localhost:8978`.

### 3. Configure a conexão no CloudBeaver

| Campo | Valor |
|---|---|
| Driver | SQL Server |
| Host | `sqlserver` |
| Port | `1433` |
| Database | `NexusPay` |
| Username | `sa` |
| Password | `NexusPay@123` |

### 4. Execute os scripts SQL na ordem

**Obrigatórios:**
```
Database/Tables/V1__Users.sql
Database/StoredProcedures/V1__SP_CREATE_USER.sql
Database/StoredProcedures/V1__SP_LOGIN_USER.sql
```

**Apenas em desenvolvimento:**
```
Database/Seeds/Development/V1__DefaultUser.sql
```

### 5. Configure o ambiente

Verifique os `appsettings.json` de `NexusPay.Api` e `NexusPay.Server`:

```json
{
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

---

## Autenticação

A API utiliza **JWT Bearer Token**. Após o login, inclua o token em todas as requisições autenticadas:

```
Authorization: Bearer {token}
```

### Endpoint de login

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
│     └── V1__Users.sql
├── StoredProcedures/
│     ├── V1__SP_CREATE_USER.sql
│     └── V1__SP_LOGIN_USER.sql
└── Seeds/
      └── Development/
            └── V1__DefaultUser.sql
```

> Scripts versionados com prefixo `V{n}__` — nunca edite um script já commitado, sempre crie uma nova versão.

---

## Segurança

- Senhas armazenadas com **BCrypt** (hash + salt automático)
- Tokens JWT com expiração configurável e `ClockSkew = Zero`
- CORS configurado por origem explícita
- Validação de entrada em todas as rotas via FluentValidation
- Erros internos nunca expostos ao cliente — tratados pelo `GlobalExceptionHandler`

---

## Licença

MIT

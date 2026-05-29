🌐 [English version](README.en.md)

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
│  (gRPC :7199)       │  + MailKit (SMTP)
└────────┬────────────┘
         │ ADO.NET + Stored Procedures     Redis (Sessões + Reset Tokens)
         ▼                                      │
┌─────────────────────┐             ┌───────────┴──────────┐
│   SQL Server        │             │   Redis              │
│   Docker (:1433)    │             │   Docker (:6379)     │
└─────────────────────┘             └──────────────────────┘

┌─────────────────────┐
│   Nginx             │  Templates HTML estáticos
│   Docker (:5500)    │  (reset-password.html)
└─────────────────────┘
```

### Projetos

| Projeto | Responsabilidade |
|---|---|
| `NexusPay.Api` | API REST — endpoints, autenticação, validação |
| `NexusPay.Client` | gRPC clients consumidos pela API |
| `NexusPay.Contracts` | Contratos Protobuf (`.proto`) compartilhados |
| `NexusPay.Server` | Serviços gRPC — lógica de negócio + envio de e-mail |
| `NexusPay.Data` | Acesso a dados via ADO.NET + Stored Procedures |
| `NexusPay.Shared` | Models, helpers e extensões compartilhadas |
| `NexusPay.Templates` | Templates HTML estáticos servidos via Nginx |

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
- **Redis** — Gerenciamento de sessão, blacklist de tokens e reset tokens
- **MailKit** — Envio de e-mails transacionais via SMTP
- **Nginx** — Servidor de arquivos estáticos para templates HTML
- **Scalar / OpenAPI** — Documentação interativa de endpoints
- **Docker** — Containerização completa (SQL Server, Redis, Nginx, CloudBeaver)
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

Isso sobe automaticamente:

| Container | Endereço |
|---|---|
| SQL Server | `localhost:1433` |
| Redis | `localhost:6379` |
| CloudBeaver | `http://localhost:8978` |
| Frontend (Nginx) | `http://localhost:5500` |

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
3. `Seeds/` ← seeds de roles obrigatórias + usuário admin para desenvolvimento

### 5. Configure o ambiente

Crie os arquivos `appsettings.json` em `NexusPay.Api` e `NexusPay.Server`
baseando-se no `appsettings.EXAMPLE.json`.

```json
{
  "ConnectionStrings": {
    "SQL": "Server=localhost,1433; Database=NexusPay; User Id=sa; Password=YOUR_PASSWORD; TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "NexusPay",
    "Audience": "YOUR_AUDIENCE",
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS",
    "ExpirationMinutes": 60
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "FromEmail": "seu-email@gmail.com",
    "Password": "YOUR_APP_PASSWORD",
    "FromName": "NexusPay"
  },
  "FrontendUrl": "http://localhost:5500"
}
```

> ⚠️ A `Key` JWT deve ser **idêntica** nos dois projetos.  
> ⚠️ Para o Gmail, utilize uma **App Password** — não a senha da conta.

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

### Fluxo de recuperação de senha

1. `POST /auth/forgot-password` — envia e-mail com link de redefinição contendo um token com expiração de **1 minuto**
2. Usuário clica no link e é direcionado ao frontend (`http://localhost:5500/reset-password.html`)
3. `POST /auth/reset-password` — valida o token no Redis e atualiza a senha com BCrypt

### Endpoints de autenticação

**Login:**
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@nexuspay.com",
  "password": "sua-senha"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGci...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "userId": "6b6e433e-...",
  "userName": "Admin",
  "role": "Admin"
}
```

**Forgot Password:**
```http
POST /auth/forgot-password
Content-Type: application/json

{
  "email": "usuario@exemplo.com"
}
```

**Reset Password:**
```http
POST /auth/reset-password
Content-Type: application/json

{
  "email": "usuario@exemplo.com",
  "token": "token-recebido-por-email",
  "newPassword": "nova-senha"
}
```

**Logout:**
```http
GET /auth/logout
Authorization: Bearer {token}
```

### Credenciais do usuário admin (desenvolvimento)

| Campo | Valor |
|---|---|
| Email | `admin@nexuspay.com` |
| Senha | `1234` |

---

## Autorização por Roles

| Role | Acesso |
|---|---|
| `Admin` | Todos os endpoints, incluindo exclusão de usuários |
| `User` | Endpoints próprios — login, logout, update do próprio perfil |

---

## Estrutura do projeto

```
NexusPay/
├── docker-compose.yml
├── nginx.conf                  ← configuração do servidor de templates
├── Database/
│     ├── Tables/
│     │     ├── ROLES.sql
│     │     └── USERS.sql
│     ├── StoredProcedures/
│     └── Seeds/
│           ├── ROLES_SEEDS.sql   ← roles obrigatórias
│           └── ADMIN_USER.sql    ← usuário admin para desenvolvimento
└── NexusPay.Templates/
      └── reset-password.html
```

> Scripts versionados com prefixo `V{n}__` — nunca edite um script já commitado, sempre crie uma nova versão.

---

## Segurança

- Senhas armazenadas com **BCrypt** (hash + salt automático)
- Tokens JWT com expiração configurável e `ClockSkew = Zero`
- **Sessão única por usuário** — novo login invalida automaticamente a sessão anterior
- **Blacklist de tokens no Redis** — invalidação imediata no logout, TTL automático
- **Reset tokens no Redis** — expiração de 1 minuto para recuperação de senha
- **TokenValidationMiddleware** — verifica blacklist do Redis em toda requisição autenticada
- **Roles** — controle de acesso por perfil (`Admin`, `User`)
- CORS configurado por origem explícita
- Validação de entrada em todas as rotas via FluentValidation
- Erros internos nunca expostos ao cliente — tratados pelo `GlobalExceptionHandler`

---

## Licença

MIT

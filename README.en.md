🌐 [Versão em Português](README.md)

# NexusPay 💳

> High-performance payment platform built with a service-oriented architecture, gRPC communication, JWT authentication, and Redis session management.

---

## Why gRPC?

Communication between the REST API and internal services uses **gRPC** instead of conventional REST for deliberate technical reasons:

- **Performance** — gRPC uses HTTP/2 with stream multiplexing and binary serialization via Protocol Buffers, up to 10x faster than JSON/REST for internal communication
- **Strong contract** — `.proto` files define a typed and versionable contract between services, eliminating ambiguities
- **Code generation** — clients and servers are automatically generated from `.proto`, zero manual integration code
- **Native streaming** — built-in support for bidirectional streaming for real-time operations

---

## Architecture

```
HTTP Client
     │
     ▼
┌─────────────────────┐
│   NexusPay.Api      │  ASP.NET Core Minimal APIs + JWT Auth
│   (REST :5000)      │  + TokenValidationMiddleware (Redis)
└────────┬────────────┘
         │ gRPC (HTTP/2 + Protobuf)
         ▼
┌─────────────────────┐
│  NexusPay.Server    │  Internal gRPC Services
│  (gRPC :7199)       │  + MailKit (SMTP)
└────────┬────────────┘
         │ ADO.NET + Stored Procedures     Redis (Sessions + Reset Tokens)
         ▼                                      │
┌─────────────────────┐             ┌───────────┴──────────┐
│   SQL Server        │             │   Redis              │
│   Docker (:1433)    │             │   Docker (:6379)     │
└─────────────────────┘             └──────────────────────┘

┌─────────────────────┐
│   Nginx             │  Static HTML Templates
│   Docker (:5500)    │  (reset-password.html)
└─────────────────────┘
```

### Projects

| Project | Responsibility |
|---|---|
| `NexusPay.Api` | REST API — endpoints, authentication, validation |
| `NexusPay.Client` | gRPC clients consumed by the API |
| `NexusPay.Contracts` | Shared Protobuf contracts (`.proto`) |
| `NexusPay.Server` | gRPC services — business logic + email sending |
| `NexusPay.Data` | Data access via ADO.NET + Stored Procedures |
| `NexusPay.Shared` | Shared models, helpers and extensions |
| `NexusPay.Templates` | Static HTML templates served via Nginx |

---

## Stack

- **.NET 10** — Runtime and framework
- **ASP.NET Core Minimal APIs** — Lean and performant REST endpoints
- **gRPC / Protocol Buffers** — Internal service communication
- **SQL Server 2022** — Relational database
- **ADO.NET** — Data access without ORM, via Stored Procedures
- **FluentValidation** — Declarative input validation
- **BCrypt.Net** — Secure password hashing
- **JWT Bearer** — Stateless authentication
- **Redis** — Session management, token blacklist and reset tokens
- **MailKit** — Transactional email sending via SMTP
- **Nginx** — Static file server for HTML templates
- **Scalar / OpenAPI** — Interactive endpoint documentation
- **Docker** — Full containerization (SQL Server, Redis, Nginx, CloudBeaver)
- **CloudBeaver** — Web-based database management interface

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/nexuspay.git
cd nexuspay
```

### 2. Start the containers

```bash
docker-compose up -d
```

This automatically starts:

| Container | Address |
|---|---|
| SQL Server | `localhost:1433` |
| Redis | `localhost:6379` |
| CloudBeaver | `http://localhost:8978` |
| Frontend (Nginx) | `http://localhost:5500` |

### 3. Configure the connection in CloudBeaver

Access `http://localhost:8978` and connect to SQL Server:

| Field | Value |
|---|---|
| Driver | SQL Server |
| Host | `sqlserver` |
| Port | `1433` |
| Database | `NexusPay` |
| Username | `sa` |
| Password | `NexusPay@123` |

### 4. Run the SQL scripts in order

Execute all files from the `Database/` folder in the following order:

1. `Tables/`
2. `StoredProcedures/`
3. `Seeds/` ← mandatory role seeds + admin user for development

### 5. Configure the environment

Update the `appsettings.json` for both `NexusPay.Api` and `NexusPay.Server`:

```json
{
  "ConnectionStrings": {
    "SQL": "Server=localhost,1433; Database=NexusPay; User Id=sa; Password=NexusPay@123; TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Issuer": "NexusPay",
    "Audience": "NexusPayUsers",
    "Key": "your-secret-key-at-least-32-characters",
    "ExpirationMinutes": 60
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "FromEmail": "your-email@gmail.com",
    "Password": "your-app-password",
    "FromName": "NexusPay"
  },
  "FrontendUrl": "http://localhost:5500"
}
```

> ⚠️ The JWT `Key` must be **identical** in both projects.  
> ⚠️ For Gmail, use an **App Password** — not your account password.

### 6. Run the projects

```bash
# Terminal 1 — gRPC Server
cd NexusPay.Server && dotnet run

# Terminal 2 — REST API
cd NexusPay.Api && dotnet run
```

Access the interactive API documentation at `http://localhost:5000/scalar/v1`

---

## Authentication & Session

The API uses **JWT Bearer Token** with session management via **Redis**. After login, include the token in all authenticated requests:

```
Authorization: Bearer {token}
```

### Session behavior

- **New login with active session** — the previous token is automatically invalidated. Only one active session per user is allowed
- **Logout** — immediately invalidates the token in Redis, regardless of expiration time
- **Revoked token** — any request with an invalid token returns `401 Unauthorized`

### Password recovery flow

1. `POST /auth/forgot-password` — sends an email with a reset link containing a token that expires in **1 minute**
2. User clicks the link and is redirected to the frontend (`http://localhost:5500/reset-password.html`)
3. `POST /auth/reset-password` — validates the token in Redis and updates the password with BCrypt

### Authentication endpoints

**Login:**
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@nexuspay.com",
  "password": "your-password"
}
```

**Response:**
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
  "email": "user@example.com"
}
```

**Reset Password:**
```http
POST /auth/reset-password
Content-Type: application/json

{
  "email": "user@example.com",
  "token": "token-received-by-email",
  "newPassword": "new-password"
}
```

**Logout:**
```http
GET /auth/logout
Authorization: Bearer {token}
```

### Default admin credentials (development)

| Field | Value |
|---|---|
| Email | `admin@nexuspay.com` |
| Password | `1234` |

---

## Role-based Authorization

| Role | Access |
|---|---|
| `Admin` | All endpoints, including user deletion |
| `User` | Own endpoints — login, logout, own profile update |

---

## Project Structure

```
NexusPay/
├── docker-compose.yml
├── nginx.conf                  ← template server configuration
├── Database/
│     ├── Tables/
│     │     ├── ROLES.sql
│     │     └── USERS.sql
│     ├── StoredProcedures/
│     └── Seeds/
│           ├── ROLES_SEEDS.sql   ← mandatory roles
│           └── ADMIN_USER.sql    ← admin user for development
└── NexusPay.Templates/
      └── reset-password.html
```

> Scripts versioned with `V{n}__` prefix — never edit an already committed script, always create a new version.

---

## Security

- Passwords stored with **BCrypt** (automatic hash + salt)
- JWT tokens with configurable expiration and `ClockSkew = Zero`
- **Single session per user** — new login automatically invalidates the previous session
- **Token blacklist in Redis** — immediate invalidation on logout, automatic TTL
- **Reset tokens in Redis** — 1-minute expiration for password recovery
- **TokenValidationMiddleware** — checks Redis blacklist on every authenticated request
- **Roles** — access control by profile (`Admin`, `User`)
- CORS configured with explicit origins
- Input validation on all routes via FluentValidation
- Internal errors never exposed to the client — handled by `GlobalExceptionHandler`

---

## License

MIT

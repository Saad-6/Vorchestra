# Vorchestra

A self-hosted, multi-tenant SaaS orchestration platform built on .NET 9. Vorchestra automates the full lifecycle of per-tenant service deployments on Linux servers — from provisioning to code deployment — using a trigger-driven workflow engine backed by SSH-executed bash scripts.

Each tenant runs as a fully isolated process with its own database. There is no shared-service multi-tenancy.

---

## Architecture

The platform is split into four independent microservices and a set of shared libraries.

```
services/
  vorchestra/   — Core orchestrator (Tenants, Servers, Plans, Projects)
  xcript/       — Script registry (Scripts, Variables, Groups)
  vbaton/       — SSH execution engine (runs scripts on servers)
Workflow.*      — Workflow service (trigger-based workflow definitions)
shared/         — Shared.Domain, Shared.Application, Shared.Infrastructure
Shared.Contracts/ — MassTransit message contracts
Shared.DTO/     — Shared DTOs passed across service boundaries
```

All services follow **clean architecture** with **CQRS** via MediatR. Services communicate asynchronously over **MassTransit + RabbitMQ**.

---

## Services

### Vorchestra — Port 5000

Core orchestrator. Manages the main domain entities.

| Entity | Description |
|---|---|
| `Tenant` | A customer. Has a slug, admin credentials, domain, and lifecycle status. |
| `Server` | A Linux server (IP, SSH credentials, resource capacity). |
| `Plan` | A subscription tier (price, RAM/storage/product limits). |
| `TenantSubscription` | Links a tenant to a plan with billing cycle and activation dates. |
| `Project` | A deployable codebase belonging to a tenant (source zip or git repo). |
| `TenantProject` | Assigns a project to a tenant with deployment status. |

**Tenant lifecycle:** `PENDING` → `FREE_TRIAL` / `SUBSCRIBED` → `SUSPENDED`

**Server status:** `ACTIVE` | `INACTIVE` | `MAINTENANCE` | `DELETED` | `PAYMENT_FAILED`

**Billing cycles:** `MONTHLY` | `ANNUALLY`

---

### Xcript — Port 5001

Script and variable registry. Stores bash templates — no execution logic.

| Entity | Description |
|---|---|
| `Script` | A bash template with `{{placeholder}}` tokens in its content. |
| `Variable` | A named placeholder with a `source` string (e.g. `"Tenant.Slug"`). |
| `ScriptVariable` | Join: links a variable to a script. |
| `Group` | An ordered collection of scripts. |
| `ScriptGroup` | Join: links a script to a group with an `order` field. |

**Variable sources** are namespaced constant strings resolved at runtime by Vbaton:

```
Tenant.Id               Server.Id
Tenant.Name             Server.IpAddress
Tenant.Slug             Server.Port
Tenant.Port             Server.DefaultDirectory
Tenant.Domain           Server.Username
Tenant.ConnectionString Server.Password
Project.Path
```

`Project.Path` resolves to the source-control URL (if git) or `<defaultDir>/<filename>` (if zip upload).

---

### Vbaton — Port 5002

SSH execution engine. Accepts an `ExecutionRequest`, fetches scripts from Xcript, substitutes variables, and runs scripts on the target server via SSH (Renci.SshNet). Persists results to `ExecutionLog` and `ExecutionLogDetail`.

The endpoint can be called directly (admin provisioning) or triggered via MassTransit events.

**Variable resolution** is centralized in `VariableMap` — a single dictionary from source strings to lambdas over the execution request. Adding a new variable source requires one entry in that map and nowhere else.

---

### Workflow — Port TBD

Manages trigger-based workflow definitions. A workflow is a named, ordered set of Xcript groups that executes when a specific event fires.

| Entity | Description |
|---|---|
| `ProjectWorkflow` | A workflow scoped to a project, activated by a project-level trigger. |
| `ServerWorkflow` | A workflow scoped to a server, activated by a server-level trigger. |
| `WorkflowGroup` | Join: links an Xcript group to a workflow with an order. |

**Project triggers:**

```
Project.TenantSubscribed   — fires when a tenant activates a paid plan or trial
Project.TenantSuspended    — fires when a tenant is suspended
Project.TenantReactivated  — fires when a suspended tenant is re-activated
Project.CodeUpdated        — fires when a project's source code is updated
Project.Manual             — admin-triggered execution
```

**Server triggers:**

```
Server.Manual              — admin-triggered server provisioning
```

---

## Shared Libraries

| Library | Purpose |
|---|---|
| `Shared.Domain` | `BaseEntity` (Guid Id, DateTimeOffset CreatedAt/UpdatedAt), `ScriptVariableSource`, `WorkflowTrigger`, queue name constants |
| `Shared.Application` | `ResponseModel<T>`, `PaginatedResponseModel<T>`, `FilterModel`, `IHashService` |
| `Shared.Infrastructure` | `HashService`, shared DI extension |
| `Shared.Contracts` | MassTransit request/response contracts for inter-service messaging |
| `Shared.DTO` | `ExecutionRequestDto` and related DTOs shared between Vbaton and callers |

---

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 9 / C# |
| API | ASP.NET Core Web API |
| CQRS / Mediator | MediatR |
| ORM | EF Core |
| Database | PostgreSQL (one DB per service) |
| Messaging | MassTransit + RabbitMQ |
| SSH | Renci.SshNet |
| Authentication | JWT Bearer |
| Deployment target | Ubuntu Linux — nginx + systemd per tenant |
| Scripts | Bash only |

---

## Key Event Flows

### Tenant Subscription → Auto-Deploy
1. Admin calls `POST /api/Tenant/subscribe-plan` (or apply-free-trial)
2. Vorchestra emits a `TenantSubscribed` event
3. Vbaton consumes the event, resolves the tenant/server context, fetches the project's workflow from Workflow service, fetches the associated Xcript groups, runs scripts over SSH, writes `ExecutionLog`

### Server Provisioning (manual)
1. Admin registers a server via `POST /api/Server`
2. Admin creates a `ServerWorkflow` with trigger `Server.Manual` and assigns Xcript groups to it
3. Admin calls `POST /api/WorkFlow` on Vbaton with server credentials and the group/script IDs

### Code Update → Redeploy
1. Admin uploads a new project zip or updates the source reference
2. Vorchestra emits a `CodeUpdated` event
3. Vbaton finds the `ProjectWorkflow` with trigger `Project.CodeUpdated` and re-runs the deployment scripts

---

## Project Structure

```
Vorchestra.slnx
├── services/
│   ├── vorchestra/
│   │   ├── Vorchestra/                  — Domain (entities, constants)
│   │   ├── Vorchestra.Application/      — Commands, Queries, Interfaces
│   │   ├── Vorchestra.Infrastructure/   — EF Core, Services, Event Publishers
│   │   ├── Vorchestra.DTOs/
│   │   └── Vorchestra.API/              — Controllers, DI wiring
│   ├── xcript/
│   │   ├── Xcript.Domain/
│   │   ├── Xcript.Application/
│   │   ├── Xcript.Infrastructure/
│   │   ├── Xcript.DTOs/
│   │   └── Xcript.API/
│   └── vbaton/
│       ├── Vbaton.Domain/
│       ├── Vbaton.Application/          — ExecuteWorkFlowCommand, VariableMap
│       ├── Vbaton.Infrastructure/       — SshService, Event Handlers
│       ├── Vbaton.DTO/
│       └── Vbaton.API/
├── Workflow.Domain/
├── Workflow.Application/
├── Workflow.Infrastructure/
├── Workflow.DTO/
├── Workflow.API/
├── shared/
│   ├── Shared.Domain/
│   ├── Shared.Application/
│   └── Shared.Infrastructure/
├── Shared.Contracts/                    — MassTransit message types
└── Shared.DTO/                          — Cross-service DTOs
```

---

## API Reference

Each service exposes Swagger at `/swagger`. A detailed endpoint reference with TypeScript types and payload examples is in [`FRONTEND_GUIDE.md`](FRONTEND_GUIDE.md).

**Default ports:**

| Service | URL |
|---|---|
| Vorchestra | `http://localhost:5000` |
| Xcript | `http://localhost:5001` |
| Vbaton | `http://localhost:5002` |

All endpoints require `Authorization: Bearer <token>`.

---

## What Is Not Yet Built

| Item | Notes |
|---|---|
| Authentication service | JWT is validated but no token-issuing endpoint exists yet |
| `GET /api/WorkFlow` | No query endpoint on Vbaton — execution logs are write-only |
| MassTransit event wiring | `TenantSubscribed` → Vbaton consumer not yet connected end-to-end |
| vpay (Stripe billing) | Planned as a future service; currently deferred |
| Monitoring service | Heartbeat, restart-on-failure, resource tracking against plan limits |
| Multi-server load balancing | Server selection during tenant assignment is manual |
| Composite workflow groups | Groups containing other groups — flat only for now |

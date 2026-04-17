# Vorchestra — Frontend Developer Guide

This document is the complete reference for building the Next.js frontend for the Vorchestra platform. It covers the system architecture, all API endpoints with exact payloads, shared response structures, and valid constant values.

---

## 1. System Overview

Vorchestra is a self-hosted, multi-tenant SaaS orchestration platform. It automates the full lifecycle of per-tenant service deployments on Linux servers. Each tenant runs as a fully isolated process with its own database.

There are **three backend services**, each running independently:

| Service | Default Port | Purpose |
|---|---|---|
| **Vorchestra** | `5000` | Core orchestrator — manages Tenants, Servers, Plans, Subscriptions |
| **Xcript** | `5001` | Script registry — manages bash script templates, variables, and groups |
| **Vbaton** | `5002` | SSH execution engine — runs script groups on servers, records execution logs |

All services require **JWT Bearer authentication** in the `Authorization` header:
```
Authorization: Bearer <token>
```

---

## 2. Universal Response Structure

Every endpoint returns one of two response shapes. Content-Type is always `application/json`.

### Standard Response
```ts
interface ResponseModel<T> {
  success: boolean;
  message: string;
  data: T | null;
}
```

### Paginated Response
Extends `ResponseModel<T[]>` with pagination metadata:
```ts
interface PaginatedResponseModel<T> {
  success: boolean;
  message: string;
  data: T[] | null;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
```

**HTTP status codes:**
- `200 OK` — `success: true`
- `400 Bad Request` — `success: false` (validation or business rule failure, body contains the response model with message)

---

## 3. Valid Constant Values

These string constants are validated server-side. Use exactly these values (case-sensitive).

### TenantStatus
```
"PENDING"       — Tenant created, not yet onboarded
"ONBOARDED"     — Setup complete
"FREE_TRIAL"    — Active free trial
"SUBSCRIBED"    — Active paid subscription
"SUSPENDED"     — Subscription lapsed or manually suspended
```

### ServerStatus
```
"ACTIVE"         — Server online and accepting tenants
"INACTIVE"       — Server offline
"MAINTENANCE"    — Server under maintenance
"DELETED"        — Soft-deleted
"PAYMENT_FAILED" — Associated payment issue
```

### BillingCycle
```
"MONTHLY"
"ANNUALLY"
```

---

## 4. Vorchestra API — `http://localhost:5000`

Manages the core entities: Plans, Servers, Tenants.

---

### 4.1 Plans

#### `GET /api/Plan`
List all plans. Optionally filter by active status or name.

**Query parameters:**
```
isActive?  boolean   — filter by active flag
query?     string    — partial name search
```

**Response:** `ResponseModel<PlanViewDto[]>`

```ts
interface PlanViewDto {
  id: string;           // Guid
  name: string;
  slug: string;
  monthlyPrice: number;
  annualPrice: number;
  maxRam: number;       // GB
  maxStorageGb: number;
  maxProducts: number;
  isActive: boolean;
}
```

---

#### `POST /api/Plan`
Create a new plan.

**Body:** `application/json`
```ts
{
  name: string;
  slug: string;
  monthlyPrice: number;
  annualPrice: number;
  maxRam: number;
  maxStorageGb: number;
  maxProducts: number;
  isActive: boolean;
}
```

**Response:** `ResponseModel<string>` — `data` contains the new plan's `Guid`

---

#### `PUT /api/Plan`
Update an existing plan.

**Body:** `application/json`
```ts
{
  id: string;           // Guid — required
  name: string;
  slug: string;
  monthlyPrice: number;
  annualPrice: number;
  maxRam: number;
  maxStorageGb: number;
  maxProducts: number;
  isActive: boolean;
}
```

**Response:** `ResponseModel<string>` — `data` contains the plan's `Guid`

---

#### `DELETE /api/Plan/{planId}`
Delete a plan by ID.

**Route param:** `planId` — Guid

**Response:** `ResponseModel<string>`

---

### 4.2 Servers

#### `GET /api/Server`
Paginated list of servers. Optionally filter by status or name.

**Query parameters:**
```
pageNumber?   number   — default 1
pageSize?     number   — default 10
status?       string   — one of ServerStatus constants
name?         string   — partial name search
```

**Response:** `ResponseModel<ServerViewDto[]>` *(note: not PaginatedResponseModel — pagination metadata is not returned here)*

```ts
interface ServerViewDto {
  id: string;
  name: string;
  iPAddress: string;    // note: capital IP
  userName: string;
  port: number;
  status: string;       // ServerStatus constant
  defaultDirectory: string;
  totalRamGb: number;
  totalStorageGb: number;
  usedRamGb: number;
  usedStorageGb: number;
  coreCount: number;
}
```

---

#### `POST /api/Server`
Register a new server.

**Body:** `application/json`
```ts
{
  name: string;
  iPAddress: string;
  userName: string;
  password: string;
  port: number;
  status: string;          // ServerStatus constant
  defaultDirectory: string;
  totalRamGb: number;
  totalStorageGb: number;
  coreCount: number;
}
```

**Response:** `ResponseModel<string>` — `data` contains the new server's `Guid`

---

#### `PUT /api/Server`
Update an existing server.

**Body:** `application/json`
```ts
{
  id: string;              // Guid — required
  name: string;
  iPAddress: string;
  userName: string;
  password: string;
  port: number;
  status: string;          // ServerStatus constant
  defaultDirectory: string;
  totalRamGb: number;
  totalStorageGb: number;
  coreCount: number;
}
```

**Response:** `ResponseModel<string>` — `data` contains the server's `Guid`

---

#### `DELETE /api/Server/{serverId}`
Delete a server by ID.

**Route param:** `serverId` — Guid

**Response:** `ResponseModel<string>`

---

### 4.3 Tenants

#### `GET /api/Tenant`
Paginated list of tenants. Optionally filter by status or name.

**Query parameters:**
```
pageNumber?   number   — default 1
pageSize?     number   — default 10
status?       string   — one of TenantStatus constants
name?         string   — partial name search
```

**Response:** `ResponseModel<TenantViewDto[]>`

```ts
interface TenantViewDto {
  id: string;
  name: string;
  description: string;
  phoneNumber: string;
  adminEmail: string;
  businessEmail: string;
  domain: string;
  status: string;                   // TenantStatus constant
  suspendedAt: string | null;       // ISO DateTimeOffset
  suspensionReason: string;
  trialEndsAt: string | null;
  subscriptionStartDate: string | null;
  subscriptionEndDate: string | null;
  billingCycle: string;             // BillingCycle constant
  slug: string;
  identifier: string;
  isSetupComplete: boolean;
  onboardedAt: string | null;
  planId: string | null;            // Guid
  serverId: string | null;          // Guid
}
```

---

#### `POST /api/Tenant`
Create a new tenant record. Initial status will be `PENDING`.

**Body:** `application/json`
```ts
{
  name: string;
  description: string;
  phoneNumber: string;
  adminEmail: string;
  businessEmail: string;
  domain: string;
  slug: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the new tenant's `Guid`

---

#### `PUT /api/Tenant`
Update an existing tenant's details.

**Body:** `application/json`
```ts
{
  id: string;                     // Guid — required
  name: string;
  description: string;
  phoneNumber: string;
  adminEmail: string;
  businessEmail: string;
  domain: string;
  status: string;                 // TenantStatus constant
  suspendedAt?: string | null;    // ISO DateTime
  suspensionReason?: string | null;
  slug?: string | null;
  identifier?: string | null;
}
```

**Response:** `ResponseModel<string>` — `data` contains the tenant's `Guid`

---

#### `POST /api/Tenant/apply-free-trial`
Start a 30-day free trial for a tenant. Tenant status moves to `FREE_TRIAL`.

**Body:** `application/json`
```ts
{
  tenantId: string;   // Guid
}
```

**Response:** `ResponseModel<string>`

---

#### `POST /api/Tenant/subscribe-plan`
Assign a paid plan to a tenant. Tenant status moves to `SUBSCRIBED`. Optionally assigns a server.

**Body:** `application/json`
```ts
{
  tenantId: string;        // Guid
  planId: string;          // Guid
  serverId?: string | null; // Guid — optional server assignment
  billingCycle: string;    // "MONTHLY" | "ANNUALLY"
}
```

**Response:** `ResponseModel<string>`

---

#### `POST /api/Tenant/cancel-subscription`
Cancel a tenant's active subscription.

**Body:** `application/json`
```ts
{
  tenantId: string;   // Guid
}
```

**Response:** `ResponseModel<string>`

---

## 5. Xcript API — `http://localhost:5001`

Manages the script registry: Scripts, Variables, Groups.

**Concept summary:**
- A **Script** is a bash template with `{{variableName}}` placeholders in its content.
- A **Variable** has a `name` (the placeholder in the script) and a `source` (where the value comes from at runtime — e.g. `"Tenant.Slug"`, `"Server.IpAddress"`).
- A **Group** is an ordered collection of scripts. Groups are executed together by Vbaton.
- Variables are assigned to Scripts via the script-variable join. Groups contain Scripts via the group-script join with an `order` field.

---

### 5.1 Scripts

#### `GET /api/Script/{id}`
Get a single script by ID.

**Route param:** `id` — Guid

**Response:** `ResponseModel<ScriptViewDto>`

```ts
interface ScriptViewDto {
  id: string;
  name: string;
  description: string;
  content: string;     // bash script with {{placeholder}} tokens
  order: number | null; // only populated when returned as part of a group
}
```

---

#### `GET /api/Script`
Paginated list of scripts.

**Query parameters:**
```
pageNumber?   number   — default 1
pageSize?     number   — default 10
searchTerm?   string   — partial name search
```

**Response:** `PaginatedResponseModel<ScriptViewDto>`

---

#### `POST /api/Script`
Create a new script.

**Body:** `application/json`
```ts
{
  name: string;
  description: string;
  content: string;     // bash script body, use {{variableName}} for placeholders
}
```

**Response:** `ResponseModel<string>` — `data` contains the new script's `Guid`

---

#### `PUT /api/Script`
Update an existing script.

**Body:** `application/json`
```ts
{
  id: string;          // Guid — required
  name: string;
  description: string;
  content: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the script's `Guid`

---

#### `DELETE /api/Script`
Delete a script.

**Query parameters:**
```
id   string (Guid) — required
```

**Response:** `ResponseModel<string>`

---

#### `POST /api/Script/variables`
Assign a variable to a script (links the variable's placeholder to this script).

**Body:** `application/json`
```ts
{
  scriptId: string;    // Guid
  variableId: string;  // Guid
}
```

**Response:** `ResponseModel<string>`

---

#### `DELETE /api/Script/variables`
Remove a variable from a script.

**Query parameters:**
```
scriptId    string (Guid)
variableId  string (Guid)
```

**Response:** `ResponseModel<string>`

---

### 5.2 Variables

#### `GET /api/Variable/{id}`
Get a single variable by ID.

**Route param:** `id` — Guid

**Response:** `ResponseModel<VariableViewDto>`

```ts
interface VariableViewDto {
  id: string;
  name: string;        // placeholder name used in script content e.g. "slug"
  source: string;      // runtime source e.g. "Tenant.Slug", "Server.IpAddress"
  description: string;
}
```

---

#### `GET /api/Variable`
Paginated list of variables.

**Query parameters:**
```
pageNumber?   number   — default 1
pageSize?     number   — default 20
searchTerm?   string   — partial name search
```

**Response:** `PaginatedResponseModel<VariableViewDto>`

---

#### `GET /api/Variable/sources`
Returns all valid source strings for the `source` field of a variable. Use this to populate a dropdown in the variable creation form.

**Response:** `ResponseModel<string[]>`

Example data:
```json
["Tenant.Id", "Tenant.Name", "Tenant.Slug", "Tenant.Port", "Tenant.Domain",
 "Tenant.ConnectionString", "Server.Id", "Server.Name", "Server.IpAddress",
 "Server.Port", "Server.Username", "Server.Password"]
```

---

#### `POST /api/Variable`
Create a new variable.

**Body:** `application/json`
```ts
{
  name: string;        // the placeholder name (used in script as {{name}})
  source: string;      // must be one of the values from GET /api/Variable/sources
  description: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the new variable's `Guid`

---

#### `PUT /api/Variable`
Update an existing variable.

**Body:** `application/json`
```ts
{
  id: string;          // Guid — required
  name: string;
  source: string;
  description: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the variable's `Guid`

---

#### `DELETE /api/Variable`
Delete a variable.

**Query parameters:**
```
id   string (Guid) — required
```

**Response:** `ResponseModel<string>`

---

### 5.3 Groups

#### `GET /api/Group/{id}`
Get a single group by ID, including its ordered scripts.

**Route param:** `id` — Guid

**Response:** `ResponseModel<GroupViewDto>`

```ts
interface GroupViewDto {
  id: string;
  name: string;
  description: string;
  scripts: ScriptViewDto[];  // ordered by Order field ascending
}
```

---

#### `GET /api/Group`
Paginated list of groups.

**Query parameters:**
```
pageNumber?   number   — default 1
pageSize?     number   — default 10
searchTerm?   string   — partial name search
```

**Response:** `PaginatedResponseModel<GroupViewDto>`

---

#### `POST /api/Group`
Create a new group.

**Body:** `application/json`
```ts
{
  name: string;
  description: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the new group's `Guid`

---

#### `PUT /api/Group`
Update an existing group.

**Body:** `application/json`
```ts
{
  id: string;          // Guid — required
  name: string;
  description: string;
}
```

**Response:** `ResponseModel<string>` — `data` contains the group's `Guid`

---

#### `DELETE /api/Group`
Delete a group.

**Query parameters:**
```
groupId   string (Guid) — required
```

**Response:** `ResponseModel<string>`

---

#### `POST /api/Group/scripts`
Add a script to a group with a specific execution order.

**Body:** `application/json`
```ts
{
  scriptId: string;    // Guid
  groupId: string;     // Guid
  order: number;       // execution order within the group (1-based, ascending)
}
```

**Response:** `ResponseModel<string>`

---

#### `DELETE /api/Group/scripts`
Remove a script from a group.

**Query parameters:**
```
scriptId   string (Guid)
groupId    string (Guid)
```

**Response:** `ResponseModel<string>`

---

## 6. Vbaton API — `http://localhost:5002`

The SSH execution engine. Receives a fully resolved execution request, fetches the scripts from Xcript, substitutes variables, and runs the scripts on the target server via SSH.

This API is triggered either:
- **Directly** via HTTP (e.g. admin manually running a provisioning script group on a server)
- **Indirectly** via MassTransit (internal events from Vorchestra — not exposed to the frontend yet)

---

### `POST /api/WorkFlow`
Execute a script group (or explicit list of scripts) on a server.

**Body:** `application/json`
```ts
{
  server: {
    id: string;          // Guid — server ID for audit logging
    ipAddress: string;
    username: string;
    password: string;
    port: number;        // typically 22
  };
  tenant: {              // nullable — omit or set null for server provisioning runs
    id: string;
    name: string;
    slug: string;
    domain: string;
    connectionString: string;
    port: number;
  } | null;
  groupId?: string;      // Guid — provide this OR scriptIds, not both
  scriptIds?: string[];  // Guid[] — alternative to groupId
  variableContext: {     // key = variable Name (placeholder), value = resolved value
    [key: string]: string;
  };
}
```

**Note on `variableContext`:** The keys must match the `name` field of the Variables assigned to the scripts. For example, if a script has `{{slug}}` and the variable's name is `slug` with source `Tenant.Slug`, then the caller must provide `{ "slug": "acme-corp" }`.

**Response:** `ResponseModel<string>`

```ts
{
  success: boolean;
  message: string;    // e.g. "All scripts executed successfully" | "One or more scripts failed"
  data: string;       // combined stdout/stderr output of all scripts
}
```

Execution results are persisted to `ExecutionLog` and `ExecutionLogDetail` in the Vbaton database. Each script's exit code, stdout, and stderr are captured individually.

---

## 7. Key Workflows for the Frontend

### Workflow A — Create and configure a server
1. `POST /api/Server` — register the server record
2. In Xcript: create a script group containing server setup scripts (install .NET, nginx, etc.)
3. `POST /api/WorkFlow` — trigger execution with the server's credentials and the setup group ID

### Workflow B — Onboard a new tenant
1. `POST /api/Tenant` — create the tenant record
2. `POST /api/Tenant/apply-free-trial` or `POST /api/Tenant/subscribe-plan` — activate them
3. Deployment is triggered automatically via internal events (not yet wired — future work)

### Workflow C — Script authoring
1. `GET /api/Variable/sources` — fetch available sources for the dropdown
2. `POST /api/Variable` — create variables for each placeholder your script needs
3. `POST /api/Script` — create the script with `{{placeholderName}}` tokens in content
4. `POST /api/Script/variables` — link variables to the script
5. `POST /api/Group` — create a group
6. `POST /api/Group/scripts` — add scripts to the group in order

---

## 8. Suggested Frontend Architecture

**Stack:** Next.js 15 (App Router), TypeScript, Tailwind CSS, shadcn/ui, TanStack Query v5

**Suggested page structure:**
```
/app
  /servers           — list + create/edit server, trigger provisioning
  /plans             — list + create/edit plans
  /tenants           — list + create/edit tenants, apply trial/plan
  /scripts           — list + create/edit scripts, assign variables
  /variables         — list + create/edit variables
  /groups            — list + create/edit groups, manage script order
  /executions        — view execution logs (future — no GET endpoint yet)
```

**API client:** Generate a typed client from the Swagger docs served at each service's `/swagger` endpoint. Or maintain a hand-written client module per service (`/lib/api/vorchestra.ts`, `/lib/api/xcript.ts`, `/lib/api/vbaton.ts`).

**Environment variables:**
```env
NEXT_PUBLIC_VORCHESTRA_URL=http://localhost:5000
NEXT_PUBLIC_XCRIPT_URL=http://localhost:5001
NEXT_PUBLIC_VBATON_URL=http://localhost:5002
```

---

## 9. What Is Not Yet Built

| Item | Status |
|---|---|
| `GET /api/WorkFlow` — query execution logs | Not implemented (no query endpoint exists yet) |
| Authentication service / login endpoint | Not built — JWT is validated but no issuer service exists yet |
| `TenantSubscribed` event → auto-deploy | MassTransit consumer not yet wired |
| vpay (Stripe billing) | Deferred — planned as a future service |
| Monitoring service | Deferred — heartbeat, restart-on-failure |

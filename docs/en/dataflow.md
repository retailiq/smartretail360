# Data Flow

This document systematically describes the full data flow process under the multi-layer microservice architecture of SmartRetail360, including the general mechanism and the actual data flow paths and flowcharts (Mermaid) for three key business scenarios (login, data upload and report refresh, Copilot intelligent Q&A).

## 1. General Data Flow Mechanism

### 1.1 Request Entry
- Users initiate requests (e.g., login, upload, Q&A) through the frontend UI
- Requests pass through Cloudflare CDN (caching and security protection) and enter the API Gateway
- API Gateway performs:
  - JWT decoding (user identity verification)
  - Rate limiting and blacklist filtering
  - Attaching user attributes to request headers (user_id, role, tenant_id)
  - Routing requests to target services (.NET, NestJS Lambda)

### 1.2 Authentication and Permission Control
- The server validates identity via JWT
- Applies RBAC / ABAC models to control operation permissions (e.g., whether upload or resource access is allowed)
- Permission failures return an error response directly and are logged in the audit log

### 1.3 Service Processing Mechanism
- Microservices extract user context from JWT
- Interact with databases (PostgreSQL / Redis), cache, S3, Weaviate, etc.
- If subsequent processes are needed, publish Kafka events (decoupled processing)

### 1.4 Logging
- Behavioral logs are written to PostgreSQL (e.g., upload, export)
- System logs are written to Loki (for Grafana queries)
- Prometheus collects service expectations (latency, CPU, failure rate)

### 1.5 Asynchronous Triggering
- Kafka event streams connect microservices: ETL, recommendation, model training, etc.
- Services asynchronously consume events like file_uploaded, report_ready

### 1.6 Frontend Feedback Mechanism
- All APIs return a standard format:
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation successful"
}
```
- Error cases:
```json
{
  "success": false,
  "error": {
    "code": "PERMISSION_DENIED",
    "message": "You do not have permission to perform this operation"
  }
}
```
- Real-time updates are notified via WebSocket (Kafka-driven)

## 2. User Login

### 2.1 Sequence Diagram (Mermaid)
```mermaid
sequenceDiagram
    participant U as User
    participant FE as Frontend (Next.js)
    participant GW as API Gateway
    participant Auth as .NET Auth Service
    participant PG as PostgreSQL
    participant Log as Serilog + Loki

    U->>FE: Enter username and password, click login
    FE->>GW: POST /auth/login
    GW->>GW: Parse JWT / IP rate limiting (skipped)
    GW->>Auth: Forward request
    Auth->>PG: Query user, validate password
    Auth->>Log: Log ő behavior (success/failure)
    Auth-->>GW: Return JWT (with user_id, role, tenant_id)
    GW-->>FE: Return JWT + login success message
```

### 2.2 Flowchart
```mermaid
flowchart TD
    A[Copilot Input Box] --> B[API Gateway]
    B --> C[NestJS Lambda - Copilot]
    C --> D[LangChain Calls LLM]
    D --> E[Calls .NET Report Service]
    E --> F[PostgreSQL Queries Report Data]
    F --> G[Returns Chart Data to LangChain]
    G --> H[Writes to Redis Cache]
    G --> I[Writes to Loki Logs]
    G --> J[Returns Chart Data and Suggested Text to Frontend]
```

## 3. Data Upload

### 3.1 Sequence Diagram
```mermaid
sequenceDiagram
    participant U as User
    participant FE as Frontend Upload Page
    participant GW as API Gateway
    participant Upload as .NET File Service
    participant S3 as S3
    participant Kafka as Kafka
    participant ETL as ETL Pipeline (Lambda)
    participant PG as PostgreSQL
    participant Notify as WebSocket Push Service
    participant Log as Serilog + Loki

    U->>FE: Drag and drop CSV upload
    FE->>GW: POST /upload/orders
    GW->>Upload: Forward request + JWT validation
    Upload->>S3: Store raw file
    Upload->>PG: Write upload log
    Upload->>Kafka: Send file_uploaded
    Upload->>Log: Write upload success log
    Kafka-->>ETL: Trigger cleaning task
    ETL->>S3: Read + write to Silver / Gold
    ETL->>PG: Write report data
    ETL->>Kafka: Send report_ready
    Kafka-->>Notify: Notify frontend of data update
    Notify-->>FE: WebSocket refreshes dashboard in real-time
```

### 3.2 Flowchart
```mermaid
flowchart TD
    A[Copilot Input Box] --> B[API Gateway]
    B --> C[NestJS Lambda - Copilot]
    C --> D[LangChain Calls LLM]
    D --> E[Calls .NET Report Service]
    E --> F[PostgreSQL Queries Report Data]
    F --> G[Returns Chart Data to LangChain]
    G --> H[Writes to Redis Cache]
    G --> I[Writes to Loki Logs]
    G --> J[Returns Chart Data and Suggested Text to Frontend]
```

## 4. Copilot Chart Query

### 4.1 Sequence Diagram
```mermaid
sequenceDiagram
    participant U as User
    participant FE as Copilot UI
    participant GW as API Gateway
    participant Lambda as NestJS Copilot Service (GraphQL)
    participant LLM as LangChain + LLM
    participant DotNet as .NET Report Service
    participant PG as PostgreSQL
    participant Redis as Redis Cache
    participant Log as Serilog + Loki

    U->>FE: Ask "Last month's sales ranking"
    FE->>GW: POST /copilot/query
    GW->>Lambda: Forward GraphQL + JWT validation
    Lambda->>LLM: Parse semantic intent
    LLM->>DotNet: Call .NET report query
    DotNet->>PG: Query data
    DotNet-->>LLM: Return result data
    LLM->>Redis: Write Copilot cache
    LLM->>Log: Write Coppilot Q&A log
    LLM-->>FE: Return chart JSON + suggested text
```

### 4.2 Flowchart
```mermaid
flowchart TD
    A[Copilot Input Box] --> B[API Gateway]
    B --> C[NestJS Lambda - Copilot]
    C --> D[LangChain Calls LLM]
    D --> E[Calls .NET Report Service]
    E --> F[PostgreSQL Queries Report Data]
    F --> G[Returns Chart Data to LangChain]
    G --> H[Writes to Redis Cache]
    G --> I[Writes to Loki Logs]
    G --> J[Returns Chart Data and Suggested Text to Frontend]
```
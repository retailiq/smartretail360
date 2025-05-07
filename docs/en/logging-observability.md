# Logging and Observability

## I. Overview of Logging System

SmartRetail360 employs centralized log collection and structured logging strategies to support debugging, operations, monitoring, compliance auditing, and failure recovery.

| Type | Content | Example Technologies | Storage Target | Mandatory |
|------|---------|----------------------|----------------|-----------|
| Application Logs | All request entries, processing chains, exceptions | Serilog, Winston | Loki, CloudWatch | ✅ |
| Behavioral Logs | User behavior records (upload, download, prediction, etc.) | Serilog, GraphQL plugin | PostgreSQL | ✅ |
| System Logs | Service status, Pod lifecycle, container status | K8s + FluentBit | Loki | ✅ |
| Security Logs | Login, logout, abnormal IP | Serilog | PostgreSQL, Loki | ✅ |
| Audit Logs | Permission operations, sensitive data access | Serilog, Audit Middleware | PostgreSQL (read-only) | ✅ |

---

## II. Why Is the Logging System Critical?

- Microservice systems have long chains and complex asynchronous calls; a **unique tracking ID (trace_id)** enables full-chain traceability.
- Regulatory compliance (e.g., GDPR, CCPA) requires logging user-related behaviors to support **audit accountability**.
- Exception logs trigger **real-time alerting mechanisms** to reduce silent failures.
- Structured log formats support **Grafana visualization analysis** and metric aggregation.

---

## III. Log Collection and Implementation

### 3.1 Backend (.NET & NestJS)

| Environment | Technology Stack | Log Processing |
|-------------|------------------|---------------|
| .NET | Serilog + middleware | Automatically injects trace_id, user context, and API duration |
| NestJS | Winston + Interceptor | Encapsulates request lifecycle, unified JSON output |

### 3.2 Log Push

- In K8s, logs are pushed via FluentBit → Loki → Grafana for querying
- In VPS scenarios, logs are pushed via Filebeat → Loki
- All logs are structured in unified JSON format (including timestamp, trace_id, user, tenant)

---

## IV. Visualization and Monitoring

| Component | Function | Description |
|-----------|----------|-------------|
| Grafana | Log querying, trace views, dashboards | Supports trace_id aggregation, report export |
| Prometheus | Service metric collection (CPU, memory, API latency) | Integrated with Loki/Node Exporter |
| AlertManager | Alert notifications | Supports threshold alerts + Sentry exception event forwarding |
| Sentry | Exception monitoring | Integrated with frontend JS, NestJS, .NET |

---

## V. Exception Monitoring Mechanism

- All API exceptions are uniformly handled (NestJS Filter / ASP.NET Middleware)
- Captured exceptions are pushed to Sentry: including stack trace, trace_id, user information
- Configurable exception type whitelist to avoid alert flooding
- AlertManager configured for multi-channel notifications (Email, Slack, DingTalk)

---

## VI. Audit Log Strategy

- Create `audit_logs` table in PostgreSQL
- All sensitive operations are injected with `user_id`, `tenant_id`, `action`, `timestamp` via middleware
- Direct SQL writes are prohibited; logs can only be written via server-side logic
- Supports frontend interface for visualized querying, CSV export, and role-based visibility control

---

## VII. Security Requirements

- All logs are desensitized before storage (e.g., phone numbers, emails)
- Log retention: 90 days (Loki) and 1 year (PostgreSQL audit)
- Use Vault to manage log service connection credentials
- Non-production environments must not send logs to the production Loki cluster

---

## VIII. Tools and Recommended Configurations

| Tool | Purpose | Description |
|------|---------|-------------|
| Serilog | .NET structured logging | Supports Sink to push to Loki, Postgres |
| Winston | Node/Nest logging library | JSON output + file storage |
| Loki | Log aggregation storage | High performance, K8s native |
| Grafana | Visualization analysis | Supports per-user/tenant dashboards |
| Prometheus | Metric collection | Includes Node Exporter, Blackbox |
| Sentry | Exception tracking | Unified exception platform for frontend and backend |
| AlertManager | Alert push | Email, Slack, DingTalk |
| FluentBit / Filebeat | Log collection | Compatible with multi-environment push to Loki or ELK |

---

## IX. Summary and Best Practices

- Generate a unique trace_id for each request, propagated through the full chain of logs
- Separate audit logs, exception logs, and system logs, storing and alerting accordingly
- Standardize all logs in JSON format for easy integration with Grafana/Prometheus
- Configure permissions so the frontend log interface only displays records related to the current tenant/user
- Ensure logs are tamper-proof, audit logs are read-only, and sensitive information is desensitized, compliant with CCPA/GDPR
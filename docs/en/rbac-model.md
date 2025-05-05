# 🔐 RBAC Permission Model

SmartRetail360 implements a flexible Role-Based Access Control (RBAC) system with fine-grained permissions and multi-tenant support.

## 👤 Role Structure

- A user can have multiple roles (many-to-many)
- Roles are associated with a set of permissions
- Permissions are bound to resource types for fine control

## 🧩 Permission Groups

Predefined permission templates:

- **Admin**: Full system access
- **Analyst**: Dashboard and reporting only
- **Marketing**: Access to behavioral data and recommendation tools

## 🏢 Multi-Tenant Isolation

Uses `tenant_id` to isolate data:

- Shared schema (logical isolation)
- Separate dashboards and reports per tenant

## 🛡 Audit & Security

- All sensitive actions are logged in audit trails
- JWT tokens contain role ID and tenant ID decoded by middleware
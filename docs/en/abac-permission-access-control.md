# ABAC Permission Access Control

This document systematically explains how to implement secure access control based on the ABAC (Attribute-Based Access Control) model, combined with typical user roles and permission actions.

---

## 1. ABAC Definition

ABAC is an attribute-based access control model. Permission decisions are based on the following four types of attributes:

| Attribute Type | Example Content |
|----------------|-----------------|
| User Attributes (Subject) | User ID, Role, Tenant |
| Resource Attributes (Resource) | Tenant, Resource Creator, Resource ID |
| Environment Attributes (Environment) | Request Time, Device Type, etc. |
| Action Attributes (Action) | Operations such as read, upload, delete, train, etc. |

---

## 2. Architecture Flow

```
Frontend Request → Backend Parses Attributes (User + Resource + Environment + Action) → Policy Table Matching → Expression Evaluation → Return Result
```

---

## 3. Database Table Design

### 3.1 User Table

```sql
CREATE TABLE users (
  id UUID PRIMARY KEY,
  username TEXT,
  role TEXT,
  tenant_id UUID
);
```

### 3.2 Resource Table (Can be a unified resource_registry or specific business tables)

```sql
CREATE TABLE resource_registry (
  id UUID PRIMARY KEY,
  resource_type TEXT,
  source_id UUID,
  tenant_id UUID,
  owner_id UUID,
  created_at TIMESTAMP
);
```

### 3.3 Policy Table

```sql
CREATE TABLE policies (
  id UUID PRIMARY KEY,
  resource_type TEXT,
  action TEXT,
  expression TEXT,
  effect TEXT CHECK (effect IN ('allow', 'deny')),
  enabled BOOLEAN DEFAULT true
);
```

### 3.4 Audit Log Table (Optional)

```sql
CREATE TABLE audit_logs (
  id UUID PRIMARY KEY,
  user_id UUID,
  resource_id UUID,
  action TEXT,
  success BOOLEAN,
  evaluated_at TIMESTAMP,
  details JSONB
);
```

---

## 4. User Roles and Permission Function Model

| Role English Name | Role Chinese Name | Available Function Description |
|-------------------|-------------------|-------------------------------|
| TenantAdmin       | Tenant Administrator | Manage users and roles within the tenant, upload sales data, trigger model training, view all analysis records, configure permission policies |
| MarketingUser     | Marketing Staff      | Upload sales data, view charts and conversion analysis, access prediction results, use AI Copilot for queries |
| MarketingAnalyst  | Marketing Analyst    | Create and analyze charts, perform funnel analysis, user segmentation, path analysis, use Copilot to build reports |
| DataAnalyst       | Data Analyst         | Data cleaning, review ETL output, build training datasets, analyze model performance |
| Viewer            | Viewer               | Browse authorized charts, reports, and dashboard content, cannot upload or edit any resources |
| SystemAdmin       | Platform System Administrator | Platform-side super permissions, cross-tenant viewing, policy configuration, audit logs, and system settings |

---

## 5. Expression Engine Recommendations

| Engine | Applicable Languages |
|--------|----------------------|
| SpEL | Java/.NET |
| JSON Logic | Node.js/Python |
| JEXL / Casbin | Multi-language Compatible |

---

## 6. Request Evaluation Process

1. Extract user information from JWT (user_id, role, tenant_id)
2. Obtain resource ID from path or parameters, query resource attributes
3. Set action behavior, e.g., `action = 'read'`
4. Query policies from the policy table matching resource_type + action
5. Inject attribute context and execute expression
6. Evaluate result to determine whether to allow or deny

---

## 7. Application Examples

### ✅ Example 1: User Uploads File

- Condition: Only MarketingUser and TenantAdmin can upload
- Policy Expression:
  ```spel
  user.role in ['MarketingUser', 'TenantAdmin'] && user.tenant_id == resource.tenant_id
  ```

---

### ✅ Example 2: View Self-Created Analysis Records

- Condition: Only the creator can view
- Policy Expression:
  ```spel
  user.id == resource.owner_id && user.tenant_id == resource.tenant_id
  ```

---

## 8. Frontend Integration Requirements

- All operation requests must include JWT
- Resource read requests must include resource ID
- Upload/sensitive operations are automatically parsed by the backend for resource_type + action → ABAC check
- Button visibility can be controlled based on user roles and policy APIs (optional)

---

## 9. ABAC Capability Summary

| Capability                | Implementation |
|---------------------------|----------------|
| Multi-tenant Data Isolation | `user.tenant_id == resource.tenant_id` |
| Precise Resource Ownership Validation | `user.id == resource.owner_id` |
| Action-level Granular Control | `action == 'upload' / 'read' / 'delete'` |
| Dynamic Policy Updates       | Policies stored in database, expressions can be dynamically maintained |
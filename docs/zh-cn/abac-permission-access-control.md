# ABAC 权限访问控制

本文件系统性说明如何基于 ABAC（Attribute-Based Access Control）模型进行安全访问控制，结合典型用户角色与权限动作。

---

## 一、ABAC 定义

ABAC 是基于属性的访问控制模型。权限决策基于以下四类属性：

| 属性类型 | 示例内容 |
|----------|----------|
| 用户属性（Subject） | 用户 ID、角色、所属租户 |
| 资源属性（Resource） | 所属租户、资源创建人、资源 ID |
| 环境属性（Environment） | 请求时间、设备类型等 |
| 操作属性（Action） | 操作行为，如 read、upload、delete、train 等 |

---

## 二、架构流程

```
前端请求 → 后端解析属性（User + Resource + Environment + Action）→ 策略表匹配 → 表达式判断 → 返回结果
```

---

## 三、数据表设计

### 3.1 用户表

```sql
CREATE TABLE users (
  id UUID PRIMARY KEY,
  username TEXT,
  role TEXT,
  tenant_id UUID
);
```

### 3.2 资源表（可为统一 resource_registry 或各类业务表）

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

### 3.3 策略表

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

### 3.4 审计日志表（可选）

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

## 四、用户角色与权限功能模型

| 角色英文名       | 中文角色名       | 可使用功能说明 |
|------------------|------------------|----------------|
| TenantAdmin      | 租户管理员       | 管理租户内用户与角色、上传销售数据、触发模型训练、查看全量分析记录、配置权限策略 |
| MarketingUser    | 市场人员         | 上传销售数据、查看图表与转化分析、访问预测结果、使用 AI Copilot 提问 |
| MarketingAnalyst | 市场分析师       | 创建与分析图表、执行漏斗分析、用户分群、路径分析、使用 Copilot 构建报表 |
| DataAnalyst      | 数据分析师       | 数据清洗、审核 ETL 输出、构建训练数据集、分析模型效果 |
| Viewer           | 浏览用户         | 浏览被授权的图表、报表和仪表盘内容，不能上传或编辑任何资源 |
| SystemAdmin      | 平台系统管理员   | 平台侧超级权限，跨租户查看、策略配置、审计日志与系统设置 |

---

## 五、表达式引擎建议

| 引擎 | 适用语言 |
|------|----------|
| SpEL | Java/.NET |
| JSON Logic | Node.js/Python |
| JEXL / Casbin | 多语言通用 |

---

## 六、请求判断流程

1. 提取 JWT 中用户信息（user_id, role, tenant_id）
2. 从路径或参数获取资源 ID，查询资源属性
3. 设置操作行为，如 `action = 'read'`
4. 从策略表中查找 resource_type + action 匹配策略
5. 注入属性上下文，执行表达式
6. 判断结果，决定是否放行

---

## 七、应用示例

### ✅ 示例 1：用户上传文件

- 条件：只有 MarketingUser 和 TenantAdmin 可以上传
- 策略表达式：
  ```spel
  user.role in ['MarketingUser', 'TenantAdmin'] && user.tenant_id == resource.tenant_id
  ```

---

### ✅ 示例 2：查看自己创建的分析记录

- 条件：仅允许创建人查看
- 策略表达式：
  ```spel
  user.id == resource.owner_id && user.tenant_id == resource.tenant_id
  ```

---

## 八、前端集成要求

- 所有操作请求需携带 JWT
- 读取资源类请求需携带资源 ID
- 上传/敏感操作后端自动解析 resource_type + action → ABAC 检查
- 可结合用户角色和策略接口控制按钮显示（非强制）

---

## 九、ABAC 能力总结

| 能力               | 实现方式 |
|--------------------|----------|
| 多租户数据隔离     | `user.tenant_id == resource.tenant_id` |
| 精准资源归属校验   | `user.id == resource.owner_id` |
| 动作级细粒度控制   | `action == 'upload' / 'read' / 'delete'` |
| 可热更新策略       | 策略存数据库，表达式可动态维护 |

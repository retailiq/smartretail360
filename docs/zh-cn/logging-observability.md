# 日志和可视化

## 一、日志系统概述

SmartRetail360 使用集中式日志采集和结构化日志策略以支持调试、运维、监控、合规审计及故障恢复。

| 类型 | 内容 | 示例技术 | 存储目标 | 是否强制 |
|------|------|----------|----------|----------|
| 应用日志 | 所有请求入口、处理链路、异常 | Serilog, Winston | Loki, CloudWatch | ✅ |
| 行为日志 | 用户行为记录（上传、下载、预测等） | Serilog, GraphQL plugin | PostgreSQL | ✅ |
| 系统日志 | 服务状态、Pod 生命周期、容器状态 | K8s + FluentBit | Loki | ✅ |
| 安全日志 | 登录、登出、异常 IP | Serilog | PostgreSQL, Loki | ✅ |
| 审计日志 | 权限操作、敏感数据访问 | Serilog, Audit Middleware | PostgreSQL (只读) | ✅ |

---

## 二、为什么日志系统至关重要？

- 微服务系统链路长、异步调用复杂，**唯一追踪 ID（track_id）** 可用于追溯全链路。
- 法规合规（如 GDPR、CCPA）要求记录用户相关行为，支持**审计追责**。
- 异常日志触发 **实时告警机制**，减少 silent failure。
- 结构化日志格式支持 **Grafana 可视化分析** 与指标聚合。

---

## 三、日志采集与实现方式

### 3.1 后端（.NET & NestJS）

| 环境 | 技术栈 | 日志处理 |
|------|--------|----------|
| .NET | Serilog + middleware | 自动注入 trace_id、用户上下文、接口耗时 |
| NestJS | Winston + Interceptor | 封装请求生命周期，统一输出 JSON |

### 3.2 日志推送

- K8s 中通过 FluentBit → Loki → Grafana 查询
- VPS 场景通过 Filebeat 推送日志 → Loki
- 所有日志结构统一使用 JSON 格式（含时间戳、trace_id、用户、租户）

---

## 四、可视化与监控

| 组件 | 功能 | 说明 |
|------|------|------|
| Grafana | 日志查询、追踪视图、仪表盘 | 支持 trace_id 聚合、报表导出 |
| Prometheus | 服务指标采集（CPU、内存、接口延迟） | Loki/Node Exporter 集成 |
| AlertManager | 告警通知 | 支持阈值告警 + Sentry 异常事件转发 |
| Sentry | 异常监控 | 接入前端 JS、NestJS、.NET |

---

## 五、异常监控机制

- 所有接口异常统一处理（NestJS Filter / ASP.NET Middleware）
- 异常捕获推送至 Sentry：包含堆栈、trace_id、用户信息
- 可配置异常类型白名单，避免告警泛滥
- AlertManager 配置多通道通知（Email、Slack、钉钉）

---

## 六、审计日志策略

- PostgreSQL 中建表 `audit_logs`
- 所有敏感操作通过中间件注入 `user_id`, `tenant_id`, `action`, `timestamp`
- 禁止直接写入 SQL，可仅通过服务端逻辑落库
- 支持前端界面可视化查询、导出 CSV、按角色权限控制可见范围

---

## 七、安全性要求

- 所有日志落库前自动脱敏（如手机号、邮箱）
- 日志保留 90 天（Loki）与 1 年（PostgreSQL 审计）
- 使用 Vault 管理日志服务连接凭证
- 非生产环境不得将日志发送至正式 Loki 集群

---

## 八、工具与推荐配置

| 工具 | 用途 | 说明 |
|------|------|------|
| Serilog | .NET 结构化日志 | 支持 Sink 推送 Loki、Postgres |
| Winston | Node/Nest 日志库 | JSON 输出 + 文件落地 |
| Loki | 日志聚合存储 | 高性能、K8s 原生 |
| Grafana | 可视化分析 | 支持分用户/租户仪表盘 |
| Prometheus | 指标采集 | 包括 Node Exporter, Blackbox |
| Sentry | 异常追踪 | 前后端统一异常平台 |
| AlertManager | 告警推送 | 邮件、Slack、钉钉 |
| FluentBit / Filebeat | 日志采集 | 兼容多环境推送 Loki 或 ELK |

---

## 九、总结与最佳实践

- 每个请求统一生成 trace_id，贯穿全链路日志
- 审计日志、异常日志、系统日志分离，分别入库/告警
- 所有日志结构标准化 JSON 格式，便于 Grafana/Prometheus 接入
- 配置权限，前端日志界面只展示当前租户/用户相关记录
- 日志不可篡改、审计只读、敏感信息脱敏，符合 CCPA/GDPR


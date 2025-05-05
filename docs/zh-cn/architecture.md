# 📐 系统架构概览

SmartRetail360 采用微服务 + 多语言技术栈，具备高可扩展性和工业级稳定性。

## 🧱 架构组成

- **前端**：Next.js + TailwindCSS，支持动态仪表盘与文件上传
- **主后端**：.NET 8 微服务，处理用户、权限、认证等核心业务
- **AI 模块**：NestJS（用于托管）+ Python（AI 逻辑），部署在 Lambda 上
- **数据工程**：Python + Spark，采用 Medallion 架构（Bronze/Silver/Gold）
- **数据科学**：训练 LightGBM 等模型用于推荐与预测
- **数据库**：
    - PostgreSQL：结构化数据仓库
    - MinIO：模拟 S3 数据湖
    - Neo4j：图数据库，用于社区发现与推荐优化
- **基础设施**：
    - Docker + Kubernetes 部署
    - Terraform 管理基础资源
    - GitHub Actions 实现 CI/CD 自动化部署

## 🔄 数据流简图

```mermaid
flowchart LR
  FE[📱 前端] -->|上传数据| API[🛠 .NET/NestJS API]
  API -->|调用| Lambda[⚙️ Lambda AI服务]
  API --> PG[(PostgreSQL)]
  API --> MinIO
  AI --> Neo4j[(Neo4j)]
  Data[📊 Data Pipeline] --> PG
  Data --> MinIO
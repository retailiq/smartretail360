# 📐 System Architecture Overview

SmartRetail360 follows a microservices-based, multi-language architecture designed for industrial-grade scalability and flexibility.

## 🧱 Components

- **Frontend**: Next.js + TailwindCSS with interactive dashboards and file upload support
- **Main Backend**: .NET 8 microservices handling core business logic (users, permissions, auth)
- **AI Module**: NestJS (for hosting) + Python (ML logic), deployed on AWS Lambda
- **Data Engineering**: Python + Spark following a Medallion Lakehouse (Bronze/Silver/Gold)
- **Data Science**: LightGBM-based predictive and recommendation models
- **Databases**:
    - PostgreSQL for structured analytics
    - MinIO as S3-compatible object storage
    - Neo4j for graph analytics and recommendation
- **Infrastructure**:
    - Docker + Kubernetes for container orchestration
    - Terraform for IaC
    - GitHub Actions for CI/CD automation

## 🔄 Data Flow (Mermaid)

```mermaid
flowchart LR
  FE[📱 Frontend] -->|Uploads CSV| API[🛠 .NET/NestJS API]
  API -->|Calls| Lambda[⚙️ AI Lambda Service]
  API --> PG[(PostgreSQL)]
  API --> MinIO
  AI --> Neo4j[(Neo4j)]
  Data[📊 Data Pipeline] --> PG
  Data --> MinIO
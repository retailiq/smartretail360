# SmartRetail360

📘 <a href="https://retailiq.github.io/smartretail360" target="_blank">Online Documentation (Docsify)</a>

SmartRetail360 is a cloud-native intelligent retail analytics and automation platform for small to mid-sized e-commerce businesses. It integrates full-stack development, data engineering, data science, and AI to deliver sales analysis, forecasting, personalized recommendations, and customer service automation.

## 🔧 Tech Stack Overview

- **Frontend**: Next.js, Tailwind CSS, Zustand, GraphQL
- **Backend**:
    - .NET 8: User management, file processing, analytics services
    - NestJS: AI Gateway, charting, recommender, Copilot chat
- **Data Engineering**: PySpark, Flink, Airflow, Delta Lake (MinIO)
- **Data Science**: LightGBM, Prophet, DoubleML, scikit-learn
- **AI**: LangChain, Mistral LLM, CLIP, Weaviate (RAG)
- **Databases & Messaging**:
    - PostgreSQL: Structured data storage
    - Redis: Caching
    - MinIO: Data lake storage (Bronze/Silver/Gold)
    - Kafka: Event streaming
    - Weaviate: Vector database for semantic search
- **Infrastructure**: Kubernetes, AWS Lambda, Terraform, Prometheus, Loki

## 🧩 System Architecture

- **Presentation Layer**: Next.js dashboard and embedded AI Copilot
- **Application Layer**: .NET and NestJS microservices
- **AI Layer**: LangChain + LLMs for recommendation, Q&A, summarization
- **Data Engineering Layer**: Ingestion, ETL, feature engineering
- **Storage Layer**: Hybrid storage across PostgreSQL, Redis, MinIO, Weaviate
- **Infrastructure Layer**: K8s + Serverless with monitoring & observability

## 🚀 Key Features

- Upload and validate CSV/Parquet sales data
- Auto ETL pipeline from Bronze → Silver → Gold (Delta Lake)
- Interactive dashboards and sales forecasting
- Customer segmentation and product conversion funnel
- AI Copilot for natural language queries and visual insights
- Intelligent customer service and content generation
- Semantic search and vector-based product discovery
- Multi-tenant support, RBAC, and audit logging
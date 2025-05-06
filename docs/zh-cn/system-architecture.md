# 系统架构

## 一、概述

SmartRetail360 是一款面向中小型电商平台的云原生智能运营平台，旨在提供销售数据分析、预测、用户分群、营销内容生成、自动化客服和个性化推荐等功能。系统整合了全栈开发（Next.js、.NET、NestJS）、数据工程（PySpark、Flink）、数据科学（LightGBM、Prophet）、人工智能（LLM、RAG）以及数据库管理（PostgreSQL、Redis、AWS S3、Weaviate、Kafka），支持高并发、多区域部署，符合 GDPR/CCPA 合规要求，具备企业级可扩展性和可维护性，达到 FAANG 级标准。

## 二、系统架构设计

### 2.1 总体架构
SmartRetail360 采用分层微服务架构，结合事件驱动（Kafka）和 API 驱动（REST/GraphQL）设计，部署于 AWS EC2（Kubernetes）+ AWS Lambda 基础设施。系统分为以下八个主要层：

1. **表示层 (Frontend Layer)**  
   - **功能**: 提供用户界面，包括首页概览仪表盘、AI Copilot 操作助手、智能搜索引擎、报表下载与导出和动态仪表盘定制，支持多语言和实时更新。  
   - **技术**: Next.js、TypeScript、Tailwind CSS、Recharts、Zustand、next-intl、Material UI、react-hook-form、zod、Floating UI、Shepherd.js、Framer Motion、Web Speech API、React Table、Plotly、react-dnd。  
   - **优化**: 采用 SSR/SSG 提升性能，WebSocket 实现实时数据推送，Cloudflare CDN 加速静态资源。

2. **应用层 (Application Layer)**  
   - **功能**: 处理用户注册/登录/注销、用户信息修改与密码重置、权限配置、MFA、销售数据导入、报表下载与导出、登录历史/活动记录查看、订阅与计费模块、AI Copilot 操作助手、推荐系统、动态仪表盘定制、多维度交叉分析、智能客服系统、营销内容自动生成和自助服务门户。  
   - **技术**: .NET 8 + ASP.NET Core、NestJS、REST/GraphQL、gRPC、Redis、Hangfire、Kafka、Serilog、FluentValidation、ASP.NET Identity、CsvHelper、DinkToPDF、Stripe SDK、AWS SES、Entity Framework Core、Polly。  
   - **微服务**:  
     - .NET 微服务: 用户注册/登录/注销、用户信息修改与密码重置、权限配置、MFA、销售数据导入、报表下载与导出、登录历史/活动记录查看、订阅与计费模块。  
     - NestJS 微服务: Data Gateway、推荐系统、多维度交叉分析、动态仪表盘定制、智能客服系统、AI Copilot 操作助手、营销内容自动生成。  
   - **API 网关**: AWS API Gateway，提供认证、限流和版本控制。

3. **AI 层 (AI Layer)**  
   - **功能**: 执行营销内容自动生成、智能客服系统、AI Copilot 操作助手和推荐系统任务。  
   - **技术**: Mistral + LoRA、LangChain、Weaviate（RAG）、CLIP、FastAPI、TensorFlow、Faiss、MLflow。  
   - **优化**: 模型量化（INT8）、Lambda 推理、Redis/Weaviate 缓存。

4. **数据科学层 (Data Science Layer)**  
   - **功能**: 执行销售预测、用户分群与价值评估和商品漏斗转化分析任务。  
   - **技术**: Prophet、LightGBM、KMeans、DBSCAN、DoubleML、SHAP、scikit-learn、PyCaret、pandas、matplotlib、H2O、Isolation Forest、Featuretools。  
   - **优化**: MLflow 跟踪模型，H2O AutoML 提升效率。

5. **数据工程层 (Data Engineering Layer)**  
   - **功能**: 数据摄取、清洗、转换和存储，支撑销售数据导入和数据清洗与建模流程。  
   - **技术**: PySpark、Flink、Delta Lake、Airflow、Dagster、Great Expectations、Apache Atlas、Polars、DuckDB、delta-rs、AWS S3 SDK。  
   - **流程**: Bronze → Silver → Gold 数据管道，支持增量更新和 Z-Order 索引。

6. **数据库层 (Storage Layer)**  
   - **功能**: 存储和管理关系型、缓存、对象、向量和事件数据。  
   - **技术**:  
     - PostgreSQL: 用户数据、审计日志、元数据（分区表、GIN 索引）。  
     - Redis: 缓存热销榜单和 Copilot 回答（集群模式、TTL）。  
     - AWS S3: 数据湖和文件存储（Z-Order、S3 Glacier）。  
     - Weaviate: 向量数据（HNSW、S3 备份）。  
     - Kafka: 事件流（多分区、Snappy 压缩）。  
   - **凭证管理**: Vault 动态轮换。

7. **基础设施层 (Infrastructure Layer)**  
   - **功能**: 云部署、容器编排和监控。  
   - **技术**: Kubernetes（Helm）、Terraform、Pulumi、Grafana、Prometheus、Loki、Cloudflare、AWS Lambda、Istio、LaunchDarkly、Jenkins、GitHub Actions、Docker、AWS ALB、AWS VPC。  
   - **优化**: 多区域 VPC、Istio 服务网格、LaunchDarkly 灰度发布。

8. **安全与合规层 (Security & Compliance Layer)**  
   - **功能**: 认证、授权、隐私保护和审计。  
   - **技术**: JWT + OAuth、RBAC、bcrypt、TLS、Vault、AWS Macie、Cloudflare WAF、AWS KMS、FluentValidation。  

### 2.2 架构图验证

![system-architecture](/Users/jinyuanzhang/IdeaProjects/smartretail360/docs/images/system-architecture.png)

### 2.3 数据流设计
1. **用户请求发起**:
   - 用户通过 HTTPS（或 WebSocket）发起请求，例如通过销售数据导入上传 CSV 文件或通过 AI Copilot 操作助手查询销售数据。
   - 请求经过 Cloudflare（CDN 加速和 WAF 防护），转发至 AWS API Gateway。
   - API Gateway 进行 JWT 验证、限流和路由，分发至 .NET 或 NestJS 服务。

2. **数据摄取与处理**:
   - 销售数据导入请求由 .NET 处理，文件存储至 AWS S3（Bronze 层）。
   - .NET 服务通过 Kafka 发布 `file.uploaded` 事件，触发数据工程层（PySpark/Flink）。
   - 数据工程层执行数据清洗与建模流程（Bronze → Silver → Gold），清洗、合并和特征工程后存储至 AWS S3（Delta Lake）。
   - 数据血缘通过 Apache Atlas 记录，Airflow/Dagster 调度任务。

3. **分析与 AI 处理**:
   - .NET 销售分析报表与图表展示从 Gold 层读取数据，生成报表并存储至 PostgreSQL。
   - NestJS Data Gateway 接收 AI Copilot 操作助手请求，调用 AI 层（LangChain）进行语义解析。
   - AI 层结合 Weaviate（RAG）检索上下文，生成响应或图表，缓存至 Redis。
   - 数据科学层（LightGBM、Prophet）执行销售预测任务，结果存储至 PostgreSQL 或 Redis。

4. **推荐与实时更新**:
   - NestJS 推荐系统从 Weaviate 获取向量数据，结合 TensorFlow/Faiss 生成推荐列表。
   - Kafka 触发推荐更新事件，NestJS 处理智能客服系统多轮对话并缓存至 Redis。
   - WebSocket 通过 Nginx 推送实时更新（如首页概览仪表盘数据、推荐结果）至前端。

5. **监控与反馈**:
   - 所有服务日志通过 Serilog 输出，存储至 Loki。
   - Prometheus 收集性能指标（如 API 延迟、模型推理时间），Grafana 提供可视化。
   - 异常触发 Alertmanager 告警，通知运维团队。

## 三、工业级最佳实践

### 3.1 可扩展性
- **水平扩展**: Kubernetes HPA 自动扩展 Pod，AWS Lambda 按需伸缩 AI 推理，Redis/Weaviate 集群支持高负载。
- **预计算**: Delta Lake 物化视图，热销榜单预聚合存储至 Redis。
- **实时性**: Flink 亚秒级延迟，Kafka 多分区支持高吞吐量。

### 3.2 高可用性
- **多区域部署**: AWS us-east-1、eu-west-1、ap-southeast-1，Cloudflare DNS 负载均衡。
- **负载均衡**: AWS ALB + Istio 管理流量，PostgreSQL 多节点读写分离。
- **故障恢复**: AWS S3 定时备份（Glacier 冷存储），Weaviate 向量数据定期备份。

### 3.3 高并发
- **负载均衡**: AWS ALB 分配流量，Istio 服务网格优化微服务通信。
- **缓存优化**: Redis 缓存热点数据（如热销榜单、Copilot 响应），TTL 控制过期。
- **异步处理**: Kafka 削峰填谷，处理 ETL、推荐和营销事件。
- **分布式计算**: PySpark/Flink 分布式 ETL，Kubernetes 动态调度。
- **限流与熔断**: AWS API Gateway 限流，Polly 实现熔断和重试。

### 3.4 安全性
- **认证与授权**: JWT + OAuth 验证用户，RBAC 控制权限，Vault 管理密钥。
- **数据保护**: bcrypt 哈希密码，TLS 加密传输，AWS KMS 加密存储，AWS Macie 隐私合规。
- **防护**: Cloudflare WAF 防 DDoS，FluentValidation 防注入。

### 3.5 监控与日志
- **可观测性**: Prometheus 监控指标（API 响应时间、模型性能），Grafana 可视化，Loki 存储日志。
- **告警**: Alertmanager 实时通知异常（如数据库延迟、模型漂移）。
- **审计**: PostgreSQL 存储用户行为日志，Serilog 结构化日志。

### 3.6 部署与 CI/CD
- **自动化**: Terraform/Pulumi 管理基础设施，Jenkins/GitHub Actions 实现 CI/CD。
- **灰度发布**: LaunchDarkly 支持 A/B 测试和滚动更新。
- **容器化**: Kubernetes + Helm 确保部署一致性，Docker 打包服务。

## 四、部署策略
- **基础设施**: AWS EC2（Kubernetes）托管核心服务，AWS Lambda 部署 AI 推理。
- **多区域**: 主区域 AWS us-east-1，备选 eu-west-1 和 ap-southeast-1。
- **负载均衡**: AWS ALB + Istio，Cloudflare CDN 加速。
- **备份**: AWS S3 Glacier 冷存储，PostgreSQL 和 Weaviate 定期备份至 S3。
- 
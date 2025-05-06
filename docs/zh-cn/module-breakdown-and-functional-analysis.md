# 模块拆分与功能分析

## 一、模块总览

SmartRetail360 是一款面向中小型电商的云原生智能运营平台，提供销售数据分析、预测、用户分群、营销内容生成、自动化客服和个性化推荐等功能。系统整合全栈开发、数据工程、数据科学、AI 和数据库管理，支持高并发、多区域部署，符合 GDPR/CCPA 合规标准。本文档详细拆分模块、子模块、功能及其技术实现，明确模块间通信机制和部署策略，遵循企业级最佳实践，确保模块化、可扩展性和可维护性。

| **模块类别** | **子模块/服务** | **核心职责** | **部署位置** |
| --- | --- | --- | --- |
| **前端** | 登录页、仪表盘、文件上传页、图表分析页、Copilot UI、推荐页、用户设置页、权限管理页、报表管理页、搜索页、引导页、订阅管理页、活动日志页、商品漏斗分析页 | 提供用户界面、数据可视化、文件上传、自然语言交互和客户面向功能 | AWS EC2 (Kubernetes + Nginx), Cloudflare CDN |
| **后端 (.NET)** | Auth 服务、用户管理服务、文件处理服务、报表导出服务、审计日志服务、订阅与计费服务、租户管理服务、通知服务 | 处理用户认证、授权、文件处理、日志记录、计费和租户隔离 | AWS EC2 (Kubernetes) |
| **数据网关 (NestJS)** | Copilot 接口、图表聚合、推荐服务、多模态处理 | 封装数据工程、数据科学和 AI 服务，提供统一 API 供前端调用 | AWS Lambda |
| **AI 模块** | RAG 知识问答、语义搜索、图像理解、自然语言响应 | 执行智能客服、推荐和内容生成的推理任务 | AWS Lambda |
| **数据工程** | 数据入湖、字段校验、数据清洗、归一化、表合并、特征工程、ETL 调度 | 将原始数据转化为可操作的结构化洞察 | AWS EC2 (Kubernetes) |
| **数据科学** | 用户分群、销售预测、异常检测、因果分析、模型输出生成 | 构建和部署机器学习模型以支持预测和分析任务 | AWS EC2 (Kubernetes) |
| **数据存储** | PostgreSQL, Redis, AWS S3, Weaviate, Kafka | 存储结构化、缓存、非结构化、向量和事件数据，PostgreSQL 作为 Data Warehouse 存储分报表 | AWS EC2 (Kubernetes) |
| **基础设施与安全** | Vault, Terraform, Cloudflare, Prometheus, Grafana, Loki, Istio, API 网关, Jenkins, Kubernetes | 管理凭证、编排、可观测性、网络、安全和容器化部署 | AWS EC2, Cloudflare, AWS Infra |

---

## 二、功能与模块映射

本节将 `features.md` 中列出的所有功能映射到相应模块、子模块和技术实现，覆盖所有前端页面、后端服务和支持组件，确保功能全面无遗漏。部分功能（自助服务门户、营销内容管理、多维度交叉分析、动态仪表盘定制、客服问答、营销内容生成）被归类为“后续开发功能”。

### 2.1 前端功能模块

前端模块包含所有用户交互界面，包括页面、交互组件和客户端逻辑，支持多语言、主题切换和响应式设计，使用 Material UI 和 Tailwind CSS 确保 UI 一致性和开发效率。以下是前端功能的完整清单。

| **功能名称** | **页面/组件** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **用户登录** | 登录页面 | Next.js, Material UI, Tailwind CSS, react-hook-form, zod | 通过邮箱/密码或 OAuth 认证用户（支持租户和普通用户），获取 JWT 令牌 |
| **仪表盘概览** | 首页仪表盘 | Next.js, Material UI, Recharts, Zustand, Tailwind CSS | 展示关键指标（销售额、订单数、客户数）、近期上传数据、模型状态和图表/报表快捷入口 |
| **文件上传** | 文件上传页面 | react-hook-form, zod, Next.js, Material UI, Tailwind CSS | 支持 CSV/JSON/Parquet 文件拖拽上传，包含字段校验 |
| **图表分析** | 图表分析页面 | Next.js, Material UI, Plotly, Recharts, React Table, Zustand | 渲染 8 类核心图表（总销量、品类排行、趋势图等），支持过滤和交互（如时间轴缩放、Hover 提示） |
| **AI Copilot 交互** | Copilot 浮动窗口 | Zustand, Floating UI, Next.js, Material UI, Web Speech API, CLIP | 支持自然语言/语音/图片输入，生成图表、KPI 或建议 |
| **推荐展示** | 推荐页面 | Next.js, Material UI, Zustand, Tailwind CSS | 展示个性化商品推荐列表，支持点击查看详情 |
| **用户分群** | 用户分群页面 | Next.js, Material UI, React Table, Tailwind CSS | 展示分群结果（高价值、流失风险用户），支持导出 ID 列表 |
| **报表导出** | 报表管理页面 | Next.js, Material UI, Tailwind CSS, DinkToPDF (客户端触发) | 导出图表/表格为 PNG、PDF、CSV，自动添加日期和版本号 |
| **语言与主题切换** | 用户设置页面 | next-intl, Material UI, Tailwind CSS, Next.js | 支持多语言（中文、英文、西班牙语等）和明暗主题切换 |
| **权限管理** | 权限管理页面 | Next.js, Material UI, React Table, Tailwind CSS | 管理员界面，用于配置角色、权限和租户设置 |
| **智能搜索** | 全局搜索栏 | Next.js, Material UI, Weaviate Client, Tailwind CSS | 支持关键词/语义搜索图表、报表和功能模块，包含自动补全 |
| **引导式 Onboarding** | 引导覆盖层 | Shepherd.js, Framer Motion, Material UI, Next.js | 为新用户提供交互式引导，优化单手操作 |
| **订阅管理** | 订阅页面 | Next.js, Material UI, Tailwind CSS, Stripe SDK | 查看和管理订阅计划、计费历史和配额 |
| **活动日志查看** | 审计日志页面 | Next.js, Material UI, React Table, Tailwind CSS | 展示用户活动日志（登录、上传、导出），支持筛选和导出 |
| **商品漏斗转化分析** | 漏斗分析页面 | Next.js, Material UI, Plotly, Tailwind CSS | 展示从浏览到成交的转化率，标记异常商品并提供优化建议 |
| **用户资料更新** | 用户设置页面 | Next.js, Material UI, react-hook-form, zod, Tailwind CSS | 允许用户更新个人资料（如姓名、邮箱）和密码 |
| **通知中心** | 通知中心页面 | Next.js, Material UI, Tailwind CSS, WebSocket | 显示系统通知（如上传成功、模型训练完成），支持实时更新 |
| **数据上传历史** | 数据上传历史页面 | Next.js, Material UI, React Table, Tailwind CSS | 查看历史上传记录，包括文件状态、时间和异常反馈 |
| **模型训练状态** | 模型状态页面 | Next.js, Material UI, Recharts, Tailwind CSS | 显示模型训练状态（进行中、已完成、最后更新时间） |
| **帮助中心** | 帮助中心页面 | Next.js, Material UI, Tailwind CSS | 提供 FAQ、使用指南和支持联系方式 |

**前端 API 方法、协议类型与部署方式**：

- **API 方法**：主要使用 `POST`（文件上传、用户操作）、`GET`（数据查询、导出）、`query/mutate`（GraphQL 图表和 Copilot）。
- **协议类型**：REST（命令式操作如上传、导出），GraphQL（动态查询如图表、推荐）。
- **部署方式与位置**：AWS EC2 (Kubernetes + Nginx) 托管动态页面，Cloudflare CDN 提供静态资源加速。

**前端最佳实践**：

- **模块化**：Material UI 和 Tailwind CSS 构建可复用组件（如 ChartCard、TableGrid），Zustand 管理状态。
- **性能**：Next.js 支持服务端渲染（SSR）和静态生成（SSG），Cloudflare CDN 缓存。
- **可访问性**：遵循 ARIA 标准，支持键盘导航和响应式设计。
- **国际化**：通过 next-intl 支持多语言，包含阿拉伯语 RTL 布局。
- **安全性**：Next.js 中间件防止 XSS/CSRF，zod 进行输入校验。

---

### 2.2 后续开发功能（前端）

以下前端功能计划在后续开发周期实现，暂未包含在 Release 1 中。

| **功能名称** | **页面/组件** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **自助服务门户** | 自助服务门户页面 | Next.js, Material UI, Tailwind CSS, React Table | 允许终端客户自助查询订单状态、退货进度和常见问题 |
| **营销内容管理** | 营销内容页面 | Next.js, Material UI, Tailwind CSS, react-hook-form | 创建、编辑和发布营销内容（商品标题、促销文案、社交媒体帖子） |
| **多维度交叉分析** | 交叉分析页面 | Next.js, Material UI, Plotly, React Table, Tailwind CSS | 支持按地域、品类、时间等多维度组合分析，生成动态 Pivot 表 |
| **动态仪表盘定制** | 仪表盘定制页面 | Next.js, Material UI, react-dnd, Tailwind CSS | 拖拽式定制仪表盘布局和控件 |

---

### 2.3 后端 (.NET) 功能模块

后端 (.NET) 模块划分为明确的子模块（服务），每个服务专注于特定领域，确保清晰的责任划分。用户注册/登录/注销功能明确归属 Auth 服务，与用户管理服务分离。以下是功能模块的完整清单。

| **功能名称** | **子模块（服务）** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **租户/用户注册、登录、注销** | Auth 服务 | ASP.NET Identity, JWT, bcrypt | 支持租户和普通用户注册（邮箱/密码、OAuth）、登录（含 MFA）、注销，生成/验证 JWT 令牌 |
| **用户信息管理** | 用户管理服务 | ASP.NET Identity, Entity Framework Core | 更新用户信息（姓名、邮箱）、密码重置、账户设置管理 |
| **权限控制** | 用户管理服务 | ASP.NET Policy, RBAC | 管理基于角色的访问控制（管理员、分析师、租户管理员），支持资源级和字段级权限 |
| **文件上传与解析** | 文件处理服务 | CsvHelper, AWS S3 SDK, IFormFile | 校验并解析 CSV/JSON/Parquet 文件，存储至 AWS S3，触发 ETL 管道 |
| **报表导出** | 报表导出服务 | DinkToPDF, CsvWriter | 生成并导出报表为 PNG、PDF、CSV，自动添加版本命名 |
| **审计日志** | 审计日志服务 | Serilog, PostgreSQL | 记录用户行为（登录、上传、导出）以满足合规和故障排查需求 |
| **订阅与计费** | 订阅与计费服务 | Stripe SDK, Hangfire | 管理订阅计划、配额和计费周期，支持多租户计费 |
| **租户管理** | 租户管理服务 | Entity Framework Core, PostgreSQL | 创建和管理租户账户，实现数据隔离和租户特定配置 |
| **系统通知** | 通知服务 | Hangfire, AWS SES | 通过邮件或应用内消息发送通知（如上传成功、模型训练完成） |
| **登录 IP 跟踪与异常拦截** | Auth 服务 | Serilog, PostgreSQL | 跟踪登录 IP，检测异常行为并执行安全策略 |

**后端 (.NET) API 方法、协议类型与部署方式**：

- **API 方法**：`POST`（认证、上传、操作）、`GET`（查询、导出）、`PUT`（更新）。
- **协议类型**：REST（命令式操作如认证、上传、导出）。
- **部署方式与位置**：AWS EC2 (Kubernetes) 托管微服务，负载均衡通过 Istio。

**后端 (.NET) 最佳实践**：

- **清晰划分**：Auth 服务独立处理认证，降低耦合；用户管理、文件处理等服务按领域划分。
- **可扩展性**：微服务架构，Kubernetes 编排支持水平扩展。
- **安全性**：JWT 认证，bcrypt 密码哈希，AWS KMS 加密。
- **可靠性**：API 幂等性，Polly 实现重试机制，Saga 模式处理分布式事务。
- **日志**：Serilog 结构化日志，集成 Loki 提升可观测性。
- **性能**：PostgreSQL 连接池，Redis 缓存热点数据。

---

### 2.4 数据网关 (NestJS) 功能模块

数据网关模块封装 AI、数据科学和分析服务，提供统一的 API 接口，支持前端动态查询和实时处理。

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **Copilot 查询处理** | Copilot 接口 | NestJS, LangChain, GraphQL, Weaviate | 解析自然语言查询，获取数据并生成可视化内容或建议 |
| **图表聚合** | 图表聚合 | NestJS, GraphQL, Recharts | 为仪表盘聚合数据，支持动态过滤和跨维度分析 |
| **推荐服务** | 推荐服务 | NestJS, Faiss, TensorFlow | 基于协同过滤和内容数据生成个性化商品推荐 |
| **多模态处理** | 多模态处理 | NestJS, CLIP, LangChain | 处理图片/文本输入，支持 Copilot 和搜索功能 |
| **语义搜索** | Copilot 接口 | NestJS, Weaviate, GraphQL | 使用向量嵌入支持图表、报表和功能的模糊搜索 |

**数据网关 API 方法、协议类型与部署方式**：

- **API 方法**：`query/mutate`（动态查询和操作）。
- **协议类型**：GraphQL（支持复杂查询和聚合）。
- **部署方式与位置**：AWS Lambda 无服务器部署，弹性扩展。

**数据网关最佳实践**：

- **灵活性**：GraphQL 支持动态查询，gRPC 用于内部微服务通信。
- **性能**：Redis 缓存热点数据，Weaviate HNSW 优化向量搜索。
- **可扩展性**：AWS Lambda 按需扩展。
- **安全性**：JWT 验证，API 网关限流，gRPC 使用 Mutual TLS。

---

### 2.5 后续开发功能（数据网关）

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **客服问答** | 客服问答 | NestJS, LangChain, Weaviate, GraphQL | 处理多轮客服查询，使用 RAG 检索知识库 |

---

### 2.6 AI 功能模块

AI 模块驱动智能功能，依赖大型语言模型和向量数据库。

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **RAG 知识问答** | RAG 知识问答 | LangChain, Weaviate, FastAPI | 使用检索增强生成（RAG）为客服和 Copilot 提供上下文答案 |
| **语义搜索** | 语义搜索 | Weaviate, FastAPI | 执行基于向量的文档、图表和知识库搜索 |
| **图像理解** | 图像理解 | CLIP, FastAPI, LangChain | 分析 Copilot 查询中的图片（如商品识别） |
| **自然语言响应** | 自然语言响应 | Mistral, LangChain, FastAPI | 为 Copilot 和客服生成类人响应 |

**AI API 方法、协议类型与部署方式**：

- **API 方法**：`POST`（推理请求）、`query`（GraphQL 查询）。
- **协议类型**：REST（推理任务），GraphQL（查询和响应）。
- **部署方式与位置**：AWS Lambda 按需推理。

**AI 最佳实践**：

- **优化**：模型量化（INT8）降低延迟，Redis 缓存推理结果。
- **可扩展性**：AWS Lambda 弹性扩展，Weaviate 存储向量数据。
- **准确性**：LoRA 微调，SHAP 提供解释，MLflow 跟踪模型。
- **安全性**：模型权重加密，API 端点 JWT 保护。

---

### 2.7 后续开发功能（AI）

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **营销内容生成** | 营销内容生成 | FastAPI, Mistral, LangChain | 生成商品标题、促销文案和社交媒体内容，支持 A/B 测试 |

---

### 2.8 数据工程功能模块

数据工程模块管理 ETL 管道，将原始数据转化为可操作的洞察。

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **数据入湖（Bronze）** | 数据入湖 | Airflow, Delta-rs, AWS S3 | 将原始 CSV/JSON/Parquet 文件摄入 Bronze 层 |
| **字段校验** | 字段校验 | Great Expectations, DuckDB | 校验数据类型、格式和完整性 |
| **数据清洗（Silver）** | 数据清洗 | PySpark, Polars, Flink | 去重、归一化和合并数据至 Silver 层 |
| **特征工程（Gold）** | 特征工程 | Featuretools, DuckDB, PySpark | 生成分析和建模特征（RFM、转化率） |
| **ETL 调度** | ETL 调度 | Airflow, Dagster | 编排批处理和流式 ETL 任务 |

**数据工程 API 方法、协议类型与部署方式**：

- **API 方法**：无直接 API，内部通过 Kafka 事件触发。
- **协议类型**：Kafka（事件驱动）。
- **部署方式与位置**：AWS EC2 (Kubernetes) 托管批处理任务。

**数据工程最佳实践**：

- **可靠性**：CDC 增量更新，Great Expectations 确保数据质量。
- **性能**：Z-Order 索引，Flink 低延迟流处理。
- **可扩展性**：PySpark 分布式处理，Kafka 多分区。
- **可追溯性**：Apache Atlas 数据血缘，Airflow 监控工作流。

---

### 2.9 数据科学功能模块

数据科学模块构建和部署机器学习模型，支持预测和分析任务。

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **销售预测** | 销售预测 | LightGBM, Prophet, SHAP | 预测 7-30 天的销量、退货率和品类趋势 |
| **用户分群** | 用户分群 | scikit-learn, KMeans, DBSCAN, PyCaret | 基于 RFM 和行为数据分群（高价值、流失风险等） |
| **异常检测** | 异常检测 | Isolation Forest, TensorFlow | 检测销量、退货或用户行为的异常，触发 Copilot 警报 |
| **因果分析** | 因果分析 | DoubleML, pandas, matplotlib | 分析促销对转化的因果影响 |
| **模型可解释性** | 模型可解释性 | SHAP, matplotlib | 生成特征重要性可视化，提升模型透明度 |

**数据科学 API 方法、协议类型与部署方式**：

- **API 方法**：`POST`（模型训练/推理）。
- **协议类型**：REST（触发训练任务）。
- **部署方式与位置**：AWS EC2 (Kubernetes) 托管批处理。

**数据科学最佳实践**：

- **可重现性**：MLflow 跟踪模型，H2O AutoML 实验。
- **准确性**：Prophet 和 LightGBM 组合建模，SHAP 解释。
- **可扩展性**：Kubernetes 批处理，Redis 缓存模型结果。
- **监控**：Grafana 监控模型性能，Alertmanager 警报。

---

### 2.10 数据存储功能模块

数据存储模块支持多种数据类型，PostgreSQL 作为 Data Warehouse 存储数据科学分报表，所有文件存储使用 AWS S3。

| **功能名称** | **子模块** | **技术栈** | **功能描述** | **优化措施** |
| --- | --- | --- | --- | --- |
| **结构化数据存储** | PostgreSQL | PostgreSQL 14, AWS Aurora | 存储用户信息、审计日志、元数据和数据科学分报表，分区表和 GIN 索引 | 分区表、读写分离、物化视图 |
| **缓存存储** | Redis | Redis 7, Cluster | 缓存热销榜单、Copilot 回答、推荐结果，设置 TTL | AOF+RDB 持久化、集群模式、TTL 设置 |
| **对象存储** | AWS S3 | AWS S3, Glacier | 存储数据湖（Bronze/Silver/Gold）、上传文件，支持冷热分离 | Z-Order 索引、版本控制、Glacier 冷存储 |
| **向量存储** | Weaviate | Weaviate, HNSW | 存储 Copilot 上下文、知识库和文案向量，支持语义搜索 | HNSW 索引、批量导入、S3 备份 |
| **事件流存储** | Kafka | Kafka, Confluent Platform | 存储 ETL、营销和推荐事件，支持多分区和 7 天保留 | 多分区、Snappy 压缩、多副本 |

**数据存储 API 方法、协议类型与部署方式**：

- **API 方法**：SQL（PostgreSQL 查询）、Key-Value（Redis）、S3 API（文件操作）。
- **协议类型**：SQL（结构化数据），REST（S3、Weaviate）。
- **部署方式与位置**：AWS EC2 (Kubernetes) 托管数据库，AWS S3 存储文件。

**数据存储最佳实践**：

- **性能**：分区表、索引优化、缓存热点数据。
- **可靠性**：多副本、冷热分离、S3 定时备份。
- **安全性**：Vault 管理凭证，动态轮换。
- **可扩展性**：Redis 集群、Kafka 多分区、Weaviate 分布式部署。

---

### 2.11 基础设施与安全功能模块

基础设施与安全模块确保系统部署、监控和安全合规，Kubernetes 用于容器化部署。

| **功能名称** | **子模块** | **技术栈** | **功能描述** |
| --- | --- | --- | --- |
| **凭证管理** | Vault | HashiCorp Vault | 存储和管理数据库/API 密钥，支持动态轮换 |
| **基础设施编排** | Terraform | Terraform, Pulumi | 一键部署云资源（Lambda、S3、API 网关） |
| **CDN 与防护** | Cloudflare | Cloudflare | 提供 CDN 缓存、WAF 防护，隐藏 VPS IP |
| **监控与告警** | Prometheus, Grafana, Loki | Prometheus, Grafana, Loki | 监控 API、模型、数据库性能，设置告警 |
| **服务网格** | Istio | Istio | 管理微服务通信、限流和服务发现 |
| **CI/CD** | Jenkins, LaunchDarkly | Jenkins, LaunchDarkly | 自动化构建、部署和灰度发布 |
| **容器化部署** | Kubernetes | Kubernetes, Helm | 容器化部署前端、后端、数据工程和数据科学模块，HPA 自动扩展 |

**基础设施与安全 API 方法、协议类型与部署方式**：

- **API 方法**：无直接 API，内部通过 Terraform/Jenkins 触发。
- **协议类型**：REST（Cloudflare、Vault API）。
- **部署方式与位置**：AWS EC2 (Kubernetes) 托管服务，Cloudflare 提供 CDN 和防护，AWS Infra 托管 Terraform。

**基础设施与安全最佳实践**：

- **自动化**：Terraform 基础设施即代码，Jenkins 自动化 CI/CD。
- **可观测性**：Prometheus 和 Grafana 全面监控，Loki 存储日志。
- **安全性**：Cloudflare WAF 防止攻击，Vault 管理敏感数据。
- **高可用性**：多区域部署，Istio 确保通信可靠性。

---

## 三、通信机制与接口规范

| **通信方式** | **使用场景** | **协议/接口类型** | **认证机制** | **使用理由** |
| --- | --- | --- | --- | --- |
| **HTTPS（API 网关）** | 前端调用数据网关/.NET 服务/导出服务 | REST / GraphQL | JWT + Vault | 标准化的安全协议，API 网关统一入口，适合前端与后端交互，易于扩展和监控 |
| **GraphQL** | 图表查询/推荐系统/Copilot/客服问答 | query / mutation | JWT + 角色解码 | 支持动态查询和复杂数据聚合，减少过量或不足的数据获取，适合仪表盘、推荐和 Copilot 等高交互场景 |
| **Kafka** | 文件上传完成 → ETL 入湖、营销发布、模型预测结果通知 | topic: file.uploaded 等 | 内部流控 ACL | 事件驱动架构，解耦服务，适合异步任务（如 ETL、通知），高吞吐量和可靠性 |
| **Redis** | Copilot 缓存/热销榜单/推荐结果 | Key-Value 查询 | Redis Cluster | 高性能缓存，适合低延迟、高频访问场景（如推荐、Copilot 响应） |
| **PostgreSQL** | 分群结果、预测结果、图表数据、仪表盘配置、审计日志 | SQL 查询/物化视图 | 用户/系统凭证 | 结构化数据存储，适合复杂查询和事务处理，PostgreSQL 作为 Data Warehouse 提供高性能分析 |
| **AWS S3** | 上传文件、ETL 临时数据存储、文档归档 | S3 API + presigned URL | IAM + STS | 高可用、低成本的对象存储，适合大规模文件存储和数据湖管理 |
| **Weaviate** | Copilot、客服语义问答、图像嵌入向量检索 | REST + GraphQL | Token 授权 | 向量数据库，高效支持语义搜索和 RAG，适合 Copilot 和知识库场景 |
| **gRPC（内部）** | NestJS 内部微服务通信（推荐、文案、图表模块） | ProtoBuf + gRPC | Mutual TLS | 高性能、低延迟，适合内部微服务间复杂通信，Protobuf 提供强类型契约 |

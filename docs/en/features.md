# Features Document

## 1. Release 1 Features

### 1.1 Basic Features

##### Overview
The platform provides basic functions such as user management, multi-language support, and permission configuration to ensure a stable operating experience for multiple users and tenants.

##### Feature List
- User registration / login / logout
- OAuth login
- MFA: Supports two-factor authentication to enhance account security
- Login IP tracking and anomaly interception: Records login source IP and detects abnormal behavior
- System notification mechanism: Events like successful uploads, model training completion, and data export completion will trigger system prompts or email reminders
- Audit log: Records login, logout, data import/export, and report access behaviors, supporting export and query filtering
- User grouping: Supports backend grouping of users by organization, tags, or roles
- User information modification and password reset
- Multi-language interface support (e.g., Chinese-English switching)
- Theme switching (light / dark)
- User account permissions and role configuration (RBAC)
- Multi-tenant support: Tenant administrators can activate/manage users under their tenant
- User account deletion
- Login history / activity record viewing (audit log)
- Subscription and billing module: Supports multi-tenant billing plans, package quotas, and subscription management

### 1.2 Business Features

#### 1.2.1 Home Dashboard Overview

##### Overview 
The system homepage provides a unified data entry point, enabling users to quickly access key metrics and navigate to core functional modules.

##### Target Users  
All users

##### Features Involved  
- Summary of the latest uploaded data (quantity, time, status)
- Recommended chart shortcuts
- Model status prompt cards (training / completed / last updated time)
- Quick access to recently used charts and reports

---

#### 1.2.2 Sales Data Import

##### Overview

Supports uploading CSV, JSON, and Parquet files for sales record import. After uploading, the system automatically performs data cleaning, standardization, and modeling processes, writing to the data lake and structured analysis tables for subsequent prediction, analysis, and recommendation modules.

##### Target Users

Operators / Data administrators

##### Features Involved

- Drag-and-drop file upload
- Multi-format file parsing and field mapping validation
- Data storage in AWS S3
- ETL cleaning process (Bronze → Silver → Gold)
- Analysis table saved to Data Warehouse (PostgreSQL)
- Abnormal data alerts and status feedback
- Data permission isolation (filtered by tenant)
- System notification triggered upon successful upload (integrated with notification module)

---

#### 1.2.3 Sales Analysis Reports and Chart Display

##### Overview
After uploading sales data, users can view 8 core charts (total sales, year-over-year changes, top-selling categories, channel performance, etc.) on the `dashboard page` and export reports for presentations or internal use.

##### Target Users
Merchants / Analysts

##### 8 Core Reports
1. **Sales Overview Card** (sales amount, order count, customer count)
2. **Category Sales Ranking** (Top N category sales)
3. **Top-Selling Products List** (sorted by sales volume or amount)
4. **Return Rate Analysis** (trend of return order percentage)
5. **Regional Sales Heatmap** (sales amount by region)
6. **Sales Trend Chart** (hourly/daily/monthly trend line chart)
7. **Channel Comparison Chart** (sales share from Web, App, offline, etc.)
8. **Average Order Value and Repurchase Rate** (average order amount, repurchase customer percentage)

##### Features Involved
- Dashboard rendering (line charts, pie charts, bar charts, heatmaps)
- Real-time dashboard updates after analysis table generation
- Dynamic dashboard settings
- Report export (PNG, PDF, CSV)
- Interactive charts: Bar chart clicks link to other charts, timeline drag-and-zoom, hover to show key sales events

---

#### 1.2.4 Sales Forecasting

##### Overview
Users can view sales amount, return rate, and top-selling category forecasts for the next 7 to 30 days, along with the forecast basis, to adjust operations and inventory plans in advance.

##### Target Users
Analysts / Operations supervisors

##### Forecast Metrics
- Total sales forecast (daily/weekly)
- Category popularity forecast (Top N category trends)
- Return rate forecast (by product or category)
- Key product sales forecast (filterable by SKU)

##### Features Involved
- Prophet + LightGBM model fusion
- SHAP feature importance chart generation
- Visualized trend charts with forecast error prompts
- Export results as charts or tables
- Support for multi-version forecast viewing (including training time, accuracy, and used features)
- Customizable forecast period (7~90 days) and SKU granularity
- Automated training schedule prompts (showing last training time and estimated next run)

---

#### 1.2.5 Recommendation System

##### Overview

Based on user behavior and product attributes, the system generates personalized recommended product lists for in-platform recommendations, SMS pushes, or marketing campaigns.

##### Target Users

Merchants / Marketers / Copilot module

##### Features Involved

- Collaborative filtering + content-based recommendation model fusion
- Supports user input for Top-K recommendation queries
- Display and export of recommended products (ID, title, image)
- Integration with Copilot for unified recommendation card output
- Tracking of recommendation click-through and conversion rates (for model evaluation)
- Recommendation explainability (e.g., “Because you recently purchased X”)
- Integration with Copilot for automatic recommendation updates after filtering (model closed loop)

---

#### 1.2.6 User Segmentation and Value Assessment

##### Overview
Users can segment customer groups based on historical behavior data, such as high-value customers, inactive users, and potential churn users, and export tagged user lists for marketing campaigns.

##### Target Users
Marketers / CRM managers

##### Features Involved
- RFM model calculation + KMeans/DBSCAN clustering
- User distribution charts + customer tag descriptions
- Filterable and exportable customer list tables
- Multi-dimensional combined filtering (category, region, etc.)

---

#### 1.2.7 Product Funnel Conversion Analysis

##### Overview
Users can view conversion rates at each step from browsing to transaction (browse → click → add to cart → order → transaction), identify products with conversion drop-offs, and adjust strategies.

##### Target Users
Product operators / Analysts

##### Features Involved
- Multi-step conversion rate calculation (by product, category, time period)
- Funnel chart + detailed table display (exportable)
- Abnormal product flagging (e.g., add-to-cart → order conversion rate <5%)
- Suggestion prompts (e.g., page optimization, price adjustments)

---

#### 1.2.8 AI Copilot Assistant

##### Overview
Users can ask questions in natural language to quickly obtain charts, KPI data, trend analysis, and automated suggestions without needing SQL or chart configuration knowledge.

##### Target Users
All platform users

##### Example Interactions
- “What were last week’s sales?”
- “Generate a top-selling product ranking for the last 30 days”
- “Which products have low add-to-cart rates?”
- “Export a list of repurchasing users”

##### Features Involved
- GPT model semantic parsing + multi-turn dialogue support
- Automatic chart generation (line, bar, pie charts, etc.)
- Data export / email sending
- Chart embedding in pages / downloading / bookmarking
- Enhanced semantic understanding with multi-turn context memory (time range, filtering conditions, etc.)
- Operational suggestion responses (e.g., “Suggestions for improving conversion rates”)

---

#### 1.2.9 Intelligent Search Engine

##### Overview
Users can enter keywords or natural language to quickly navigate to desired charts, documents, or functional modules, supporting semantic fuzzy matching and multi-language.

##### Target Users
All platform users

##### Features Involved
- Weaviate vector indexing + text inverted indexing
- Functional chart anchor navigation
- Quick suggestion words and historical search recommendations
- Support for natural language fuzzy search expressions

---

#### 1.2.10 Report Management

##### Overview
Users can view historically generated reports, supporting batch downloading, renaming, and deletion operations.

##### Target Users
All users

##### Features Involved
- Chart screenshot export as PNG
- Table export as CSV / Excel
- Batch export
- Automatic file naming with date and version number

## 2. Future Development Features (Planned)

#### 2.1 Marketing Content Generation

##### Overview
Users can generate product titles, promotional copy, and social media content (e.g., tweets, WeChat Moments) and conduct A/B testing.

##### Target Users
Marketing operators

---

#### 2.2 Intelligent Customer Service System

##### Overview
Supports automated replies for logistics/orders/after-sales issues, integrated with WhatsApp, WeChat, and other platforms.

##### Target Users
Customers / Customer service operators

---

#### 2.3 Dynamic Dashboard Customization

##### Overview
Users can drag-and-drop to customize report layouts and chart combinations, saving personalized dashboard interfaces.

##### Target Users
Merchants / Analysts

---

#### 2.4 Multi-Dimensional Cross Analysis

##### Overview
Generate interactive Pivot analysis results by combining dimensions (e.g., region × time × category).

##### Target Users
Analysts

---

#### 2.5 Self-Service Portal

##### Overview
Supports end customers in viewing order status, return progress, etc., through the system without contacting human customer service.

##### Target Users
End customers
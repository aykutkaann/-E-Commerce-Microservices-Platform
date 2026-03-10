# 🛒 E-Commerce Microservices Platform (Deployed on AKS)

Production-ready microservices architecture built with **.NET 10**, deployed on **Azure Kubernetes Service**, fully automated with **GitHub Actions CI/CD**.

---

## 🏗️ Architecture

```
Internet → Azure Load Balancer
                ├── ProductService  (Port 80)
                ├── OrderService    (Port 80)
                └── UserService     (Port 80)
                          │
                    AKS Cluster
                    (2 nodes, 2 replicas each)
                          │
                    Azure SQL Database
                    (VNet isolated, subnet-db)
```

### Why Microservices?
Each service is **independently deployable**. Changing `OrderService` never touches `ProductService` — in code or in CI/CD.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | .NET 10 (Minimal API) |
| **Database** | Azure SQL + EF Core |
| **Containers** | Docker + ACR |
| **Orchestration** | Kubernetes (AKS) |
| **Infrastructure** | Terraform |
| **CI/CD** | GitHub Actions |

---

## 📁 Project Structure

```
ecommerce-platform/
├── src/
│   ├── ProductService/      # Product CRUD + stock management
│   ├── OrderService/        # Order processing + ProductService integration
│   └── UserService/         # User management + email uniqueness
├── k8s/
│   ├── secrets.yaml         # DB credentials (base64)
│   ├── product-deployment.yaml
│   ├── order-deployment.yaml
│   ├── user-deployment.yaml
│   └── *-service.yaml       # LoadBalancer services
├── infra/
│   ├── main.tf              # Provider + Resource Group
│   ├── vnet.tf              # VNet, Subnets, NSGs
│   ├── aks.tf               # AKS Cluster
│   ├── acr.tf               # Container Registry
│   ├── sql.tf               # Azure SQL Server + DB
│   └── variables.tf
└── .github/
    └── workflows/
        ├── product-service.yml
        ├── order-service.yml
        └── user-service.yml
```

---

## ⚡ Key Design Decisions

### 1. Path-based CI/CD Triggers
Each pipeline only runs when **its own service changes**.
```yaml
on:
  push:
    paths:
      - 'src/ProductService/**'   # Only this. Nothing else.
```
> Changing `UserService` never rebuilds `ProductService`. True microservice independence.

### 2. Network Isolation
```
subnet-aks  (10.0.1.0/24) → AKS nodes
subnet-db   (10.0.2.0/24) → Azure SQL (no public access)
```
SQL Server is **not internet-facing**. Only AKS subnet can reach it via VNet rules + Service Endpoints.

### 3. Zero-Downtime Deployments
```yaml
# CI/CD updates image tag with commit SHA
kubectl set image deployment/product-service \
  product-service=acr.io/product-service:${{ github.sha }}
```
Kubernetes rolls out new pods before terminating old ones. **No downtime on deploy.**

### 4. Secret Management
DB credentials never touch the codebase.
```
appsettings.json  →  empty
GitHub Secret     →  base64 encoded connection string
Kubernetes Secret →  injected as env variable at runtime
```

### 5. EF Core Auto-Migration
```csharp
// On every startup, pending migrations apply automatically
db.Database.Migrate();
```
No manual DB scripts. Schema follows the code.

---

## 🚀 Infrastructure Setup

### Prerequisites
- Azure CLI
- Terraform
- kubectl
- Docker

### Deploy Infrastructure
```bash
cd infra
terraform init
terraform apply -var="sql_admin_password=<YOUR_PASSWORD>"
```

**What Terraform provisions:**
- Resource Group
- VNet with 2 isolated subnets
- NSGs (HTTP/HTTPS inbound for AKS, SQL port only from AKS subnet)
- Azure Container Registry
- AKS Cluster (2x `Standard_D2s_v3` nodes)
- Azure SQL Server + Database
- Firewall rule for dev machine

---

## ☸️ Kubernetes Deployment

```bash
# Connect to AKS
az aks get-credentials --resource-group ecommerce-rg --name aks-ecommerce-dev

# Deploy
kubectl apply -f k8s/secrets.yaml
kubectl apply -f k8s/product-deployment.yaml
kubectl apply -f k8s/order-deployment.yaml
kubectl apply -f k8s/user-deployment.yaml

# Verify
kubectl get pods
kubectl get services   # Get EXTERNAL-IPs
```

### Health Checks
Every deployment has **liveness** and **readiness** probes.
```
livenessProbe  → Auto-restart on crash (self-healing)
readinessProbe → No traffic until fully ready
```

---

## 🔄 CI/CD Pipeline

```
git push → Build & Test → Docker Build → ACR Push → AKS Deploy
                │
                └── Fails here? Deploy never happens.
```

### Required GitHub Secrets
| Secret | Description |
|--------|-------------|
| `AZURE_CREDENTIALS` | Service Principal JSON |
| `ACR_LOGIN_SERVER` | `ecommerceacrdev.azurecr.io` |
| `ACR_USERNAME` | ACR username |
| `ACR_PASSWORD` | ACR password |
| `AKS_CLUSTER_NAME` | `aks-ecommerce-dev` |
| `AKS_RESOURCE_GROUP` | `ecommerce-rg` |

---

## 📡 API Endpoints

### ProductService
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List all products |
| GET | `/api/products/{id}` | Get product |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### OrderService
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | List all orders |
| GET | `/api/orders/{id}` | Get order |
| GET | `/api/orders/user/{userId}` | Orders by user |
| POST | `/api/orders` | Create order (validates stock) |
| PATCH | `/api/orders/{id}/status` | Update status |
| DELETE | `/api/orders/{id}` | Delete order |

### UserService
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | List all users |
| GET | `/api/users/{id}` | Get user |
| POST | `/api/users` | Create user (unique email) |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |

---

## 🔐 Security Highlights

- SQL Server has **no public endpoint** — VNet only
- Kubernetes Secrets for credentials — **never in code**
- NSG rules: DB subnet only accepts traffic **from AKS subnet**
- Each service runs with **resource limits** (CPU/memory caps)

---

## 📊 Resource Limits Per Pod

```yaml
resources:
  requests:
    cpu: 100m       # Guaranteed
    memory: 128Mi
  limits:
    cpu: 500m       # Maximum
    memory: 256Mi
```

---

## 🔜 Roadmap

- [ ] API Gateway / Ingress Controller (single domain)
- [ ] Azure Key Vault integration
- [ ] Distributed tracing (Application Insights)
- [ ] Azure Service Bus (async inter-service messaging)
- [ ] Unit & Integration tests
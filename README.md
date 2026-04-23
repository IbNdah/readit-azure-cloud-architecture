# 🚀 ReadIt - Azure Microservices Architecture

## 📌 Overview

ReadIt is a cloud-native microservices application demonstrating a production-like Azure architecture.

It showcases how to design, deploy and debug distributed systems using:

- Azure Kubernetes Service (AKS)
- Azure Container Registry (ACR)
- Azure Service Bus
- Terraform (Infrastructure as Code)

---

## 🏗️ Architecture

- Catalog Service (producer)
- Cart Service (consumer)
- Azure Service Bus (async messaging)

---

## 🔄 Flow

User → AKS → Catalog → Service Bus → Cart

---

## 📁 Structure

readit-azure-architecture/
- catalog-service/
- cart-service/
- terraform/
- kubernetes/

---

## 📚 Documentation

- [Architecture](docs/architecture.md)
- [Deployment Guide](docs/deployment.md)

---

## 💡 Key Insight

This project focuses on real-world debugging:

Cloud engineering is not about deployment —  
it’s about understanding failures in distributed systems.

# 🏗️ Architecture Details

## Components

- AKS (Kubernetes cluster)
- ACR (container registry)
- Service Bus (message broker)

## Communication

Catalog → sends message → Service Bus → Cart consumes

## Key Concepts

- Asynchronous communication
- Microservices isolation
- Event-driven architecture

# 🚀 Deployment Guide

## 1. Infrastructure

terraform init
terraform apply

---

## 2. Build & Push

docker build -t service:vX .
docker tag service:vX <acr>/service:vX
docker push <acr>/service:vX

---

## 3. Deploy

kubectl apply -f kubernetes/

---

## 4. Validate

kubectl get pods
kubectl logs <pod>

---

## Common Issues

ImagePullBackOff → wrong tag / ACR access  
CreateContainerConfigError → missing secret  
Service Bus 401 → wrong config usage
# 🚀 ReadIt - Azure Cloud Architecture

## 📌 Overview

**ReadIt** is a cloud-native microservices application simulating an online bookstore.

This project showcases a **production-like Azure architecture**, demonstrating how to design, deploy, and operate containerized applications using modern DevOps and cloud-native practices.

---

## 🌐 Live Demo

http://<your-public-ip>

---

## 🏗️ Architecture

This project implements a full end-to-end cloud architecture:

* Containerized **.NET application** (catalog-service)
* **Azure Container Registry (ACR)** for image management
* **Azure Kubernetes Service (AKS)** for orchestration
* **NGINX Ingress Controller** for routing
* **Azure Load Balancer** for external exposure
* **Terraform** for Infrastructure as Code (modular design)

---

## ⚙️ Tech Stack

* Docker
* Kubernetes (AKS)
* Azure Container Registry (ACR)
* Terraform (IaC)
* NGINX Ingress
* GitHub Actions (CI/CD ready)

---

## ✨ Key Features

* End-to-end cloud deployment pipeline
  *(Build → Push → Deploy → Expose)*

* Kubernetes rolling updates & zero-downtime deployment

* Rollback capability (versioned deployments)

* Modular Infrastructure as Code (Terraform)

* Public access via Ingress + Load Balancer

---

## 🔄 Deployment Flow

```text
Code → Docker → ACR → AKS → Ingress → Public Endpoint
```

---

## 📁 Project Structure

```text
readit-azure-architecture/
├── catalog-service/   # .NET application
├── terraform/         # Infrastructure as Code (modules + environments)
├── kubernetes/        # Kubernetes manifests (Deployment, Service, Ingress)
└── docs/              # Architecture documentation
```

---

## 🧠 Lessons Learned

This project involved real-world troubleshooting and cloud debugging:

* Resolving **ImagePullBackOff** and ACR authentication issues
* Fixing **container port mismatches (80 vs 8080)**
* Debugging **Ingress (502 / 404 errors)**
* Managing **Terraform state and environment isolation**
* Understanding **AKS ↔ ACR identity and permissions**

---

## 🚧 Roadmap

* [ ] Add additional microservices (cart, order, inventory)
* [ ] Implement full CI/CD pipeline (GitHub Actions)
* [ ] Enable HTTPS with cert-manager / Let's Encrypt
* [ ] Add monitoring (Azure Monitor / Prometheus / Grafana)
* [ ] Introduce API Gateway pattern

---

## 👤 Author

**Cloud / Azure Architecture Portfolio Project**

---

## 💡 Value Proposition

This project demonstrates:

* Real-world Azure architecture design
* Kubernetes production concepts
* Infrastructure as Code best practices
* End-to-end DevOps workflow

👉 Designed to showcase skills for **Cloud Engineer / Azure Architect roles**

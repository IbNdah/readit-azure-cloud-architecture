# ReadIt - Azure Cloud Architecture

## Overview

ReadIt is a cloud-native microservices application simulating an online bookstore.

This project demonstrates an end-to-end deployment of a containerized application on Azure using modern cloud practices.

## Live Demo

http://<your-public-ip>
---
## Architecture

* Containerized .NET application (catalog-service)
* Azure Container Registry (ACR) for image storage
* Azure Kubernetes Service (AKS) for orchestration
* NGINX Ingress Controller for routing
* Azure LoadBalancer for public access
* Infrastructure provisioned using Terraform

## Tech Stack

* Docker
* Kubernetes (AKS)
* Azure Container Registry (ACR)
* Terraform (Infrastructure as Code)
* NGINX Ingress

## Features

* End-to-end cloud deployment (build → push → deploy)
* Rolling updates using Kubernetes
* Rollback capability
* Infrastructure as Code with modular Terraform
---

## Deployment Flow

Code → Docker → ACR → AKS → Ingress → Public Endpoint

## Project Structure

readit-azure-architecture/
├── catalog-service/   # .NET application
├── terraform/         # Infrastructure as Code
├── kubernetes/        # Kubernetes manifests
└── docs/              # Architecture documentation

## Next Steps

* Add additional microservices (cart, order, inventory)
* Implement CI/CD pipeline (GitHub Actions)
* Add HTTPS with Let's Encrypt
* Improve monitoring and logging

## Author

Cloud / Azure Architecture Portfolio Project
---

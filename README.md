# ReadIt - Azure Cloud Architecture

## Overview

ReadIt is a cloud-native microservices application simulating an online bookstore.

This project demonstrates how to deploy applications on Azure using:

* Azure Kubernetes Service (AKS)
* Azure Container Registry (ACR)
* Terraform
* Docker

---

## Current Status

* Terraform infrastructure structure created
* AKS and ACR modules defined
* Project structure initialized
* catalog-service integration in progress

---

## Project Structure

* catalog-service → application code
* terraform → infrastructure as code
* kubernetes → deployment manifests
* docs → architecture documentation

---

## Goal

Build a production-like Azure architecture demonstrating:

* microservices deployment on AKS
* infrastructure provisioning with Terraform
* scalable and secure cloud design

---

## Next Steps

* Dockerize catalog-service
* Deploy to AKS
* Expose service
* Add more services (cart, order, inventory)

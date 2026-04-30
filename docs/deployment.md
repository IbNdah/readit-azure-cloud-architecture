# 🚀 Deployment

---

## Terraform/Infrastucture

terraform init  
terraform apply  

---

## Build & Push

docker build -t service:vX .
docker tag service:vX <acr>/service:vX
docker push <acr>/service:vX

---

## Deploy

kubectl apply -f kubernetes/

---

## Update

kubectl set image deployment/<name> <container>=<acr>/service:vX

---

## Debug

kubectl get pods  
kubectl logs -f <pod>
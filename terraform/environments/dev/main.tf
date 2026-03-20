#  Resource Group
module "rg" {
  source   = "../../modules/resource-group"
  name     = "rg-readit"
  location = var.location
}

#  ACR
module "acr" {
  source              = "../../modules/acr"
  name                = "readitacr123"
  location            = var.location
  resource_group_name = module.rg.name
}

#  AKS
module "aks" {
  source              = "../../modules/aks"
  name                = "readit-aks"
  location            = var.location
  resource_group_name = module.rg.name
  dns_prefix          = "readit"

  acr_id = module.acr.id
}
module "resource_group" {

  source   = "../../modules/resource-group"
  name     = "rg-readit-dev"
  location = "westeurope"

}

module "log_analytics" {

  source         = "../../modules/log-analytics"
  name           = "log-readit-dev"
  location       = "westeurope"
  resource_group = module.resource_group.name

}

module "networking" {

  source         = "../../modules/networking"
  name           = "vnet-readit-dev"
  location       = "westeurope"
  resource_group = module.resource_group.name

}

module "acr" {

  source         = "../../modules/acr"
  name           = "acrreaditdev123"
  location       = "westeurope"
  resource_group = module.resource_group.name

}

module "aks" {

  source         = "../../modules/aks"
  name           = "aks-readit-dev"
  location       = "westeurope"
  resource_group = module.resource_group.name

}
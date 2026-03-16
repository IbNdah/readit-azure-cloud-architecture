## VNET
resource "azurerm_virtual_network" "vnet" {

  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group

  address_space = ["10.0.0.0/16"]

}

## AKS 
resource "azurerm_subnet" "aks" {

  name                 = "aks-subnet"
  resource_group_name  = var.resource_group
  virtual_network_name = azurerm_virtual_network.vnet.name

  address_prefixes = ["10.0.1.0/24"]

}
## App Service
resource "azurerm_subnet" "appsvc" {

  name                 = "appsvc-subnet"
  resource_group_name  = var.resource_group
  virtual_network_name = azurerm_virtual_network.vnet.name

  address_prefixes = ["10.0.2.0/24"]

}
resource "azurerm_kubernetes_cluster" "aks" {

  name                = var.name
  location            = var.location
  resource_group_name = var.resource_group
  dns_prefix          = "readitaks"

  default_node_pool {

    name       = "default"
    node_count = 2
    vm_size    = "Standard_B2s"

  }

  identity {
    type = "SystemAssigned"
  }

}
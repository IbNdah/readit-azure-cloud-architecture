resource "azurerm_servicebus_namespace" "sb" {
  name                = var.namespace_name
  location            = var.location
  resource_group_name = var.resource_group_name
  sku                 = "Basic"
}

resource "azurerm_servicebus_queue" "queue" {
  name         = var.queue_name
  namespace_id = azurerm_servicebus_namespace.sb.id
}

resource "azurerm_servicebus_namespace_authorization_rule" "auth" {
  name         = "root"
  namespace_id = azurerm_servicebus_namespace.sb.id

  listen = true
  send   = true
  manage = true
}
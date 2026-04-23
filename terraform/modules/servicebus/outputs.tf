output "connection_string" {
  value = azurerm_servicebus_namespace_authorization_rule.auth.primary_connection_string
}

output "queue_name" {
  value = azurerm_servicebus_queue.queue.name
}
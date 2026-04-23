output "servicebus_connection_string" {
  value     = module.servicebus.connection_string
  sensitive = true
}

output "servicebus_queue_name" {
  value = module.servicebus.queue_name
}
output "vm_public_ip" {
  description = "Public IP of the VM"
  value       = azurerm_public_ip.vm.ip_address
}

output "vm_fqdn" {
  description = "FQDN of the VM"
  value       = azurerm_public_ip.vm.fqdn
}

output "pg_fqdn" {
  description = "FQDN of the PostgreSQL server"
  value       = azurerm_postgresql_flexible_server.main.fqdn
}

output "pg_connection_string" {
  description = "Connection string for the app"
  value       = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Port=5432;Database=voltei;Username=${var.admin_username};Password=${var.pg_admin_password};SSL Mode=Require;Trust Server Certificate=true"
  sensitive   = true
}

output "ssh_command" {
  description = "SSH command to connect to the VM"
  value       = "ssh ${var.admin_username}@${azurerm_public_ip.vm.ip_address}"
}

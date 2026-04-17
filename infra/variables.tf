variable "location" {
  description = "Azure region"
  default     = "eastus2"
}

variable "vm_size" {
  description = "VM size"
  default     = "Standard_B1s"
}

variable "pg_sku" {
  description = "PostgreSQL Flexible Server SKU"
  default     = "B_Standard_B1ms"
}

variable "pg_storage_mb" {
  description = "PostgreSQL storage in MB"
  default     = 32768 # 32GB — mínimo
}

variable "admin_username" {
  description = "Admin username for VM and DB"
  default     = "voltei"
}

variable "ssh_public_key_path" {
  description = "Path to SSH public key"
  default     = "~/.ssh/id_rsa.pub"
}

variable "pg_admin_password" {
  description = "PostgreSQL admin password"
  sensitive   = true
}

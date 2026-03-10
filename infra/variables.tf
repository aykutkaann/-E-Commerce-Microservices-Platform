variable "resource_group_name" {
  description = "The name of the resource group in which to create the resources."
  type        = string
  default     = "ecommerce-rg"
}

variable "location" {
  description = "The Azure region in which to create the resources."
  type        = string
  default     = "northeurope"
}

variable "environment" {
  description = "The environment in which to create the resources."
  type        = string
  default     = "dev"
}

variable "sql_admin_username" {
  description = "The administrator username for the Azure SQL Server."
  type        = string
  default     = "sqladmin"
}

variable "sql_admin_password" {
  description = "The administrator password for the Azure SQL Server."
  type        = string
  sensitive   = true
}

variable "dev_ip" {
  description = "Local developer machine IP (for migration)"
  type        = string
  default     = "81.214.102.106"  
}
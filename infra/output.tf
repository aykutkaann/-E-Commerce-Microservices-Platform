output "resource_group_name" {
  value = azurerm_resource_group.main.name
}

output "acr_login_server" {
  description = "ACR login URL — for Docker push "
  value       = azurerm_container_registry.main.login_server
}

output "acr_admin_username" {
  description = "ACR username"
  value       = azurerm_container_registry.main.admin_username
}

output "aks_cluster_name" {
  description = "AKS cluster name"
  value       = azurerm_kubernetes_cluster.main.name
}

output "sql_server_fqdn" {
  description = "SQL Server Connection string — for EF Core "
  value       = azurerm_mssql_server.main.fully_qualified_domain_name
}
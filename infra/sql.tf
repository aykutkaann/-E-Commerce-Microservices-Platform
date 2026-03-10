
#SQL server
resource "azurerm_mssql_server" "main" {
  name                         = "sql-ecommerce-${var.environment}"
  resource_group_name          = azurerm_resource_group.main.name
  location                     = azurerm_resource_group.main.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password

  # Ensure subnet and NSG association is created before SQL server
  depends_on = [
    azurerm_subnet_network_security_group_association.db
  ]

  tags = {
    Environment = var.environment
  }
}


# SQL Database 
resource "azurerm_mssql_database" "main" {
  name      = "ecommerce-db"
  server_id = azurerm_mssql_server.main.id
  sku_name  = "Basic"

  depends_on = [azurerm_mssql_server.main]

  tags = {
    Environment = var.environment
  }
}

## VNet rules to allow AKS subnet to access SQL Server
resource "azurerm_mssql_virtual_network_rule" "aks" {
  name      = "allow-aks-subnet"
  server_id = azurerm_mssql_server.main.id
  subnet_id = azurerm_subnet.aks.id

  depends_on = [
    azurerm_mssql_server.main,
    azurerm_subnet.aks
  ]
}

resource "azurerm_mssql_firewall_rule" "dev_machine" {
  name             = "allow-dev-machine"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = var.dev_ip   
  end_ip_address   = var.dev_ip
}
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.0"
    }
  }
}

provider "azurerm" {
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}


#RG

resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    Project     = "ecommerce-platform"
    Environment = var.environment
    ManagedBy   = "Terraform"
  }
}


## ACR 

resource "azurerm_container_registry" "main" {
  name                = "ecommerceacr${var.environment}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = true


  tags = {
    Environment = var.environment
  }

  timeouts {
    create = "30m"
    update = "30m"
    delete = "30m"
  }

}


## AKS

resource "azurerm_kubernetes_cluster" "main" {
  name                = "aks-ecommerce-${var.environment}"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = "ecommerce-${var.environment}"

  default_node_pool {
    name           = "default"
    node_count     = 2
    vm_size        = "Standard_D2s_v3"
    vnet_subnet_id = azurerm_subnet.aks.id
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
    network_policy = "azure"
    service_cidr   = "10.1.0.0/16"
    dns_service_ip = "10.1.0.10"
  }


  depends_on = [
    azurerm_subnet_network_security_group_association.aks
  ]

  tags = {
    Environment = var.environment
  }
}


## AKS ACR assignment

resource "azurerm_role_assignment" "aks_acr" {
  principal_id                     = azurerm_kubernetes_cluster.main.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.main.id
  skip_service_principal_aad_check = true
}
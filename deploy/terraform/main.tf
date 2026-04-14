# [PHASE-4] Infrastructure as Code Baseline
# Multi-Cloud Orchestration (Aiven + Render)

terraform {
  required_providers {
    aiven = {
      source  = "aiven/aiven"
      version = ">= 4.0.0"
    }
    render = {
      source  = "render-oss/render"
      version = ">= 1.0.0"
    }
  }
}

# --- VARIABLES (Security Decoupling) ---

variable "aiven_project_name" {
  description = "Project name in Aiven Console"
  type        = string
}

variable "legacy_db_host" {
  description = "Public host for the legacy database (sistema2020)"
  type        = string
  default     = "PLACEHOLDER"
}

variable "legacy_db_user" {
  description = "Username for the legacy database"
  type        = string
  default     = "avnadmin"
}

variable "legacy_db_password" {
  description = "Sensitive password for the legacy database"
  type        = string
  sensitive   = true
}

# --- AIVEN CONFIGURATION (MySQL + Redis) ---

resource "aiven_mysql" "hospital_db" {
  project                 = var.aiven_project_name
  cloud_name              = "google-us-east4"
  plan                    = "business-4"
  service_name            = "mysql-hospital-prod"
  maintenance_window_dow  = "sunday"
  maintenance_window_time = "10:00:00"
  
  mysql_user_config {
    mysql_version = "8"
    public_access {
      mysql = true
    }
  }
}

resource "aiven_redis" "hospital_cache" {
  project      = var.aiven_project_name
  cloud_name   = "google-us-east4"
  plan         = "startup-4"
  service_name = "redis-hospital-cache"
}

# --- RENDER CONFIGURATION (WebAPI) ---

resource "render_web_service" "hospital_api" {
  name       = "sistema-sat-hospitalario-api"
  plan       = "starter"
  region     = "oregon"
  runtime    = "docker"
  
  env_vars = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "DeploymentSettings__Mode" = "Cloud"
    "ConnectionStrings__mysql-system"     = aiven_mysql.hospital_db.service_uri
    "ConnectionStrings__mysql-identity"   = aiven_mysql.hospital_db.service_uri
    
    # [SEC-005] Sensitive String Interpolation (Decoupled)
    "ConnectionStrings__LegacyConnection" = "Server=${var.legacy_db_host};Port=15848;Database=Sistema2020;Uid=${var.legacy_db_user};Pwd=${var.legacy_db_password};"
    
    "ConnectionStrings__Redis"            = aiven_redis.hospital_cache.service_uri
  }
}

# --- OUTPUTS (Sensitive Data Protection) ---

output "system_database_uri" {
  value     = aiven_mysql.hospital_db.service_uri
  sensitive = true
}

output "redis_cache_uri" {
  value     = aiven_redis.hospital_cache.service_uri
  sensitive = true
}

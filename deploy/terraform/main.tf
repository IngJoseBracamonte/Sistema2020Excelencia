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
    "ConnectionStrings__DefaultConnection" = aiven_mysql.hospital_db.service_uri
    "ConnectionStrings__Redis" = aiven_redis.hospital_cache.service_uri
  }
}

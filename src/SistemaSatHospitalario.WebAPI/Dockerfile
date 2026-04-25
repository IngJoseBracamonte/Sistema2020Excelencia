# ═══════════════════════════════════════════════════════════
# Dockerfile — Sistema Sat Hospitalario WebAPI (.NET 10)
# Multi-stage: SDK (build) → ASP.NET Runtime (final)
# ═══════════════════════════════════════════════════════════

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias (cache layer optimization)
COPY ["SistemaSatHospitalario.WebAPI/SistemaSatHospitalario.WebAPI.csproj", "SistemaSatHospitalario.WebAPI/"]
COPY ["SistemaSatHospitalario.Core.Application/SistemaSatHospitalario.Core.Application.csproj", "SistemaSatHospitalario.Core.Application/"]
COPY ["SistemaSatHospitalario.Core.Domain/SistemaSatHospitalario.Core.Domain.csproj", "SistemaSatHospitalario.Core.Domain/"]
COPY ["SistemaSatHospitalario.Infrastructure/SistemaSatHospitalario.Infrastructure.csproj", "SistemaSatHospitalario.Infrastructure/"]
COPY ["SistemaSatHospitalario.ServiceDefaults/SistemaSatHospitalario.ServiceDefaults.csproj", "SistemaSatHospitalario.ServiceDefaults/"]

RUN dotnet restore "SistemaSatHospitalario.WebAPI/SistemaSatHospitalario.WebAPI.csproj"

# Copiar código fuente y compilar
COPY . .
WORKDIR "/app/SistemaSatHospitalario.WebAPI"
RUN dotnet build "SistemaSatHospitalario.WebAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SistemaSatHospitalario.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

LABEL maintainer="SistemaSatHospitalario" \
      description="Hospital System WebAPI" \
      version="1.0"

WORKDIR /app

# Instalar curl (healthcheck) y configurar timezone Venezuela
RUN apt-get update && apt-get install -y --no-install-recommends curl tzdata \
    && rm -rf /var/lib/apt/lists/*

ENV TZ=America/Caracas
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Directorio para logs persistentes (montado como volumen)
RUN mkdir -p /app/Logs

COPY --from=publish /app/publish .

EXPOSE 8080

# Health check nativo Docker — verifica endpoint /health del sistema
HEALTHCHECK --interval=30s --timeout=10s --retries=5 --start-period=45s \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SistemaSatHospitalario.WebAPI.dll"]

# Use the official .NET 10 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["SistemaSatHospitalario.WebAPI/SistemaSatHospitalario.WebAPI.csproj", "SistemaSatHospitalario.WebAPI/"]
COPY ["SistemaSatHospitalario.Core.Application/SistemaSatHospitalario.Core.Application.csproj", "SistemaSatHospitalario.Core.Application/"]
COPY ["SistemaSatHospitalario.Core.Domain/SistemaSatHospitalario.Core.Domain.csproj", "SistemaSatHospitalario.Core.Domain/"]
COPY ["SistemaSatHospitalario.Infrastructure/SistemaSatHospitalario.Infrastructure.csproj", "SistemaSatHospitalario.Infrastructure/"]
COPY ["SistemaSatHospitalario.ServiceDefaults/SistemaSatHospitalario.ServiceDefaults.csproj", "SistemaSatHospitalario.ServiceDefaults/"]

RUN dotnet restore "SistemaSatHospitalario.WebAPI/SistemaSatHospitalario.WebAPI.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/app/SistemaSatHospitalario.WebAPI"
RUN dotnet build "SistemaSatHospitalario.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SistemaSatHospitalario.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 10 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SistemaSatHospitalario.WebAPI.dll"]

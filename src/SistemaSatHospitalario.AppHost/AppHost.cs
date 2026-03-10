var builder = DistributedApplication.CreateBuilder(args);

// WebAPI
var apiService = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("apiservice");

// Angular PWA
builder.AddNpmApp("angular-frontend", "../SistemaSatHospitalario.Frontend", scriptName: "start")
    .WithReference(apiService)
    .WithHttpEndpoint(env: "PORT")
    .WaitFor(apiService);

builder.Build().Run();

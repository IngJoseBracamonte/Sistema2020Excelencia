var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("api");

builder.AddNpmApp("frontend", "../SistemaSatHospitalario.Frontend", "start")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();

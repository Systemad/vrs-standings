using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var webApi = builder.AddProject<vrsranking_Server>("webApi");
//var webApp = builder.AddViteApp("webApp", "../vrsranking.client", "pnpm").WithPnpmPackageInstallation().WaitFor(webApi);

builder.AddViteApp("webApp", "../vrsranking.client", "pnpm").WithPnpmPackageInstallation().WaitFor(webApi);
/*
builder.AddPnpmApp("server", "../vrsranking.client", "dev")
    //.WithHttpsEndpoint(env: "52032")
    .WithExternalHttpEndpoints()
    .WithPnpmPackageInstallation()
    .WithReference(webApi)
    .WaitFor(webApi);
*/
builder
    .Build().Run();
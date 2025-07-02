using Projects;
using AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env")
    .ConfigureComposeFile(file =>
    {
        file.Name = "vrsranking";
    });

builder.AddDashboard();
var webApi = builder.AddProject<WebApi>("webapi");
//var webApp = builder.AddViteApp("webApp", "../vrsranking.client", "pnpm").WithPnpmPackageInstallation().WaitFor(webApi);

/*
builder.AddViteApp("webApp", "../vrsranking.client", "pnpm")
    .WithHttpsEndpoint(env: "52032")
    //.WithUrl("https://localhost:52032")
    .WithPnpmPackageInstallation()
    .WaitFor(webApi);
*/

builder.AddPnpmApp("webui", "../webui", "dev")
    .WithPnpmPackageInstallation()
    .WithHttpsEndpoint(env: "PORT")
    .WithReverseProxy(webApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WithOtlpExporter()
    .WithEnvironment("BROWSER", "none");
    
    //.WithReference(webApi)
    //.WaitFor(webApi);
builder
    .Build().Run();
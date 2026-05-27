using Aspire.Hosting.Docker.Resources.ServiceNodes;
#pragma warning disable ASPIREJAVASCRIPT001

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("compose")
    .WithDashboard(dashboard =>
    {
        dashboard
            .WithHostPort(8080)
            .WithExternalHttpEndpoints()
            .WithForwardedHeaders(enabled: true);
    })
    .ConfigureComposeFile(composeFile =>
    {
        var backend = composeFile.Services["backend"];
        backend.PullPolicy = "build";
    });

var postgres = builder.AddPostgres("postgres");
var db = postgres.AddDatabase("tasks");

var backend = builder
    .AddProject<Projects.Backend>("backend")
    .WithReference(db)
    .WaitFor(db);

var frontend = builder
    .AddProject<Projects.WebMvc>("frontend")
    .WithReference(backend)
    .WaitFor(backend);

// var frontend = builder
//      .AddViteApp("frontend", "../Frontend", "dev")
//      .WithBuildScript("build")
//      .WithPnpm()
//      .PublishAsStaticWebsite(
//          apiPath: "/tasks",
//          apiTarget: backend,
//          options =>
//          {
//              options.StripPrefix = false;
//              options.OutputPath = "./bin/web";
//          }
//         )
//      .WithExternalHttpEndpoints()
//      .WithReference(backend);

builder.Build().Run();

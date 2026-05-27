using Aspire.Npgsql.EntityFrameworkCore.PostgreSQL;
using DtsDevChallenge.Backend.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TUnit.Core.Interfaces;

namespace DtsDevChallenge.Backend.Tests;

public class WebApplicationFactory : WebApplicationFactory<Program>, IAsyncInitializer
{
    /// <inheritdoc />
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Migrations:SkipMigrations"] = "true"
            });
        });
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ITaskRepository>();
            services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        });
        
        return base.CreateHost(builder);
    }

    public Task InitializeAsync()
    {
        _ = Server;

        return Task.CompletedTask;
    }
}
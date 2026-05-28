using System.Net.Http.Headers;
using DtsDevChallenge.WebMvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TUnit.Core.Interfaces;

namespace DtsDevChallenge.E2ETests.Fixtures;

public class ApiClientFixture : IAsyncInitializer
{
    [ClassDataSource<AppFixture>(Shared = SharedType.None)]
    public required AppFixture App { get; init; }

    public ApiClient Client { get; private set; } = null!;

    public Task InitializeAsync()
    {
        var httpClient = App.CreateHttpClient("backend");
        
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        
        Client = new ApiClient(
            httpClient,
            App.App.Services.GetRequiredService<ILogger<ApiClient>>()
        );
        return Task.CompletedTask;
    }
}
using Aspire.Hosting.Testing;
using TUnit.Core.Interfaces;

namespace DtsDevChallenge.E2ETests.Fixtures;

public class FrontendFixture : IAsyncInitializer
{
    [ClassDataSource<AppFixture>(Shared = SharedType.None)]
    public required AppFixture App { get; init; }

    public string Endpoint { get; private set; } = null!;

    public Task InitializeAsync()
    {
        Endpoint = App.App.GetEndpoint("frontend").ToString();
        return Task.CompletedTask;
    }
}
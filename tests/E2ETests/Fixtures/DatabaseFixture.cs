using Npgsql;
using TUnit.Core.Interfaces;

namespace DtsDevChallenge.E2ETests.Fixtures;

public class DatabaseFixture : IAsyncInitializer, IAsyncDisposable
{
    [ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
    public required AppFixture App { get; init; }

    public NpgsqlConnection Connection { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var connectionString = await App.GetConnectionStringAsync("tasks");
        Connection = new NpgsqlConnection(connectionString);
        await Connection.OpenAsync();
    }

    public async ValueTask DisposeAsync() => await Connection.DisposeAsync();
}
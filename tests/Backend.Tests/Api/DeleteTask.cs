using System.Net;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Common;
using Microsoft.Extensions.DependencyInjection;
using TUnit.FsCheck;

namespace DtsDevChallenge.Backend.Tests.Api;

public class DeleteTask
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.None)]
    public required WebApplicationFactory WebApplicationFactory { get; init; }

    [Test]
    public async Task MissingTaskReturns404()
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        var response = await client.DeleteAsync("/tasks/1");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(0);
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task GetTaskItem(TaskItemsWithId data)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        foreach (var taskItem in data.Items)
        {
            repo.Tasks[taskItem.Id] = taskItem;
        }

        var response = await client.DeleteAsync($"/tasks/{data.Items[data.Id].Id}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(0);

        await Assert.That(repo.Tasks).DoesNotContainKey(data.Items[data.Id].Id);
    }
}
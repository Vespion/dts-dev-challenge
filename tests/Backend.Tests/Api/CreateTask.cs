using System.Net;
using System.Net.Http.Json;
using System.Web;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Common;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions.Conditions;
using TUnit.FsCheck;

namespace DtsDevChallenge.Backend.Tests.Api;

public class CreateTask
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.None)]
    public required WebApplicationFactory WebApplicationFactory { get; init; }

    [Test]
    
    public async Task CreateTaskItem()
    {
        var title = "title";
        var description = "description";
        var dueBy = DateTimeOffset.Now;
        var status = TaskItemStatus.Pending;
        
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        var response = await client.PostAsync("/tasks", new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            { "title", title },
            { "description", description },
            { "dueBy", dueBy.ToString("o") },
            { "itemStatus", status.ToString() }
        }));

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.HasJsonContent()).IsTrue();

        var content = await response.Content.ReadFromJsonAsync<TaskItem>();
        
        await Assert.That(content?.Title).IsEqualTo(title);
        await Assert.That(content?.Description).IsEqualTo(description);
        await Assert.That(content?.DueBy).IsEqualTo(dueBy);
        await Assert.That(content?.ItemStatus).IsEqualTo(status);

        var repoItem = repo.Tasks.Single().Value;
        
        await Assert.That(repoItem.Title).IsEqualTo(title);
        await Assert.That(repoItem.Description).IsEqualTo(description);
        await Assert.That(repoItem.DueBy).IsEqualTo(dueBy);
        await Assert.That(repoItem.ItemStatus).IsEqualTo(status);
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using TUnit.FsCheck;

namespace DtsDevChallenge.Backend.Tests.Api.PatchTask;

public class TaskModification
{
    internal const string PatchContentType = "application/json-patch+json";
    
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.None)]
    public required WebApplicationFactory WebApplicationFactory { get; init; }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task ModifyTitle(TaskItem item, string newTitle)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();
        repo.Tasks[item.Id] = item;

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Replace(t => t.Title, newTitle);

        var response = await client.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var apiItem = await response.Content.ReadFromJsonAsync<TaskItem>();
        await Assert.That(apiItem).IsNotNull();
        await Assert.That(apiItem.Title).IsEqualTo(newTitle);
        await Assert.That(repo.Tasks[item.Id].Title).IsEqualTo(newTitle);
    }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task ModifyDescription(TaskItem item, string newValue)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();
        repo.Tasks[item.Id] = item;

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Replace(t => t.Description, newValue);

        var response = await client.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var apiItem = await response.Content.ReadFromJsonAsync<TaskItem>();
        await Assert.That(apiItem).IsNotNull();
        await Assert.That(apiItem.Description).IsEqualTo(newValue);
        await Assert.That(repo.Tasks[item.Id].Description).IsEqualTo(newValue);
    }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task RemoveDescription(TaskItem item)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();
        repo.Tasks[item.Id] = item;

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Remove(t => t.Description);

        var response = await client.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var apiItem = await response.Content.ReadFromJsonAsync<TaskItem>();
        await Assert.That(apiItem).IsNotNull();
        await Assert.That(apiItem.Description).IsNullOrEmpty();
        await Assert.That(repo.Tasks[item.Id].Description).IsNullOrEmpty();
    }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task ModifyStatus(TaskItem item, TaskItemStatus newValue)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();
        repo.Tasks[item.Id] = item;

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Replace(t => t.ItemStatus, newValue);

        var response = await client.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var apiItem = await response.Content.ReadFromJsonAsync<TaskItem>();
        await Assert.That(apiItem).IsNotNull();
        await Assert.That(apiItem.ItemStatus).IsEqualTo(newValue);
        await Assert.That(repo.Tasks[item.Id].ItemStatus).IsEqualTo(newValue);
    }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task ModifyDueBy(TaskItem item, DateTimeOffset newValue)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();
        repo.Tasks[item.Id] = item;

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Replace(t => t.DueBy, newValue);

        var response = await client.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var apiItem = await response.Content.ReadFromJsonAsync<TaskItem>();
        await Assert.That(apiItem).IsNotNull();
        await Assert.That(apiItem.DueBy).IsEqualTo(newValue);
        await Assert.That(repo.Tasks[item.Id].DueBy).IsEqualTo(newValue);
    }
}
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Common;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions.Conditions;
using TUnit.FsCheck;

namespace DtsDevChallenge.Backend.Tests.Api;

public class GetTasks
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.None)]
    public required WebApplicationFactory WebApplicationFactory { get; init; }
    
    [Test]
    public async Task GetEmptyList()
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        var response = await client.GetAsync("/tasks");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.HasJsonContent()).IsTrue();
        
        var stringContent = await response.Content.ReadAsStringAsync();

        await Assert.That(stringContent).IsEqualTo("[]");
    }

    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task GetList(TaskItem[] items)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository) WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        foreach (var taskItem in items)
        {
            repo.Tasks.Add(taskItem.Id, taskItem);
        }
        
        var response = await client.GetAsync($"/tasks?page=0&pageSize={items.Length}");

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.HasJsonContent()).IsTrue();

        var content = await response.Content.ReadFromJsonAsync<TaskItem[]>();
        await Assert.That(content).IsEquivalentTo(items);
    }
    
    [Test]
    [FsCheckProperty(Arbitrary = [typeof(DataGen)])]
    public async Task PagesCorrectly(TaskItem[] items, int sliceSize)
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository) WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        foreach (var taskItem in items)
        {
            repo.Tasks.Add(taskItem.Id, taskItem);
        }

        var itemPages = items.SplitIntoGroups(sliceSize);

        for (var i = 0; i < itemPages.Length; i++)
        {
            var itemPage = itemPages[i];
            var response = await client.GetAsync($"/tasks?page={i}&pageSize={sliceSize}");

            await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
            await Assert.That(response.HasJsonContent()).IsTrue();

            var content = await response.Content.ReadFromJsonAsync<TaskItem[]>();
            await Assert.That(content).IsEquivalentTo(itemPage);
        }
    }
}

internal static class LinqExt
{
    public static T[][] SplitIntoGroups<T>(this T[] array, int n)
    {
        // Validate inputs
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (n <= 0) throw new ArgumentException("Group size (N) must be greater than 0.", nameof(n));
 
        // Split into groups using LINQ
        return array
            .Select((item, index) => new { Item = item, Index = index })
            .GroupBy(pair => pair.Index / n)
            .Select(group => group.Select(pair => pair.Item).ToArray())
            .ToArray();
    }
}
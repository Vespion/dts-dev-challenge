using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.Json;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Backend.Utils;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;

namespace DtsDevChallenge.Backend.Tests.Api.PatchTask.Validation;

public class ApiValidation
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.None)]
    public required WebApplicationFactory WebApplicationFactory { get; init; }
    
    [Test]
    public async Task MissingTaskReturns404()
    {
        var client = WebApplicationFactory.CreateClient();
        var repo = (InMemoryTaskRepository)WebApplicationFactory.Services.GetRequiredService<ITaskRepository>();
        repo.Tasks.Clear();

        var patchDoc = new JsonPatchDocument<TaskItem>();

        var response = await client.PatchAsync("/tasks/1",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, TaskModification.PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);

        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(0);
    }

    [Test]
    public async Task InvalidContentTypeFails()
    {
        var client = WebApplicationFactory.CreateClient();

        var patchDoc = new JsonPatchDocument<TaskItem>();

        var response = await client.PatchAsync("/tasks/1",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, "application/json")
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.UnsupportedMediaType);

        await Assert.That(response.Content.Headers.ContentLength).IsEqualTo(0);
    }
    
    [Test]
    public async Task InvalidPatchFails()
    {
        var client = WebApplicationFactory.CreateClient();

        var response = await client.PatchAsync("/tasks/1",
            new StringContent("{}", Encoding.UTF8, TaskModification.PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RejectsEditingId()
    {
        var client = WebApplicationFactory.CreateClient();

        var patchDoc = new JsonPatchDocument<TaskItem>();
        patchDoc.Add(t => t.Id, 1);
        patchDoc.Replace(t => t.Id, 2);
        patchDoc.Remove(t => t.Id);

        var response = await client.PatchAsync("/tasks/1",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, TaskModification.PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        await PatchDocValidator.AssertEditingIdResults(await response.Content.ReadAsProblemsAsync());
    }
    
    [Test]
    public Task RejectsDeletingTitleAllowingEdits()
    {
        return RejectDeletion(
            "Title",
            t => t.Title,
            "New Title"
        );
    }
        
    [Test]
    public Task RejectsDeletingDueByAllowingEdits()
    {
        return RejectDeletion(
            "DueBy",
            t => t.DueBy,
            DateTimeOffset.Now
        );
    }
        
    [Test]
    public Task RejectsDeletingItemStatusAllowingEdits()
    {
        return RejectDeletion(
            "ItemStatus",
            t => t.ItemStatus,
            TaskItemStatus.Closed
        );
    }
        
    [Test]
    public async Task AllowsEditingDescription()
    {
        var doc = new JsonPatchDocument<TaskItem>();
        doc.Add(x => x.Description, "blah blah blah");
        doc.Replace(x => x.Description, "blah2 blah2 blah2");
        doc.Remove(x => x.Description);
            
        var errors = doc.ValidatePatchDocument();
            
        await Assert.That(errors).Count().IsZero();
    }
        
    private async Task RejectDeletion<TProp>(
        string path,
        Expression<Func<TaskItem, TProp>> prop,
        TProp value
    )
    {
        var client = WebApplicationFactory.CreateClient();

        var doc = new JsonPatchDocument<TaskItem>();
        doc.Add(prop, value);
        doc.Remove(prop);
        doc.Replace(prop, value);

        var response = await client.PatchAsync("/tasks/1",
            new StringContent(JsonSerializer.Serialize(doc), Encoding.UTF8, TaskModification.PatchContentType)
        );

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        
        await PatchDocValidator.AssertRejectDeletionResults(await response.Content.ReadAsProblemsAsync(), path);
    }
}

internal static class Extensions
{
    internal static async Task<IDictionary<string, string[]>> ReadAsProblemsAsync(this HttpContent content)
    {
        var jsonString = await content.ReadAsStringAsync();
        await Assert.That(jsonString).IsNotNull();
        
        var problem = JsonDocument.Parse(jsonString);
        
        var errors  = problem.RootElement.GetProperty("errors").Deserialize<Dictionary<string, string[]>>();
        await Assert.That(errors).IsNotNull();
        return errors!;
    }
}
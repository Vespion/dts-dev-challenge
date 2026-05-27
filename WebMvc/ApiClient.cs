using System.Text;
using System.Text.Json;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace DtsDevChallenge.WebMvc;

public sealed class ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
{
    public async Task<TaskItem[]> GetTaskList(int page = 0, int pageSize = 2500)
    {
        try
        {
            var tasks = await httpClient.GetFromJsonAsync<TaskItem[]>($"/tasks?page={page}&pageSize={pageSize}",
                CommonJsonContext.Default.Options);

            return tasks ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching task list");
        }

        return [];
    }

    public async Task<TaskItem?> GetTask(int id)
    {
        try
        {
            var task = await httpClient.GetFromJsonAsync<TaskItem>($"/tasks/{id}",
                CommonJsonContext.Default.Options);
            
            return task;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching task {Id}", id);
        }

        return null;
    }

    public async Task<TaskItem?> UpdateTask(TaskItem item)
    {
        var patchDoc = new JsonPatchDocument<TaskItem>();
        
        patchDoc.Replace(x => x.Title, item.Title);
        patchDoc.Replace(x => x.Description, item.Description);
        patchDoc.Replace(x => x.DueBy, item.DueBy.ToUniversalTime());
        patchDoc.Replace(x => x.ItemStatus, item.ItemStatus);
        
        var response = await httpClient.PatchAsync($"/tasks/{item.Id}",
            new StringContent(JsonSerializer.Serialize(patchDoc), Encoding.UTF8, "application/json-patch+json")
        );
        
        return await response.Content.ReadFromJsonAsync<TaskItem>();
    }
    
    public async Task<TaskItem?> CreateTask(TaskItem item)
    {
        try
        {
            var response = await httpClient.PostAsync("/tasks", new FormUrlEncodedContent(new Dictionary<string, string?>
            {
                { "title", item.Title },
                { "description", item.Description },
                { "dueBy", item.DueBy.ToString("o") },
                { "itemStatus", item.ItemStatus.ToString() }
            }));

            return await response.Content.ReadFromJsonAsync<TaskItem>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating new task {@Task}", item);
        }

        return null;
    }

    public async Task DeleteTask(int id)
    {
       await httpClient.DeleteAsync($"/tasks/{id}");
    }
}
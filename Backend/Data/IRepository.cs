using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace DtsDevChallenge.Backend.Data;

public interface ITaskRepository
{
    ValueTask<TaskItem[]> GetTasks(uint page = 0, uint pageSize = 10);
    
    ValueTask<TaskItem> CreateTaskItem(string title, string? description, DateTimeOffset dueBy, TaskItemStatus itemStatus);
    
    ValueTask<TaskItem?> GetTaskItem(int id);
    
    ValueTask DeleteTaskItem(int id);
    
    ValueTask<TaskItem> ModifyTaskItem(int id, JsonPatchDocument<TaskItem> patch);
}
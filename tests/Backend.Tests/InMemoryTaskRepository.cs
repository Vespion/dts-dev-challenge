using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;

namespace DtsDevChallenge.Backend.Tests;

public sealed class InMemoryTaskRepository: ITaskRepository, IDisposable
{
    internal readonly Dictionary<int, TaskItem> Tasks = new();
    private int _nextId;
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);
    
    /// <inheritdoc />
    public ValueTask<TaskItem[]> GetTasks(uint page = 0, uint pageSize = 10)
    {
        try
        {
            _lock.EnterReadLock();
            return ValueTask.FromResult(Tasks
                .Skip((int)(page * pageSize))
                .Take((int)pageSize)
                .Select(x => x.Value)
                .ToArray());
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public ValueTask<TaskItem> CreateTaskItem(string title, string? description, DateTimeOffset dueBy, TaskItemStatus itemStatus)
    {
        try
        {
            _lock.EnterWriteLock();
            var id = Interlocked.Increment(ref _nextId);
            Tasks[id] = new TaskItem(id, title, description, dueBy, itemStatus);
            return ValueTask.FromResult(Tasks[id]);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public ValueTask<TaskItem?> GetTaskItem(int id)
    {
        try
        {
            _lock.EnterReadLock();
            return ValueTask.FromResult(Tasks.TryGetValue(id, out var item) ? item : null);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <inheritdoc />
    public ValueTask DeleteTaskItem(int id)
    {
        try
        {
            _lock.EnterWriteLock();
            return !Tasks.Remove(id) ? throw new KeyNotFoundException() : ValueTask.CompletedTask;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public ValueTask<TaskItem> ModifyTaskItem(int id, JsonPatchDocument<TaskItem> patch)
    {
        try
        {
            _lock.EnterWriteLock();
            var task = Tasks[id];
            patch.ApplyTo(task);
            Tasks[id] = task;
            return ValueTask.FromResult(Tasks[id]);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _lock.Dispose();
    }
}
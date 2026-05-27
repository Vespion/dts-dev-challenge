namespace DtsDevChallenge.Common;

/// <summary>
/// Data model for a task
/// </summary>
public class TaskItem
{
    public TaskItem()
    {
    }
    
    /// <summary>
    /// Data model for a task
    /// </summary>
    /// <param name="id">The ID for this task</param>
    /// <param name="title">The title for this task</param>
    /// <param name="description">An optional description for the task</param>
    /// <param name="dueBy">The date/time this task is due</param>
    /// <param name="itemStatus">The current status of this task</param>
    public TaskItem(int id, string title, string? description, DateTimeOffset dueBy, TaskItemStatus itemStatus)
    {
        Id = id;
        Title = title;
        Description = description;
        DueBy = dueBy;
        ItemStatus = itemStatus;
    }

    /// <summary>The ID for this task</summary>
    public int Id { get; set; }

    /// <summary>The title for this task</summary>
    public string Title { get; set; }

    /// <summary>An optional description for the task</summary>
    public string? Description { get; set; }

    /// <summary>The date/time this task is due</summary>
    public DateTimeOffset DueBy { get; set; }

    /// <summary>The current status of this task</summary>
    public TaskItemStatus ItemStatus { get; set; }

    public void Deconstruct(out int id, out string title, out string? description, out DateTimeOffset dueBy, out TaskItemStatus itemStatus)
    {
        id = Id;
        title = Title;
        description = Description;
        dueBy = DueBy;
        itemStatus = ItemStatus;
    }
}
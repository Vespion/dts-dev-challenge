namespace DtsDevChallenge.Common;

/// <summary>
/// The available states for a task
/// </summary>
public enum TaskItemStatus
{
    /// <summary>
    /// The task is in an unknown state, this is the default value.
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// A task that has been created but is not currently being worked on
    /// </summary>
    Pending,
    /// <summary>
    /// A task that was previously worked on but has been paused
    /// </summary>
    Paused,
    /// <summary>
    /// A task actively being worked on
    /// </summary>
    Active,
    /// <summary>
    /// A completed task
    /// </summary>
    Closed
}
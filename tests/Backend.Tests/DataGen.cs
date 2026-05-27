using System.Diagnostics.CodeAnalysis;
using System.Web;
using DtsDevChallenge.Common;
using FsCheck;
using FsCheck.Fluent;

namespace DtsDevChallenge.Backend.Tests;

public record TaskItemsWithId(int Id, TaskItem[] Items);

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class DataGen
{
    private static int _nextId;

    // ReSharper disable once InconsistentNaming
    private static readonly Gen<TaskItem> _taskItem =
        from title in ArbMap.Default.GeneratorFor<string>().Where(s => !string.IsNullOrEmpty(s))
        from description in ArbMap.Default.GeneratorFor<string>()
        from dueBy in ArbMap.Default.GeneratorFor<DateTimeOffset>()
        from status in ArbMap.Default.GeneratorFor<TaskItemStatus>()
        select new TaskItem(Interlocked.Increment(ref _nextId), title, description, dueBy, status);
    
    public static Arbitrary<TaskItem> TaskItem()
    {
        return _taskItem.ToArbitrary();
    }
    
    public static Arbitrary<int> PositiveInt()
    {
        return ArbMap.Default.GeneratorFor<int>()
            .Where(x => x > 0)
            .ToArbitrary();
    }
    
    public static Arbitrary<string> NonEmptyString()
    {
        return ArbMap.Default.GeneratorFor<string>()
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArbitrary();
    }
    
    public static Arbitrary<TaskItemsWithId> TaskItemsWithId()
    {
        var gen = 
            from tasks in _taskItem.NonEmptyListOf()
            from targetId in ArbMap.Default.GeneratorFor<int>()
                .Where(x => x < tasks.Count && x >= 0)
            select new TaskItemsWithId(targetId, tasks.ToArray());
        return gen.ToArbitrary();
    }
}
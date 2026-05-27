using System.Diagnostics.CodeAnalysis;
using DtsDevChallenge.Common;
using Microsoft.EntityFrameworkCore;

namespace DtsDevChallenge.Backend.Data;

/// <summary>
/// Database Context for task items
/// </summary>
[RequiresUnreferencedCode("EF Core isn't fully compatible with NativeAOT")]
[RequiresDynamicCode("EF Core isn't fully compatible with NativeAOT")]
public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    /// <summary>
    /// The tasks table
    /// </summary>
    public DbSet<TaskItem>  Tasks { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TaskItem>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
    }
}
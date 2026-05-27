using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using DtsDevChallenge.Backend;
using DtsDevChallenge.Backend.Data;
using DtsDevChallenge.Backend.Utils;
// using DtsDevChallenge.Backend.Utils;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

[assembly: InternalsVisibleTo("Backend.Tests")]

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();
#pragma warning disable IL2026
builder.AddNpgsqlDbContext<TaskDbContext>("tasks");
builder.Services.AddScoped<ITaskRepository, DbContextTaskRepository>();
#pragma warning restore IL2026

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CommonJsonContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference();

var tasksApi = app.MapGroup("/tasks")
    .WithTags("Tasks")
    .Experimental();

tasksApi.MapGet("/", async Task<Results<ProblemHttpResult, JsonHttpResult<TaskItem[]>>> (
        [FromServices] ITaskRepository repo,
        [FromServices] ILogger<Program> logger,
        [FromQuery, Description("The page number to fetch")]
        uint page = 0,
        [FromQuery, Description("The size of the page to fetch")]
        uint pageSize = 15
    ) =>
    {
        try
        {
            return TypedResults.Json(
                await repo.GetTasks(page, pageSize), CommonJsonContext.Default
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the task item page");
            return TypedResults.Problem(
                title: "An error occurred while getting the task item page",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    })
    .WithName("GetTasks")
    .WithSummary("Get tasks")
    .WithDescription("Fetches a page of task items")
    .CacheOutput();

tasksApi.MapPost("/", async Task<Results<ProblemHttpResult, JsonHttpResult<TaskItem>>> (
        [FromServices] ITaskRepository repo,
        [FromServices] ILogger<Program> logger,
        [Required, Description("The title of the task"), FromForm]
        string title,
        [Description("The description of the task"), FromForm]
        string? description,
        [Required, Description("When the task is due by"), FromForm]
        DateTimeOffset dueBy,
        [Required, Description("The current status of the task"), FromForm]
        TaskItemStatus itemStatus
    ) =>
    {
        try
        {
            return TypedResults.Json(
                await repo.CreateTaskItem(title, description, dueBy.ToUniversalTime(), itemStatus),
                CommonJsonContext.Default
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the task item");
            return TypedResults.Problem(
                title: "An error occurred while creating the task item",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    })
    .DisableAntiforgery()
    .WithName("CreateTask")
    .WithSummary("Create task")
    .WithDescription("Creates a new task item");

tasksApi.MapGet("/{id:long}", async Task<Results<NotFound, ProblemHttpResult, JsonHttpResult<TaskItem>>> (
        [FromRoute, Description("The ID number of the task")]
        int id,
        [FromServices] ITaskRepository repo,
        [FromServices] ILogger<Program> logger
    ) =>
    {
        try
        {
            var task = await repo.GetTaskItem(id);
            if (task == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Json(task, CommonJsonContext.Default);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the task item with ID {Id}", id);
            return TypedResults.Problem(
                title: "An error occurred while fetching the task item",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: id.ToString()
            );
        }
    })
    .WithName("GetTask")
    .WithSummary("Get task")
    .WithDescription("Fetches a task by its ID number")
    .CacheOutput();

tasksApi.MapDelete("/{id:long}", async Task<Results<NotFound, NoContent, ProblemHttpResult>> (
        [FromRoute, Description("The ID number of the task")]
        int id,
        [FromServices] ITaskRepository repo,
        [FromServices] ILogger<Program> logger
    ) =>
    {
        try
        {
            await repo.DeleteTaskItem(id);
            return TypedResults.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the task item");
            return TypedResults.Problem(
                title: "An error occurred while deleting the task item",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: id.ToString()
            );
        }
    })
    .WithName("DeleteTask")
    .WithSummary("Delete task")
    .WithDescription("Deletes a task");

tasksApi.MapPatch("/{id:long}",
        async Task<Results<NotFound, ProblemHttpResult, ValidationProblem, JsonHttpResult<TaskItem>>> (
            [FromRoute, Description("The ID number of the task")]
            int id,
            [FromBody, Description("An RFC 6902 JSON Patch document to apply to the target task")]
            JsonPatchDocument<TaskItem> patchDoc,
            [FromServices] ITaskRepository repo,
            [FromServices] ILogger<Program> logger
        ) =>
        {
            try
            {
                var validationResults = patchDoc.ValidatePatchDocument();

                if (validationResults.Count > 0)
                {
                    return TypedResults.ValidationProblem(
                        validationResults,
                        title: "The JSON Patch document is invalid"
                    );
                }

                return TypedResults.Json(await repo.ModifyTaskItem(id, patchDoc), CommonJsonContext.Default);
            }
            catch (KeyNotFoundException)
            {
                return TypedResults.NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while modifying the task item");
                return TypedResults.Problem(
                    title: "An error occurred while modifying the task item",
                    statusCode: StatusCodes.Status500InternalServerError,
                    instance: id.ToString()
                );
            }
        })
    .ProducesValidationProblem()
    .Accepts<JsonPatchDocument<TaskItem>>("application/json-patch+json")
    .WithName("PatchTask")
    .WithSummary("Patch task")
    .WithDescription("Modifies a task using a RFC 6902 JSON PATCH document");

using (var startupScope = app.Services.CreateScope())
{
    var services = startupScope.ServiceProvider;
    var skipMigrations = services.GetRequiredService<IConfiguration>().GetValue<bool>("Migrations:SkipMigrations");

    if (!skipMigrations)
    {
        try
        {
            var context = services.GetRequiredService<TaskDbContext>();

            // Applies any pending migrations for the context to the database.
            // Will create the database if it does not already exist.
#pragma warning disable IL3050
            await context.Database.MigrateAsync().ConfigureAwait(false);
#pragma warning restore IL3050
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database");

            // Terminate the app if migration fails so it doesn't run in an invalid state
            throw;
        }
    }
}

app.Run();
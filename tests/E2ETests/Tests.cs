using DtsDevChallenge.E2ETests.Fixtures;
using Microsoft.Playwright;
using TUnit.Playwright;

namespace DtsDevChallenge.E2ETests;

public class UserJourneys() : PageTest
{
    
    [ClassDataSource<FrontendFixture>(Shared = SharedType.None)]
    public required FrontendFixture Frontend { get; init; }

    
    [Test]
    public async Task EmptyListShowsBanner()
    {
        await Page.GotoAsync(Frontend.Endpoint);
        
        await Expect(Page.GetByText("Tasks complete")).ToBeVisibleAsync();
        await Expect(Page.GetByText("You do not currently have any tasks to complete.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task CreateTask()
    {
        var title = "Test Task 1";
        var description = "This is a test task! :D";
        var status = "Active";
        
        await Page.GotoAsync(Frontend.Endpoint);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add new task" }).ClickAsync();
        await Page.Locator("#Title").FillAsync(title);
        await Page.Locator("#ItemStatus").SelectOptionAsync([status]);
        await Page.Locator("#DueBy").FillAsync("2030-05-15T15:00");
        await Page.Locator("#Description").FillAsync(description);
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        
        await Expect(Page.GetByText("1")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Back to task list" }).ClickAsync();
        
        await Expect(Page.GetByText(description)).ToBeVisibleAsync();
        await Expect(Page.GetByText(status)).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task ModifyTask()
    {
        //Setup
        var title = "Test Task 1";
        var description = "This is a test task! :D";
        var status = "Active";
        
        await Page.GotoAsync(Frontend.Endpoint);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add new task" }).ClickAsync();
        await Page.Locator("#Title").FillAsync(title);
        await Page.Locator("#ItemStatus").SelectOptionAsync([status]);
        await Page.Locator("#DueBy").FillAsync("2030-05-15T15:00");
        await Page.Locator("#Description").FillAsync(description);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Back to task list" }).ClickAsync();
        
        //Test
        var newDescription = "This is an edited test task! :D";
        var newStatus = "Paused";
        
        await Page.GetByRole(AriaRole.Link, new() { Name = $"#1 - {title} (click to edit)" }).ClickAsync();
        
        await Page.Locator("#ItemStatus").SelectOptionAsync([newStatus]);
        await Page.Locator("#Description").FillAsync(newDescription);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Back to task list" }).ClickAsync();
        
        await Expect(Page.GetByText(newDescription)).ToBeVisibleAsync();
        await Expect(Page.GetByText(newStatus)).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task DeleteTask()
    {
        //Setup
        var title = "Test Task 1";
        var description = "This is a test task! :D";
        var status = "Active";
        
        await Page.GotoAsync(Frontend.Endpoint);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add new task" }).ClickAsync();
        await Page.Locator("#Title").FillAsync(title);
        await Page.Locator("#ItemStatus").SelectOptionAsync([status]);
        await Page.Locator("#DueBy").FillAsync("2030-05-15T15:00");
        await Page.Locator("#Description").FillAsync(description);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Back to task list" }).ClickAsync();
        
        //Test
        await Page.GetByRole(AriaRole.Link, new() { Name = $"#1 - {title} (click to edit)" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        
        await Expect(Page.GetByText("Tasks complete")).ToBeVisibleAsync();
        await Expect(Page.GetByText("You do not currently have any tasks to complete.")).ToBeVisibleAsync();
    }
}
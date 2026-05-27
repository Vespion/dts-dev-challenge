using DtsDevChallenge.WebMvc;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ApiClient>((client) =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BACKEND_HTTP"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRewriter(new RewriteOptions()
    .AddRewrite("^assets/(.*)", "lib/govuk/assets/$1", skipRemainingRules: true)
);

app.UseStaticFiles();

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
using api;
using api.CachedServices;
using GitHub;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.Configure<RepositoryPortfolio>(builder.Configuration.GetSection(nameof(RepositoryPortfolio)));
builder.Services.AddGitHubIntegration(options=>builder.Configuration.GetSection(nameof(GitHubIntegrationOptions)).Bind(options));
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IMemoryCache, MemoryCache>();
builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.Decorate<IGitHubService,CachedGitHubService>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CV Site api");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

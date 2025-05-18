using Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Load options from secrets / appsettings
builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection("GitHub"));

// Register services
builder.Services.AddMemoryCache(); // הוספת שורה זו


builder.Services.AddScoped<IGitHubService, GitHubService>();
builder.Services.Decorate<IGitHubService, GitHubCachingDecorator>();



// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

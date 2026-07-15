using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using TaskApp.Api.Middleware;
using TaskApp.Application;
using TaskApp.Infrastructure;
using TaskApp.Infrastructure.Persistence;

// Named so the CORS setup below and the app.UseCors() call agree on which policy
// to apply, without a magic string repeated in two places.
const string FrontendCorsPolicy = "FrontendCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Enums as strings on the wire (e.g. "Medium" instead of 1) — readable request
    // bodies and consistent with how TaskDto already renders Priority as text.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Global exception handling: any exception that escapes a controller action is
// caught here and turned into a 500 ProblemDetails response. AddProblemDetails()
// customizes ALL ProblemDetails responses app-wide (400s, 404s, and this 500),
// not just the ones from the exception handler.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskApp API",
        Version = "v1",
        Description = "REST API for a task management app (Lateral Group take-home)."
    });

    // Feeds the /// XML doc comments from Api (endpoints) and Application (DTOs)
    // into Swagger's descriptions — see GenerateDocumentationFile in both csproj files.
    foreach (var xmlFile in new[] { "TaskApp.Api.xml", "TaskApp.Application.xml" })
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
});

builder.Services.AddCors(options =>
{
    // Vite's default dev server port. The frontend doesn't exist yet (TASK-9), but
    // this is the origin it will run on, so CORS is ready when it arrives.
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply pending EF Core migrations and seed example data on startup. Scoped
// because TaskAppDbContext is a scoped service and Program.cs runs outside a request.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskAppDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.

// First in the pipeline so it can catch exceptions from everything after it.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();

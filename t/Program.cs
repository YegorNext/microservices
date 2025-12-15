using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<MetricsService>();

builder.Services.AddScoped<MetricsServiceClient>(provider =>
{
    var httpClient = new HttpClient();
    var endpoint = "http://localhost:5072/api/metrics";
    return new MetricsServiceClient(httpClient, endpoint);
});

builder.Services.AddScoped<AuditServiceClient>(provider =>
{
    var httpClient = new HttpClient();
    var endpoint = "http://localhost:5072/api/audit"; // або свій endpoint
    return new AuditServiceClient(httpClient, endpoint);
});

builder.Services.AddScoped<AuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

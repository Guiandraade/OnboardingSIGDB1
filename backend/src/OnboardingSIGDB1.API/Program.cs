using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OnboardingSIGDB1.API.Middleware;
using OnboardingSIGDB1.IOC;
using OnboardingSIGDB1.Domain.Dto.Companies.Commands;
using System.Reflection;

const string frontendLocalDevPolicy = "FrontendLocalDevPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OnboardingSIGDB1 API",
        Version = "v1",
        Description = "API documentation for company and employee onboarding operations."
    });

    var xmlDocumentationFiles = new[]
    {
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        $"{typeof(CompanyRequest).Assembly.GetName().Name}.xml"
    };

    foreach (var xmlFile in xmlDocumentationFiles.Distinct())
    {
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }

    c.MapType<DateTime>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string", 
        Format = "date-time",
        Nullable = true,
        Example = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15T10:30:00Z")
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendLocalDevPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
});
Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Global exception handling — must be the outermost middleware so it catches
// any unhandled infrastructure exception from the entire request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(frontendLocalDevPolicy);
app.MapControllers();
app.Run();


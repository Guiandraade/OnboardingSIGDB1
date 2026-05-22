using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using OnboardingSIGDB1.API.ModelBinders;
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
        Format = "date",
        Nullable = true,
        Example = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("dd/MM/yyyy"))
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


builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateFormatString = "dd/MM/yyyy";
});
Startup.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

var supportedCultures = new[] { new System.Globalization.CultureInfo("pt-BR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

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


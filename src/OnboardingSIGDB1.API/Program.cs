using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using OnboardingSIGDB1.IOC;

const string FrontendLocalDevPolicy = "FrontendLocalDevPolicy";

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
    options.AddPolicy(FrontendLocalDevPolicy, policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddControllers().AddNewtonsoftJson(options =>
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
app.UseCors(FrontendLocalDevPolicy);
app.MapControllers();
app.Run();


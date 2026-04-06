using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Data.Persistence;
using OnboardingSIGDB1.Data.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Contexts;
using OnboardingSIGDB1.Domain.Interfaces.Persistence;
using OnboardingSIGDB1.Domain.Interfaces.Repositories;
using OnboardingSIGDB1.Domain.Interfaces.Services;
using OnboardingSIGDB1.Domain.Notifications;
using OnboardingSIGDB1.Domain.Services.Companies;
using OnboardingSIGDB1.Domain.Services.Employees;
using OnboardingSIGDB1.Domain.Services.Positions;

namespace OnboardingSIGDB1.IOC;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<OnboardingDbContext>(options =>
            options.UseSqlServer(connectionString));

        //employee
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        
        //position
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IPositionService, PositionService>();
        
        //company
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyService, CompanyService>();

        //notifications
        services.AddScoped<INotificationContext, NotificationContext>();

        //unitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
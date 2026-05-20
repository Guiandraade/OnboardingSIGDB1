using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnboardingSIGDB1.Data.Context;
using OnboardingSIGDB1.Data.Persistence;
using OnboardingSIGDB1.Data.Repositories;
using OnboardingSIGDB1.Domain.Dto.Common.Filters;
using OnboardingSIGDB1.Domain.Dto.Common.Filters.Validators;
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

        // Employee
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IValidator<EmployeeFilter>, EmployeeFilterValidator>();
        
        // Position
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IValidator<PositionFilter>, PositionFilterValidator>();
        
        // Company
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IValidator<CompanyFilter>, CompanyFilterValidator>();

        // Employee positions
        services.AddScoped<IEmployeePositionsRepository, EmployeePositionsRepository>();
        
        // Notifications
        services.AddScoped<INotificationContext, NotificationContext>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SpecEdu.Application.Common.Interfaces;
using SpecEdu.Application.Common.Models;
using SpecEdu.Infrastructure.Authorization;
using SpecEdu.Infrastructure.Data;
using SpecEdu.Infrastructure.Identity;
using SpecEdu.Infrastructure.Services;

namespace SpecEdu.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SpecEduConnectionString"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        services.AddDataProtection()
            .SetApplicationName("SpecEdu")
            .PersistKeysToDbContext<ApplicationDbContext>();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? new JwtSettings();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT Bearer is available for API endpoints, but Identity cookies are the default for Razor Pages
        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Email settings
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ISchoolService, SchoolService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IStudentAccessService, StudentAccessService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IDiaryService, DiaryService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<IPlppService, PlppService>();
        services.AddScoped<IPlppVersionService, PlppVersionService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IConsultationService, ConsultationService>();

        // Register Lazy<T> for services with circular dependencies
        services.AddTransient(typeof(Lazy<>), typeof(LazyResolver<>));

        // Background service for processing reminders
        services.AddHostedService<ReminderBackgroundService>();

        // Register authorization handlers
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, StudentAccessAuthorizationHandler>();

        // Add authorization policies
        services.AddAuthorization(AuthorizationPolicies.AddPolicies);

        return services;
    }
}

/// <summary>
/// Helper class for resolving Lazy<T> dependencies.
/// Allows breaking circular dependencies by deferring service resolution.
/// </summary>
internal class LazyResolver<T> : Lazy<T> where T : class
{
    public LazyResolver(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<T>)
    {
    }
}

using System.Text;
using Application.Data;
using Application.Security;
using Domain.Readers;
using Domain.Repositories;
using Infrastructure.Database;
using Infrastructure.Database.Data;
using Infrastructure.Database.Readers;
using Infrastructure.Database.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        AddAuthentication(services, configuration);
        AddDatabase(services, configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
        );

        services.AddScoped<ITaskReader, TaskReader>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IDataContext, DataContext>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{configuration["Auth:Domain"]}";
                options.Audience = configuration["Auth:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://{configuration["Auth:Domain"]}",

                    ValidateAudience = true,
                    ValidAudience = configuration["Auth:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SigningKeyFromConfiguration(configuration),

                    ValidateLifetime = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256, SecurityAlgorithms.RsaSha256]
                };
            });

        services.AddScoped<ISecurityContext, SecurityContext>();

        return services;
    }

    private static SymmetricSecurityKey SigningKeyFromConfiguration(
        IConfiguration configuration)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Auth:SigningKey"]!));
    }
}
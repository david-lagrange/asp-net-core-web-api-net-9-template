using NetCoreWebAPIJWTAuth.Core.Domain.Repositories;
using NetCoreWebAPIJWTAuth.Core.Services.Abstractions;
using NetCoreWebAPIJWTAuth.Core.Services;
using NetCoreWebAPIJWTAuth.Infrastructure.Persistence;
using LoggingService;
using Microsoft.EntityFrameworkCore;
using NetCoreWebAPIJWTAuth.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NetCoreWebAPIJWTAuth.Core.Domain.ConfigurationModels;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace NetCoreWebAPIJWTAuth.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"));
        });

    public static void ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options =>
        {

        });

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("sqlConnection")?.Replace("{POSTGRESQL_DB_PASSWORD}", Environment.GetEnvironmentVariable("LOCAL_POSTGRESQL_DB_PASSWORD") ?? string.Empty) ?? throw new InvalidOperationException("Connection string 'sqlConnection' is not found.")));

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole>(o =>
        {
            o.Password.RequireDigit = true;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 10;
            o.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<RepositoryContext>()
        .AddDefaultTokenProviders();
    }

    public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(jwtConfiguration.Section, jwtConfiguration);

        var secretKey = Environment.GetEnvironmentVariable("API_JWT_SECRET");

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("API_JWT_SECRET environment variable is not set.");
        }

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguration.ValidIssuer,
                ValidAudience = jwtConfiguration.ValidAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }

    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration) => services
        .Configure<JwtConfiguration>(configuration.GetSection("JwtSettings"));

    //public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    //{
    //    string databasePasswordEnvVar = "LOCAL_POSTGRESQL_DB_PASSWORD";

    //    var connectionString = configuration.GetConnectionString("sqlConnection");
    //    var dbPassword = Environment.GetEnvironmentVariable(databasePasswordEnvVar);

    //    if (connectionString == null)
    //    {
    //        throw new InvalidOperationException("Connection string 'sqlConnection' not found.");
    //    }

    //    if (dbPassword == null)
    //    {
    //        throw new InvalidOperationException($"Environment variable '{databasePasswordEnvVar}' not found.");
    //    }

    //    connectionString = connectionString.Replace("{POSTGRESQL_DB_PASSWORD}", dbPassword);

    //    services.AddHealthChecks()
    //        .AddNpgSql(connectionString!, name: "PostgreSql Health");

    //    services.AddHealthChecksUI()
    //        .AddInMemoryStorage();
    //}

    //public static void ConfigureHealthChecksEndpoints(this WebApplication app)
    //{
    //    app.MapHealthChecks("/health", new HealthCheckOptions
    //    {
    //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    //    });

    //    // app.MapHealthChecksUI();
    //}

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Equanimity API",
                Version = "v1",
                Description = "API by Equanimity",
                TermsOfService = new Uri("https://www.equanimity-solutions.com/terms-of-use"),
                Contact = new OpenApiContact
                {
                    Name = "David Lagrange",
                    Email = "david@equanimity-solutions.com",
                    Url = new Uri("https://www.equanimity-solutions.com/about"),
                },
                License = new OpenApiLicense
                {
                    Name = "Equanimity API LICX",
                    Url = new Uri("https://www.equanimity-solutions.com/privacy-policy"),
                }
            });

            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });

            var xmlFile = $"{typeof(Infrastructure.Presentation.AssemblyReference)
                .Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);
        });
    }
}

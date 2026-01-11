using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using KontoApi.Api.Middleware;
using KontoApi.Api.Services;
using KontoApi.Application;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Infrastructure;
using KontoApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

namespace KontoApi.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web application");

            var app = CreateWebApplication(args);

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<KontoDbContext>();

                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        Log.Information("Applying database migrations...");
                        await context.Database.MigrateAsync();
                        Log.Information("Database migrations applied successfully");
                    }
                }
                catch (Exception exception)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("CRITICAL ERROR: Database migration failed");
                    sb.AppendLine($"Message: {exception.Message}");

                    if (exception.InnerException is PostgresException pgEx)
                    {
                        sb.AppendLine($"SqlState: {pgEx.SqlState}");
                        sb.AppendLine($"Detail: {pgEx.Detail}");
                        sb.AppendLine($"Hint: {pgEx.Hint}");
                        sb.AppendLine($"TableName: {pgEx.TableName}");
                    }
                    else if (exception.InnerException != null)
                    {
                        sb.AppendLine($"Inner Exception: {exception.InnerException.Message}");
                    }

                    Log.Fatal(exception, sb.ToString());
                    throw;
                }
            }

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder);

        var app = builder.Build();

        ConfigurePipeline(app);

        return app;
    }

    // Expose CreateHostBuilder for WebApplicationFactory used in integration tests.
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, loggerConfiguration)
                => loggerConfiguration.ReadFrom.Configuration(context.Configuration))
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<IntegrationTestStartup>(); });
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();

        builder.Host.UseSerilog((context, loggerConfiguration)
            => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "KontoApi", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new()
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
        });

        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtKey = configuration["Jwt:Key"] ?? "development_jwt_key";
                var jwtIssuer = configuration["Jwt:Issuer"] ?? "development_issuer";
                var jwtAudience = configuration["Jwt:Audience"] ?? "development_audience";

                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        builder.Services.AddApplication();

        builder.Services.AddInfrastructure(configuration, builder.Environment);

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                if (allowedOrigins.Length == 0)
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                else
                    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            });
        });
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        app.UseForwardedHeaders();

        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
            };
        });

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHealthChecks("/health");
        app.MapControllers();
    }
}

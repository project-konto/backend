using System.Reflection;
using System.Text;
using KontoApi.Api.Middleware;
using KontoApi.Api.Services;
using KontoApi.Application;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using KontoApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using KontoApi.Domain;
using KontoApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Api
{
    // Startup-like class used for integration tests host builder
    public class IntegrationTestStartup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public IntegrationTestStartup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                System.IO.File.AppendAllText("/tmp/konto_startup.log", DateTime.UtcNow + " - ConfigureServices invoked\n");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to write startup log in ConfigureServices");
            }

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();
            services.AddHealthChecks();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "KontoApi",
                    Version = "v1",
                    Description = "Personal Finance Management API"
                });

                options.AddSecurityDefinition("Bearer", new()
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345token\"",
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
                            Reference = new()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, KontoApi.Api.TestAuth.TestAuthenticationHandler>("Test", _ => { })
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = true;

                    var jwtKey = _configuration["Jwt:Key"] ?? "test_jwt_key";
                    var jwtIssuer = _configuration["Jwt:Issuer"] ?? "test_issuer";
                    var jwtAudience = _configuration["Jwt:Audience"] ?? "test_audience";

                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };

                    options.Events = new()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IntegrationTestStartup>>();
                            logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IntegrationTestStartup>>();
                            logger.LogInformation("Token validated successfully for user: {User}", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddApplication();
            services.AddInfrastructure(_configuration);

            // Integration tests should use an in-memory database to avoid requiring Postgres.
            services.AddDbContext<KontoDbContext>(options =>
                options.UseInMemoryDatabase("KontoApi_TestDb"));

            var allowedOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            services.AddCors(options =>
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

        public void Configure(IApplicationBuilder app)
        {
            try
            {
                System.IO.File.AppendAllText("/tmp/konto_startup.log", DateTime.UtcNow + " - Configure invoked\n");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to write startup log in Configure");
            }

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

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseRouting();

            // Seed a default test user in the in-memory database for integration tests
            try
            {
                using var scope = app.ApplicationServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<KontoDbContext>();
                db.Database.EnsureCreated();

                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var testEmail = "testuser@example.com";
                if (!db.Users.Any(u => u.Email == testEmail))
                {
                    var user = new User("Test User", testEmail, hasher.Hash("Test123!"));
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Database seeding failed in IntegrationTestStartup");
            }

            app.UseAuthentication();
            app.UseAuthorization();

            if (_env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }
    }
}

using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using KontoApi.Api.Middleware;
using KontoApi.Api.Validators;
using KontoApi.Application.Accounts;
using KontoApi.Application.Handlers;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Services;
using KontoApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;

    builder.Services.AddDbContext<KontoDbContext>(options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(KontoDbContext)));
    });
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddHealthChecks();
    builder.Host.UseSerilog((context, loggerConfiguration)
        => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionRequestValidator>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "KontoApi",
            Version = "v1",
            Description = "Personal Finance Management API"
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                                           ?? throw new InvalidOperationException("JWT Key not configured"))),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<IStatementParser, StatementParser>();
    builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ImportTransactionsHandler>();
    builder.Services.AddScoped<RegisterUserHandler>();
    builder.Services.AddScoped<LoginUserHandler>();
    builder.Services.AddScoped<AddTransactionHandler>();
    builder.Services.AddScoped<GetUserHandler>();
    builder.Services.AddScoped<GetBudgetHandler>();
    builder.Services.AddScoped<GetTransactionsHandler>();
    builder.Services.AddScoped<DeleteTransactionHandler>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
    builder.Services.AddScoped<CreateAccountHandler>();
    builder.Services.AddScoped<GetAccountsHandler>();
    builder.Services.AddScoped<DeleteAccountHandler>();


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

    var app = builder.Build();
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

    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KontoDbContext>();
        db.Database.Migrate();
    }
    catch (Exception e)
    {
        Log.Error(e, "An error occurred while migrating the database");
        throw;
    }


    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
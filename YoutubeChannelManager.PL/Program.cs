using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using YoutubeChannelManager.BLL.Behaviors;
using YoutubeChannelManager.BLL.Features.Channels.Queries.GetAllChannel;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.BLL.Repositories;
using YoutubeChannelManager.BLL.Service;
using YoutubeChannelManager.BLL.Validators;
using YoutubeChannelManager.DAL.Data;
using YoutubeChannelManager.DAL.Models;
using YoutubeChannelManager.PL.Middleware;


// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 30
    )
    .WriteTo.File(
        path: "Logs/error-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Error,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 90
    )
    .CreateLogger();

try
{
    Log.Information("Starting Youtube Channel Manager API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to the host
    builder.Host.UseSerilog();

    // Add Controllers
    builder.Services.AddControllers();

    // Add Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "Youtube Channel Manager API", 
            Version = "v1",
            Description = "API for managing YouTube channels"
        });
        
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });
        
        option.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    // Database Context
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Redis Cache Configuration
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        options.Configuration = redisConnection;
        options.InstanceName = "youtube_";
    });

    // Cache Service
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
  

    // FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<CreateChannelCommandValidator>();
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    // Identity
    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    }).AddEntityFrameworkStores<ApplicationDbContext>();

    // JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
        options.DefaultForbidScheme =
        options.DefaultScheme =
        options.DefaultSignInScheme =
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])
            )
        };
    });

    // MediatR
    builder.Services.AddMediatR(typeof(GetAllChannelsQuery).Assembly);

    // Repositories and Services
    builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<ITokenService, TokenService>();

    var app = builder.Build();

    // Serilog HTTP request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
        };
    });

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Youtube Channel Manager API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    // Custom Request Logging Middleware
    app.UseMiddleware<RequestLoggingMiddleware>();

    // Global Exception Handler
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is ValidationException validationException)
            {
                Log.Warning("Validation failed: {@Errors}", validationException.Errors);

                var errors = validationException.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                });

                await context.Response.WriteAsJsonAsync(new { Errors = errors });
            }
            else
            {
                Log.Error(exception, "Unhandled exception occurred");
                await context.Response.WriteAsJsonAsync(new { Error = exception?.Message });
            }
        });
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Application started successfully");
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

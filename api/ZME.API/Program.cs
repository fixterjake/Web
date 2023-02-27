using dotenv.net;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using VATSIM.Connect.AspNetCore.Server.Extensions;
using VATSIM.Connect.AspNetCore.Server.Options;
using ZME.API.Data;
using ZME.API.Extensions;
using ZME.API.Repositories;
using ZME.API.Services;
using ZME.API.Shared.Models;
using ZME.API.Validators;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Logging.AddSerilog(logger, dispose: true);

builder.WebHost.UseSentry(options =>
{
    options.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN") ??
        throw new ArgumentNullException("SENTRY_DSN env variable not found");
    options.TracesSampleRate = 1.0;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Format is the word Bearer, then a space, followed by the token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddVatsimConnect<AuthenticationService>(options =>
{
    options.VatsimTokenRequestOptions = new VatsimTokenRequestOptions
    {
        ClientId = Environment.GetEnvironmentVariable("CONNECT_CLIENT_ID") ??
            throw new ArgumentNullException("CONNECT_CLIENT_ID env variable not found"),
        ClientSecret = Environment.GetEnvironmentVariable("CONNECT_CLIENT_SECRET") ??
            throw new ArgumentNullException("CONNECT_CLIENT_SECRET env variable not found"),
        VatsimAuthUri = Environment.GetEnvironmentVariable("CONNECT_AUTH_URL") ??
            throw new ArgumentNullException("CONNECT_AUTH_URL env variable not found"),
        RedirectUri = Environment.GetEnvironmentVariable("CONNECT_REDIRECT_URL") ??
            throw new ArgumentNullException("CONNECT_REDIRECT_URL env variable not found")
    };
    options.JwtBearerConfig = new JwtBearerConfig
    {
        Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
            throw new ArgumentNullException("JWT_ISSUER env variable not found"),
        Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
            throw new ArgumentNullException("JWT_AUDIENCE env variable not found"),
        Secret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
            throw new ArgumentNullException("JWT_SECRET env variable not found"),
        AccessTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRATION") ??
            throw new ArgumentNullException("JWT_ACCESS_EXPIRATION env variable not found")),
        RefreshTokenExpiration = int.Parse(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRATION") ??
            throw new ArgumentNullException("JWT_REFRESH_EXPIRATION env variable not found")),
        ShortenClaimTypeNames = true
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddAuthPolicies();
});

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
        throw new ArgumentException("CONNECTION_STRING env variable not found"));
});

var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ??
    throw new ArgumentNullException("REDIS_HOST env variable not found");
builder.Services.AddSingleton(ConnectionMultiplexer.Connect(redisHost).GetDatabase());

builder.Services.AddScoped<IValidator<Airport>, AirportValidator>();
builder.Services.AddScoped<IValidator<Comment>, CommentValidator>();
builder.Services.AddScoped<IValidator<EventPosition>, EventPositionValidator>();
builder.Services.AddScoped<IValidator<EventRegistration>, EventRegistrationValidator>();
builder.Services.AddScoped<IValidator<Event>, EventValidator>();
builder.Services.AddScoped<IValidator<Faq>, FaqValidator>();

builder.Services.AddScoped<LoggingService>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<S3Service>();

builder.Services.AddScoped<AirportRepository>();
builder.Services.AddScoped<CommentRepository>();
builder.Services.AddScoped<EventRepository>();
builder.Services.AddScoped<FaqRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
    if (context != null && context.Database.GetMigrations().Any())
        context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors();
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseRouting();
app.UseSentryTracing();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

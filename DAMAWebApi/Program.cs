using System.Text;
using DAMA.Application.Interfaces;
using DAMA.Domain.Entities;
using DAMA.Infrastructure.Services;
using DAMA.Infrastructure.Setting; // Contains JwtSettings
using DAMA.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using DAMAWebApi.Middleware;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Bind the Jwt settings from appsettings.json and register it as a singleton
builder.Services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

// Register DbContext (using the connection string from appsettings.json)
builder.Services.AddDbContext<DamaContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<DamaContext>()
    .AddDefaultTokenProviders();

// Register RoleManager and SignInManager
builder.Services.AddScoped<RoleManager<Role>>();
builder.Services.AddScoped<SignInManager<User>>();

// Ensure JWT secret exists (this is validated when the JwtSettings are bound)
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
    throw new InvalidOperationException("JWT Secret is missing in configuration.");

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


    }).AddJwtBearer(options =>
    {

        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
       
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key), 
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer, 
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience, 
            ValidateLifetime = true, 
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("❌ No token found in Authorization header.");
                }
                else
                {
                    Console.WriteLine($"🔹 Received Token: {token}");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Authentication Failed: {context.Exception.Message}");
                if (context.Exception is SecurityTokenExpiredException)
                {
                    Console.WriteLine("⏳ Token has expired.");
                }
                else if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                {
                    Console.WriteLine("🔑 Token signature key is invalid or missing.");
                }
                else if (context.Exception is SecurityTokenInvalidIssuerException)
                {
                    Console.WriteLine("❌ Token issuer is incorrect.");
                }
                else if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    Console.WriteLine("🚫 Token audience is incorrect.");
                }
                else if (context.Exception is SecurityTokenValidationException)
                {
                    Console.WriteLine("🔄 Token validation failed for another reason.");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("⚠️ Unauthorized request - Token is missing or invalid.");
                return Task.CompletedTask;
            }
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("❌ No token found in Authorization header.");
                }
                else
                {
                    Console.WriteLine($"🔹 Received Token: {token}");
                }
                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Authentication Failed: {context.Exception.Message}");

                if (context.Exception is SecurityTokenExpiredException)
                {
                    Console.WriteLine("⏳ Token has expired.");
                }
                else if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                {
                    Console.WriteLine("🔑 Token signature key is invalid or missing.");
                }
                else if (context.Exception is SecurityTokenInvalidIssuerException)
                {
                    Console.WriteLine("❌ Token issuer is incorrect.");
                }
                else if (context.Exception is SecurityTokenInvalidAudienceException)
                {
                    Console.WriteLine("🚫 Token audience is incorrect.");
                }
                else if (context.Exception is SecurityTokenValidationException)
                {
                    Console.WriteLine("🔄 Token validation failed for another reason.");
                }

                return Task.CompletedTask;
            },

            OnChallenge = context =>
            {
                Console.WriteLine("⚠️ Unauthorized request - Token is missing or invalid.");
                return Task.CompletedTask;
            }
        };
    });
var requireAuthPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddAuthorizationBuilder()
    .SetDefaultPolicy(requireAuthPolicy);
// Add Authorization services (global policy)
builder.Services.AddAuthorization();

// Register HttpContextAccessor (for accessing HTTP context in services/middleware)
builder.Services.AddHttpContextAccessor();

// Register Application Services
builder.Services.AddScoped<IAuthService, AuthenticationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>(); // Make sure you have implemented ITokenService
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IPostService, PostService>();
// Add Controllers
builder.Services.AddControllers();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DAMA API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter your JWT token without the 'Bearer ' prefix.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

var app = builder.Build();

// OPTIONAL: Use a custom middleware if needed (ensure it's properly implemented)
 app.UseMiddleware<TokenValidationMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

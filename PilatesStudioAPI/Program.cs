using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PilatesStudioAPI.Configuration;
using PilatesStudioAPI.Data.Context;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Middleware;
using PilatesStudioAPI.Services.Interfaces;
using PilatesStudioAPI.Services.Implementations;
using Serilog;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddControllers();

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PilatesStudioDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use SQLite for development
        options.UseSqlite(connectionString);
    }
    else
    {
        // Use MySQL for production
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
});

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole<long>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PilatesStudioDbContext>()
.AddDefaultTokenProviders();

// Configure JWT settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = jwtSettings.Audience,
        ValidIssuer = jwtSettings.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("InstructorOnly", policy => policy.RequireRole("instructor", "admin"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("student", "instructor", "admin"));
});

// Configure CORS
var corsSettings = new CorsSettings();
builder.Configuration.GetSection("CorsSettings").Bind(corsSettings);
builder.Services.AddSingleton(corsSettings);

builder.Services.AddCors(options =>
{
    options.AddPolicy("PilatesStudioPolicy", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod();
              
        if (corsSettings.AllowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

// Configure Email settings
var emailSettings = new EmailSettings();
builder.Configuration.GetSection("EmailSettings").Bind(emailSettings);
builder.Services.AddSingleton(emailSettings);

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Pilates Studio API",
        Description = "API completa para gestión de estudio de pilates",
        Contact = new OpenApiContact
        {
            Name = "Pilates Studio",
            Email = "info@pilatesstudio.com"
        }
    });

    // Configure JWT authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Register Repositories
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IInstructorRepository, PilatesStudioAPI.Repositories.Implementations.InstructorRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IStudentRepository, PilatesStudioAPI.Repositories.Implementations.StudentRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IZoneRepository, PilatesStudioAPI.Repositories.Implementations.ZoneRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IClassRepository, PilatesStudioAPI.Repositories.Implementations.ClassRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IPlanRepository, PilatesStudioAPI.Repositories.Implementations.PlanRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.ISubscriptionRepository, PilatesStudioAPI.Repositories.Implementations.SubscriptionRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IReservationRepository, PilatesStudioAPI.Repositories.Implementations.ReservationRepository>();
builder.Services.AddScoped<PilatesStudioAPI.Repositories.Interfaces.IPaymentRepository, PilatesStudioAPI.Repositories.Implementations.PaymentRepository>();

// Register application services (repositories, services, etc.)
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IInstructorService, InstructorService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.IZoneService, PilatesStudioAPI.Services.Implementations.ZoneService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.IClassService, PilatesStudioAPI.Services.Implementations.ClassService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.IPlanService, PilatesStudioAPI.Services.Implementations.PlanService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.ISubscriptionService, PilatesStudioAPI.Services.Implementations.SubscriptionService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.IReservationService, PilatesStudioAPI.Services.Implementations.ReservationService>();
builder.Services.AddScoped<PilatesStudioAPI.Services.Interfaces.IPaymentService, PilatesStudioAPI.Services.Implementations.PaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Use Global Exception Handling
app.UseMiddleware<GlobalExceptionMiddleware>();

// Use Serilog request logging
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pilates Studio API v1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("PilatesStudioPolicy");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create database and seed roles
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PilatesStudioDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
    
    try
    {
        Log.Information("Ensuring database is created...");
        context.Database.EnsureCreated();
        Log.Information("Database check completed.");

        // Create default roles
        Log.Information("Creating default roles...");
        var roles = new[] { "admin", "instructor", "student" };
        
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<long>(roleName);
                await roleManager.CreateAsync(role);
                Log.Information("Role {Role} created successfully", roleName);
            }
        }
        
        Log.Information("Roles creation completed.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while ensuring database creation and role setup.");
    }
}

Log.Information("Starting Pilates Studio API...");
app.Run();

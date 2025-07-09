using Microsoft.EntityFrameworkCore;
using ExtraHours.API.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ExtraHours.API.Utils;
using ExtraHours.API.Repositories.Implementations;
using ExtraHours.API.Repositories.Interfaces;
using ExtraHours.API.Service.Implementations;
using ExtraHours.API.Service.Interface;
using ExtraHours.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar para leer variables de entorno
builder.Configuration.AddEnvironmentVariables();

try
{
    // Construir cadena de conexión desde variables de entorno o configuración
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        var dbHost = builder.Configuration["DB_HOST"];
        var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
        var dbName = builder.Configuration["DB_NAME"];
        var dbUser = builder.Configuration["DB_USER"];
        var dbPassword = builder.Configuration["DB_PASSWORD"];

        if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbName) ||
            string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
        {
            throw new InvalidOperationException("Faltan variables de entorno de base de datos: DB_HOST, DB_NAME, DB_USER, DB_PASSWORD");
        }

        connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode=Require;Trust Server Certificate=true";
    }

    // Configurar DbContext
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Registrar repositorios
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IExtraHourRepository, ExtraHourRepository>();
    builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
    builder.Services.AddScoped<IExtraHoursConfigRepository, ExtraHoursConfigRepository>();
    builder.Services.AddScoped<ColombianHolidayService>();

    // Registrar servicios
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();
    builder.Services.AddScoped<IExtraHourService, ExtraHourService>();
    builder.Services.AddSingleton<IJWTUtils, JWTUtils>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return new JWTUtils(configuration);
    });
    builder.Services.AddScoped<IExtraHoursConfigService, ExtraHoursConfigService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IExtraHourCalculationService, ExtraHourCalculationService>();
    builder.Services.AddScoped<ICompensationRequestService, CompensationRequestService>();

    // Configurar EmailService
    builder.Services.AddScoped<IEmailService, EmailService>();

    // Agregar controladores
    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

    // Configurar JWT con variables de entorno
    var jwtSecret = builder.Configuration["JWT_SECRET"] ?? builder.Configuration["JwtSettings:SecretKey"];
    var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? builder.Configuration["JwtSettings:Issuer"] ?? "ExtraHours.API";
    var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? builder.Configuration["JwtSettings:Audience"] ?? "ExtraHours.Client";

    if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
    {
        throw new InvalidOperationException("JWT_SECRET debe tener al menos 32 caracteres");
    }

    // Configurar autenticación con JWT
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ManagerOnly", policy => policy.RequireRole("manager"));
        options.AddPolicy("EmpleadoOnly", policy => policy.RequireRole("empleado"));
        options.AddPolicy("SuperusuarioOnly", policy => policy.RequireRole("superusuario"));
    });

    // Configurar CORS 
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(MyAllowSpecificOrigins, policy =>
        {
            if (builder.Environment.IsDevelopment())
            {
                // En desarrollo, usar configuración específica
                policy.WithOrigins(
                    "http://localhost:5173", // URL del frontend
                    "https://localhost:7086" // URL del backend
                    )
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
            else
            {
                // En producción, más permisivo
                policy.WithOrigins("https://gestor-horas-extra.vercel.app", "http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
        });
    });

    // Configurar Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ExtraHours API",
            Version = "v1",
            Description = "API para gestionar horas extra y usuarios.",
            Contact = new OpenApiContact
            {
                Name = "Jaime Gallo",
                Email = "jagallob@eafit.edu.co",
            }
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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

    var app = builder.Build();

    // Configurar middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ExtraHours API v1");
            options.RoutePrefix = "swagger";
        });
    }
    else
    {
        // En producción, también habilitar Swagger para testing
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ExtraHours API v1");
            options.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseCors(MyAllowSpecificOrigins);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Endpoints de salud para verificar que funciona
    app.MapGet("/", () => "ExtraHours API is running!");
    app.MapGet("/health", () => "OK");

    // Aplicar migraciones automáticamente
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var migrationLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            migrationLogger.LogInformation("Aplicando migraciones de base de datos...");
            await context.Database.MigrateAsync();
            migrationLogger.LogInformation("Migraciones aplicadas exitosamente");
        }
        catch (Exception ex)
        {
            migrationLogger.LogError(ex, "Error aplicando migraciones: {Message}", ex.Message);
            // No lanzar excepción aquí para que la app siga funcionando
        }
    }

    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("ExtraHours API iniciada correctamente");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error crítico al iniciar la aplicación: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");

    // Log adicional para debugging
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
    }

    throw;
}
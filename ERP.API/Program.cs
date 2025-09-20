using ERPSys.SharedKernel.Utils;
using Finance.Application.Mapping;
using Finance.Infrastructure.Extensions;
using Finance.Infrastructure.Persistence.DBContext;
using Finance.Infrastructure.Persistence.DBContext.Seed;
using Finance.Presentation;
using HR.Infrastructure.HR.DbContext;
using HR.Infrastructure.HR.Extensions;
using HR.Infrastructure.MappingProfiles;
using HR.Presentation;
using Logging.Application.Interfaces;
using Logging.Infrastructure.DbContext;
using Logging.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Users.Infrastructure.Config;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Services;
using Users.Presentation;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// ✅ Register DbContexts + Services
// =========================================
builder.Services.AddHRServices(builder.Configuration);
builder.Services.AddFinanceServices(builder.Configuration);

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDB")));

builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoggingDb")));

// ✅ Application Mappings
FinanceMappingConfig.RegisterMappings();
builder.Services.AddAutoMapper(typeof(HRProfile).Assembly);

// ✅ Core Services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddHttpContextAccessor();

// =========================================
// ✅ Controllers (HR + Users + Finance)
// =========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddApplicationPart(typeof(HRAssemblyReference).Assembly)
    .AddApplicationPart(typeof(UsersAssemblyReference).Assembly)
    .AddApplicationPart(typeof(FinancePresentationAssemblyReference).Assembly)
    .AddControllersAsServices();

// =========================================
// ✅ Messaging (InMemory or RabbitMQ)
// =========================================

// ✅ Messaging with Outbox, RabbitMQ, InMemory, Hybrid
builder.Services.AddMessagingHrWithFinance(builder.Configuration);


// =========================================
// ✅ JWT Authentication
// =========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = ClaimTypes.Role
    };
});

// =========================================
// ✅ Authorization
// =========================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission:User.Create", policy =>
        policy.RequireClaim("Permission", "User.Create"));
    options.AddPolicy("Permission:User.View", policy =>
        policy.RequireClaim("Permission", "User.View"));
});

// =========================================
// ✅ CORS Policy
// =========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();

// =========================================
// ✅ Swagger with JWT + File Upload
// =========================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ERP API", Version = "v1" });
    c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

    // ✅ File Upload Support
    c.OperationFilter<FileUploadOperationFilter>();
});


var app = builder.Build();

// =========================================
// ✅ Debug: Show loaded controllers
// =========================================
var controllerFeature = new ControllerFeature();
app.Services.GetRequiredService<ApplicationPartManager>().PopulateFeature(controllerFeature);
foreach (var controller in controllerFeature.Controllers)
{
    Console.WriteLine($"🔎 Loaded Controller: {controller.FullName}");
}

// =========================================
// ✅ DB Seeding and Migration
// =========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logService = services.GetRequiredService<ILoggingService>();
    // HR
    var hrContext = services.GetRequiredService<HRDbContext>();
    await hrContext.Database.MigrateAsync();
    await HRDbContextSeed.SeedAsync(hrContext);

    // Users
    var usersContext = services.GetRequiredService<UsersDbContext>();
    await usersContext.Database.MigrateAsync();
    await UsersDbSeed.SeedAsync(usersContext);

    // Finance
    var financeContext = services.GetRequiredService<FinanceDbContext>();
    await financeContext.Database.MigrateAsync();

    // ✅ Pass HR context → Finance seed
    FinanceDbSeed.Seed(financeContext, hrContext, logService);
}

// =========================================
// ✅ Middlewares
// =========================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP API v1");
        options.RoutePrefix = "swagger";
    });
}


app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

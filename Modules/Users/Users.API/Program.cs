using ERPSys.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Users.Infrastructure.Config;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// ✅ DbContext + Services
// =========================================
builder.Services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UsersDB")));
builder.Services.AddScoped<TokenService>();

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
// ✅ Authorization Policies
// =========================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission:User.Create", policy =>
        policy.RequireClaim("Permission", "User.Create"));
    options.AddPolicy("Permission:User.View", policy =>
        policy.RequireClaim("Permission", "User.View"));
});

// =========================================
// ✅ CORS
// =========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// =========================================
// ✅ Swagger
// =========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Users API", Version = "v1" });
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
});

var app = builder.Build();

// =========================================
// ✅ Middlewares
// =========================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Users API v1");
    });
}

// ✅ DB Migration & Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var usersContext = services.GetRequiredService<UsersDbContext>();
    await usersContext.Database.MigrateAsync();
    await UsersDbSeed.SeedAsync(usersContext);
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Controllers
app.MapControllers();
app.MapGet("/", () => "Users API running ✅");

app.Run();

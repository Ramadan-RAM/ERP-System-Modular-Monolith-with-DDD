using HR.Infrastructure.HR.DbContext;
using HR.Infrastructure.HR.Extensions;
using HR.Infrastructure.MappingProfiles;
using Logging.Application.Interfaces;
using Logging.Infrastructure.DbContext;
using Logging.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// =========================================
// ✅ Services Registration
// =========================================
builder.Services.AddHRServices(builder.Configuration);

builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoggingDb")));

builder.Services.AddAutoMapper(typeof(HRProfile).Assembly);
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddHttpContextAccessor();

AppContext.SetSwitch("Microsoft.AspNetCore.Mvc.SuppressOutputFormatterBuffering", true);

// =========================================
// ✅ Controllers + JSON Config
// =========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// =========================================
// ✅ Swagger
// =========================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "HR API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR API v1");
    });
}

// =========================================
// ✅ DB Seeding and Migration
// =========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var hrContext = services.GetRequiredService<HRDbContext>();
    await hrContext.Database.MigrateAsync();
    await HRDbContextSeed.SeedAsync(hrContext);
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => "HR API running ✅");

app.Run();

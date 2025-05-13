using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Application.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using UserManagementAPI.Application.Validators;
using UserManagementAPI.Application.Managers.User;
using UserManagementAPI.Application.Managers.User.Implementation;
using UserManagementAPI.Api.Middleware.Implementation;
using UserManagementAPI.Api.Services.User;
using UserManagementAPI.Api.Services.User.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=userprofile.db",
        b => b.MigrationsAssembly("UserManagementAPI.Application"))
);

// Add controllers
builder.Services.AddControllers();

builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation(); // Enables automatic server-side validation
builder.Services.AddValidatorsFromAssemblyContaining<BaseModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleValidator>();

// Add Managers
builder.Services.AddScoped<IUserProfileManager, UserProfileManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();

// Add Services
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Add CORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllDev",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

var app = builder.Build();

// Apply migrations at startup (creates DB and schema if they don't exist and migrations are set up)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate(); // Applies pending migrations. Creates the database if it doesn't exist.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.UseExceptionHandler(options => options.UseMiddleware<ExceptionMiddleware>());

app.UseHttpsRedirection();

app.UseRouting();

// Apply the CORS policy. This should come after UseRouting and before
app.UseCors("AllowAllDev");

app.MapControllers();

// Configure swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
});

app.Run();

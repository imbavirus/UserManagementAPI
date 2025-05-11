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
        b => b.MigrationsAssembly("UserManagementAPIApplication"))
);

// Add controllers
builder.Services.AddControllers();

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

var app = builder.Build();

app.UseRouting();
app.MapControllers();

// Configure swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Profile API V1");
});

app.UseExceptionHandler(options => options.UseMiddleware<ExceptionMiddleware>());

app.UseHttpsRedirection();

app.Run();

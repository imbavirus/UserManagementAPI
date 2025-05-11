using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using UserProfileBackend.Application.Validators;
using UserProfileBackend.Application.Managers.User;
using UserProfileBackend.Application.Managers.User.Implementation;
using UserProfileBackend.Api.Middleware.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=userprofile.db",
        b => b.MigrationsAssembly("UserProfileBackendApplication"))
);

// Add controllers
builder.Services.AddControllers();

// Register FluentValidation
builder.Services.AddFluentValidationAutoValidation(); // Enables automatic server-side validation
builder.Services.AddValidatorsFromAssemblyContaining<BaseModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleValidator>();

builder.Services.AddScoped<IUserProfileManager, UserProfileManager>();

var app = builder.Build();

// Configure swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Profile API V1");
});

app.UseExceptionHandler(options => options.UseMiddleware<ExceptionMiddleware>());

app.UseHttpsRedirection();

app.Run();

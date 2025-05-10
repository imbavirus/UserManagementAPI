using Microsoft.EntityFrameworkCore;
using UserProfileBackend.Application.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSwaggerGen(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=userprofile.db",
        b => b.MigrationsAssembly("UserProfileBackendApplication"))
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // You can customize the Swagger UI endpoint here if needed
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Profile API V1");
});

app.UseHttpsRedirection();

app.Run();

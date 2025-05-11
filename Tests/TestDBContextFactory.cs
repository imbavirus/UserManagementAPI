using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Application.Data;

namespace UserManagementAPI.Tests;

public class TestDbContextFactory : IDisposable
{
    private SqliteConnection? _connection;

    public AppDbContext CreateContext()
    {
        // Using "DataSource=:memory:" creates an in-memory SQLite database.
        // It's crucial to keep the connection open for the lifetime of the DbContext
        // when using in-memory SQLite, otherwise the database is deleted when the connection closes.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    public void Dispose() => _connection?.Dispose(); // Close and dispose the connection
}
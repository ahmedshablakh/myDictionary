using Microsoft.EntityFrameworkCore;
using MyDictionary.Infrastructure.Data;

namespace MyDictionary.Mobile.Services;

public class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;
    private static bool _initialized = false;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Add new columns if they don't exist (for existing databases)
            await AddColumnIfNotExists(context, "Words", "ExampleTranslation", "TEXT");
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            throw;
        }
    }

    private async Task AddColumnIfNotExists(ApplicationDbContext context, string tableName, string columnName, string columnType)
    {
        try
        {
            // Check if column exists
            var checkSql = $"SELECT COUNT(*) FROM pragma_table_info('{tableName}') WHERE name='{columnName}'";
            var connection = context.Database.GetDbConnection();
            await connection.OpenAsync();
            
            using var command = connection.CreateCommand();
            command.CommandText = checkSql;
            var result = await command.ExecuteScalarAsync();
            
            if (Convert.ToInt32(result) == 0)
            {
                // Column doesn't exist, add it
                var alterSql = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
                await context.Database.ExecuteSqlRawAsync(alterSql);
                System.Diagnostics.Debug.WriteLine($"Added column {columnName} to {tableName}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding column {columnName}: {ex.Message}");
            // Don't throw, just log - column might already exist
        }
    }
}

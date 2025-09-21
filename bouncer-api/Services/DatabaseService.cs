using Microsoft.Data.Sqlite;
using Bouncer.Api.Models;

namespace Bouncer.Api.Services;

public interface IDatabaseService
{
    Task InitializeDatabaseAsync();
    Task<IEnumerable<Principal>> GetAllPrincipalsAsync();
    Task<Principal?> GetPrincipalByIdAsync(int id);
    Task<Principal?> GetPrincipalByExternalIdAsync(string externalId);
    Task<Principal> CreatePrincipalAsync(string externalId);
    Task<Principal?> UpdatePrincipalAsync(int id, string externalId);
    Task<bool> DeletePrincipalAsync(int id);
}

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        var databasePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "database.sqlite");
        _connectionString = $"Data Source={databasePath}";
        _logger = logger;

        // Ensure the data directory exists
        var dataDirectory = Path.GetDirectoryName(databasePath);
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory!);
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        _logger.LogInformation("Initializing database...");

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createTablesCommand = connection.CreateCommand();
        createTablesCommand.CommandText = @"
            -- Create Principal table
            CREATE TABLE IF NOT EXISTS Principal (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ExternalId TEXT NOT NULL UNIQUE,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );

            -- Create Bundle table
            CREATE TABLE IF NOT EXISTS Bundle (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );

            -- Create Feature table
            CREATE TABLE IF NOT EXISTS Feature (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );

            -- Create BundleFeature junction table
            CREATE TABLE IF NOT EXISTS BundleFeature (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BundleId INTEGER NOT NULL,
                FeatureId INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (BundleId) REFERENCES Bundle(Id) ON DELETE CASCADE,
                FOREIGN KEY (FeatureId) REFERENCES Feature(Id) ON DELETE CASCADE,
                UNIQUE(BundleId, FeatureId)
            );

            -- Create License table
            CREATE TABLE IF NOT EXISTS License (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ClientKey TEXT NOT NULL UNIQUE,
                PrivateKey TEXT NOT NULL UNIQUE,
                Assignee TEXT NOT NULL,
                Expiration TEXT,
                Status INTEGER NOT NULL DEFAULT 1,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT
            );

            -- Create LicenseFeature junction table
            CREATE TABLE IF NOT EXISTS LicenseFeature (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LicenseId INTEGER NOT NULL,
                FeatureId INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (LicenseId) REFERENCES License(Id) ON DELETE CASCADE,
                FOREIGN KEY (FeatureId) REFERENCES Feature(Id) ON DELETE CASCADE,
                UNIQUE(LicenseId, FeatureId)
            );

            -- Create PrincipalLicense junction table
            CREATE TABLE IF NOT EXISTS PrincipalLicense (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PrincipalId INTEGER NOT NULL,
                LicenseId INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                FOREIGN KEY (PrincipalId) REFERENCES Principal(Id) ON DELETE CASCADE,
                FOREIGN KEY (LicenseId) REFERENCES License(Id) ON DELETE CASCADE,
                UNIQUE(PrincipalId, LicenseId)
            );

            -- Create indexes for better performance
            CREATE INDEX IF NOT EXISTS IX_Principal_ExternalId ON Principal(ExternalId);
            CREATE INDEX IF NOT EXISTS IX_License_ClientKey ON License(ClientKey);
            CREATE INDEX IF NOT EXISTS IX_License_Status ON License(Status);
            CREATE INDEX IF NOT EXISTS IX_BundleFeature_BundleId ON BundleFeature(BundleId);
            CREATE INDEX IF NOT EXISTS IX_BundleFeature_FeatureId ON BundleFeature(FeatureId);
            CREATE INDEX IF NOT EXISTS IX_LicenseFeature_LicenseId ON LicenseFeature(LicenseId);
            CREATE INDEX IF NOT EXISTS IX_LicenseFeature_FeatureId ON LicenseFeature(FeatureId);
            CREATE INDEX IF NOT EXISTS IX_PrincipalLicense_PrincipalId ON PrincipalLicense(PrincipalId);
            CREATE INDEX IF NOT EXISTS IX_PrincipalLicense_LicenseId ON PrincipalLicense(LicenseId);
        ";

        await createTablesCommand.ExecuteNonQueryAsync();
        _logger.LogInformation("Database initialized successfully");
    }

    public async Task<IEnumerable<Principal>> GetAllPrincipalsAsync()
    {
        var principals = new List<Principal>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ExternalId, CreatedAt, UpdatedAt FROM Principal ORDER BY Id";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            principals.Add(new Principal
            {
                Id = reader.GetInt32(0), // Id
                ExternalId = reader.GetString(1), // ExternalId
                CreatedAt = DateTime.Parse(reader.GetString(2)), // CreatedAt
                UpdatedAt = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)) // UpdatedAt
            });
        }

        return principals;
    }

    public async Task<Principal?> GetPrincipalByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ExternalId, CreatedAt, UpdatedAt FROM Principal WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Principal
            {
                Id = reader.GetInt32(0), // Id
                ExternalId = reader.GetString(1), // ExternalId
                CreatedAt = DateTime.Parse(reader.GetString(2)), // CreatedAt
                UpdatedAt = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)) // UpdatedAt
            };
        }

        return null;
    }

    public async Task<Principal?> GetPrincipalByExternalIdAsync(string externalId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, ExternalId, CreatedAt, UpdatedAt FROM Principal WHERE ExternalId = @externalId";
        command.Parameters.AddWithValue("@externalId", externalId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Principal
            {
                Id = reader.GetInt32(0), // Id
                ExternalId = reader.GetString(1), // ExternalId
                CreatedAt = DateTime.Parse(reader.GetString(2)), // CreatedAt
                UpdatedAt = reader.IsDBNull(3) ? null : DateTime.Parse(reader.GetString(3)) // UpdatedAt
            };
        }

        return null;
    }

    public async Task<Principal> CreatePrincipalAsync(string externalId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createdAt = DateTime.UtcNow.ToString("O");

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Principal (ExternalId, CreatedAt)
            VALUES (@externalId, @createdAt);
            SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("@externalId", externalId);
        command.Parameters.AddWithValue("@createdAt", createdAt);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());

        return new Principal
        {
            Id = id,
            ExternalId = externalId,
            CreatedAt = DateTime.Parse(createdAt)
        };
    }

    public async Task<Principal?> UpdatePrincipalAsync(int id, string externalId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var updatedAt = DateTime.UtcNow.ToString("O");

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Principal
            SET ExternalId = @externalId, UpdatedAt = @updatedAt
            WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@externalId", externalId);
        command.Parameters.AddWithValue("@updatedAt", updatedAt);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
        {
            return null;
        }

        return await GetPrincipalByIdAsync(id);
    }

    public async Task<bool> DeletePrincipalAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Principal WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
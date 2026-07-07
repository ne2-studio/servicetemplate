using Dapper;
using Npgsql;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra;

public class PostgresWidgetRepository(string connectionString) : IWidgetRepository
{
    public async Task<bool> ExistsByNameAsync(string name)
    {
        using var connection = new NpgsqlConnection(connectionString);
        return await connection.ExecuteScalarAsync<bool>(
            "SELECT EXISTS(SELECT 1 FROM \"Widgets\" WHERE \"Name\" = @Name)",
            new { Name = name });
    }

    public async Task SaveAsync(Widget widget)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            INSERT INTO ""Widgets"" (""Id"", ""Name"", ""CreatedAt"", ""UpdatedAt"")
            VALUES (@Id, @Name, @CreatedAt, @UpdatedAt)";

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                widget.Id,
                widget.Name,
                widget.CreatedAt,
                widget.UpdatedAt
            });
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
        {
            throw new InvalidOperationException("Widget name already exists.", ex);
        }
    }

    public async Task UpdateAsync(Widget widget)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            UPDATE ""Widgets""
            SET ""Name"" = @Name, ""UpdatedAt"" = @UpdatedAt
            WHERE ""Id"" = @Id";

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                widget.Id,
                widget.Name,
                widget.UpdatedAt
            });
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
        {
            throw new InvalidOperationException("Widget name already exists.", ex);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE FROM \"Widgets\" WHERE \"Id\" = @Id", new { Id = id });
    }

    public async Task<Widget?> LoadByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            SELECT ""Id"", ""Name"", ""CreatedAt"", ""UpdatedAt""
            FROM ""Widgets""
            WHERE ""Id"" = @Id";

        return await connection.QuerySingleOrDefaultAsync<Widget>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Widget>> ListAsync(int skip = 0, int take = 10)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            SELECT ""Id"", ""Name"", ""CreatedAt"", ""UpdatedAt""
            FROM ""Widgets""
            ORDER BY ""CreatedAt"" DESC
            LIMIT @Take OFFSET @Skip";

        return await connection.QueryAsync<Widget>(sql, new { Skip = skip, Take = take });
    }
}

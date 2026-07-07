using Dapper;
using Npgsql;
using ServiceTemplate.Ports.Output;

namespace ServiceTemplate.Infra;

public class PostgresTaskRepository(string connectionString) : ITaskRepository
{
    public async Task SaveAsync(TaskItem task)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            INSERT INTO ""Tasks"" (""Id"", ""Title"", ""CreatedAt"")
            VALUES (@Id, @Title, @CreatedAt)";

        await connection.ExecuteAsync(sql, new
        {
            task.Id,
            task.Title,
            task.CreatedAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.ExecuteAsync("DELETE FROM \"Tasks\" WHERE \"Id\" = @Id", new { Id = id });
    }

    public async Task<TaskItem?> LoadByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            SELECT ""Id"", ""Title"", ""CreatedAt""
            FROM ""Tasks""
            WHERE ""Id"" = @Id";

        return await connection.QuerySingleOrDefaultAsync<TaskItem>(sql, new { Id = id });
    }

    public async Task<IEnumerable<TaskItem>> ListAsync(int skip = 0, int take = 10)
    {
        using var connection = new NpgsqlConnection(connectionString);
        var sql = @"
            SELECT ""Id"", ""Title"", ""CreatedAt""
            FROM ""Tasks""
            ORDER BY ""CreatedAt"" DESC
            LIMIT @Take OFFSET @Skip";

        return await connection.QueryAsync<TaskItem>(sql, new { Skip = skip, Take = take });
    }
}

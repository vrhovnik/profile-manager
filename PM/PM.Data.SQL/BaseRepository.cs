using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Dapper.Contrib.Extensions;
using PM.Core;
using PM.Interfaces;

namespace PM.Data.SQL;

public abstract class BaseRepository<TEntity>(string connectionString) : IDataRepository<TEntity>
    where TEntity : class
{
    private readonly SqlConnection sqlConnection = new(connectionString);

    protected async Task<IDbConnection> GetConnection()
    {
        if (sqlConnection.State == ConnectionState.Closed)
            await sqlConnection.OpenAsync();
        return sqlConnection;
    }

    public virtual Task<PaginatedList<TEntity>> SearchAsync(int page, int pageSize, string query) =>
        throw new NotImplementedException();

    public virtual async Task<List<TEntity>> GetAsync()
    {
        using var connection = await GetConnection();
        var result = await connection.GetAllAsync<TEntity>();
        return result.ToList();
    }

    public virtual Task<bool> DeleteAsync(int entityId) => throw new NotImplementedException();
    public virtual Task<bool> UpdateAsync(TEntity entity) => throw new NotImplementedException();
    public virtual Task<TEntity> InsertAsync(TEntity entity) => throw new NotImplementedException();
    public virtual Task<TEntity> DetailsAsync(int entityId) => throw new NotImplementedException();

    public Task<bool> IsAlive()
    {
        try
        {
           return Task.FromResult(sqlConnection.State == ConnectionState.Open);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return Task.FromResult(false);
        }
        finally
        {
            if (sqlConnection.State == ConnectionState.Open)
                sqlConnection.Close();
        }
    }
}
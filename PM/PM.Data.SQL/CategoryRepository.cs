using Dapper;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Data.SQL;

public class CategoryRepository(string connectionString)
    : BaseRepository<Category>(connectionString), ICategoryRepository
{
    public override async Task<List<Category>> GetAsync()
    {
        using var connection = await GetConnection();
        var sql = "SELECT U.CategoryId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM Categories U " +
                  "LEFT JOIN Profile2Categories P ON U.CategoryId = P.CategoryId " +
                  "GROUP BY U.CategoryId, U.Name, U.Description ORDER BY U.Name";

        var result = await connection.QueryAsync<Category>(sql);
        return result.ToList();
    }

    public override async Task<bool> DeleteAsync(int entityId)
    {
        using var connection = await GetConnection();
        //delete from the join table first
        await connection.ExecuteAsync("DELETE FROM dbo.Profile2Categories WHERE CategoryId=@catId",
            new { catId = entityId });
        var result = await connection.ExecuteAsync("DELETE FROM dbo.Categories C WHERE C.CategoryId=@catId",
            new { catId = entityId });
        return result > 0;
    }

    public override async Task<Category> DetailsAsync(int entityId)
    {
        using var connection = await GetConnection();
        var foundCategory = await connection.QuerySingleOrDefaultAsync<Category>(
            "SELECT U.CategoryId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM Categories U " +
            "LEFT JOIN Profile2Categories P ON U.CategoryId = P.CategoryId " +
            " WHERE U.CategoryId=@entityId "+
            "GROUP BY U.CategoryId, U.Name, U.Description ORDER BY U.Name", new { entityId });
        return foundCategory;
    }

    public override async Task<PaginatedList<Category>> SearchAsync(int page, int pageSize, string query)
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.CategoryId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM Categories U " +
            "LEFT JOIN Profile2Categories P ON U.CategoryId = P.CategoryId ";

        if (!string.IsNullOrEmpty(query))
            sql += "WHERE C.Name LIKE '%' + @query + '%' ";

        sql += "GROUP BY U.CategoryId, U.Name, U.Description ORDER BY U.Name";
        var result = await connection.QueryAsync<Category>(sql, new { query });
        return PaginatedList<Category>.Create(result.ToList(), page, pageSize, query);
    }

    public override async Task<Category> InsertAsync(Category entity)
    {
        using var connection = await GetConnection();
        var item = await connection.ExecuteScalarAsync<int>(
            "INSERT INTO dbo.Categories(Name, Description)VALUES(@name,@desc);SELECT CAST(SCOPE_IDENTITY() as bigint)",
            new { name = entity.Name, decs = entity.Description });
        entity.CategoryId = item;
        return entity;
    }
}
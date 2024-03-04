using Dapper;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Data.SQL;

public class ProfileTypesRepository(string connectionString)
    : BaseRepository<ProfileType>(connectionString), IProfileTypeRepository
{
    public override async Task<List<ProfileType>> GetAsync()
    {
        using var connection = await GetConnection();
        var sql = "SELECT U.ProfileTypeId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM ProfileTypes U " +
                  "LEFT JOIN Profiles P ON U.ProfileTypeId = P.ProfileTypeId " +
                  "GROUP BY U.ProfileTypeId, U.Name, U.Description ORDER BY U.Name";

        var result = await connection.QueryAsync<ProfileType>(sql);
        return result.ToList();
    }

    public override async Task<bool> DeleteAsync(int entityId)
    {
        using var connection = await GetConnection();
        //delete from the join table first
        await connection.ExecuteAsync("DELETE FROM dbo.Profiles WHERE ProfileTypeId=@pti",
            new { pti = entityId });
        var result = await connection.ExecuteAsync("DELETE FROM dbo.ProfileTypes C WHERE C.ProfileTypeId=@pti",
            new { pti = entityId });
        return result > 0;
    }

    public override async Task<ProfileType> DetailsAsync(int entityId)
    {
        using var connection = await GetConnection();
        var foundCategory = await connection.QuerySingleOrDefaultAsync<ProfileType>(
            "SELECT U.ProfileTypeId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM ProfileTypes U " +
            "LEFT JOIN Profiles P ON U.ProfileTypeId = P.ProfileTypeId " +
            " WHERE U.ProfileTypeId=@entityId "+
            "GROUP BY U.ProfileTypeId, U.Name, U.Description ORDER BY U.Name", new { entityId });
        return foundCategory;
    }

    public override async Task<PaginatedList<ProfileType>> SearchAsync(int page, int pageSize, string query)
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileTypeId, U.Name, U.Description, count(P.ProfileId) as COUNT FROM ProfileTypes U " +
            "LEFT JOIN Profiles P ON U.ProfileTypeId = P.ProfileTypeId ";

        if (!string.IsNullOrEmpty(query))
            sql += "WHERE U.Name LIKE '%' + @query + '%' ";

        sql += "GROUP BY U.ProfileTypeId, U.Name, U.Description ORDER BY U.Name";
        var result = await connection.QueryAsync<ProfileType>(sql, new { query });
        return PaginatedList<ProfileType>.Create(result.ToList(), page, pageSize, query);
    }

    public override async Task<bool> UpdateAsync(ProfileType entity)
    {
        using var connection = await GetConnection();
        var result = await connection.ExecuteAsync(
            "UPDATE dbo.ProfileTypes SET Name=@name, Description=@desc WHERE ProfileTypeId=@id",
            new { name = entity.Name, desc = entity.Description, id = entity.ProfileTypeId });
        return result > 0;
    }

    public override async Task<ProfileType> InsertAsync(ProfileType entity)
    {
        using var connection = await GetConnection();
        var item = await connection.ExecuteScalarAsync<int>(
            "INSERT INTO dbo.ProfileTypes(Name, Description)VALUES(@name,@desc);SELECT CAST(SCOPE_IDENTITY() as bigint)",
            new { name = entity.Name, desc = entity.Description });
        entity.ProfileTypeId = item;
        return entity;
    }
}
using Dapper;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Data.SQL;

public class ProfileItemTypesRepository(string connectionString)
    : BaseRepository<ProfileItemType>(connectionString), IProfileItemTypesRepository
{
    public override async Task<List<ProfileItemType>> GetAsync()
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileItemTypeId, U.Name, U.Description, count(P.ProfileItemId) as COUNT FROM ProfileItemTypes U " +
            "JOIN ProfileItems P ON U.ProfileItemTypeId = P.ProfileItemTypeId " +
            "GROUP BY U.ProfileItemTypeId, U.Name, U.Description ORDER BY U.Name";

        var result = await connection.QueryAsync<ProfileItemType>(sql);
        return result.ToList();
    }

    public override async Task<bool> DeleteAsync(int entityId)
    {
        using var connection = await GetConnection();
        await connection.ExecuteAsync("DELETE FROM dbo.Profile2ProfileItems P " +
                                      "JOIN ProfileItemTypes T on T.ProfileItemId=P.ProfileItemId " +
                                      "WHERE T.ProfileItemTypeId=@pti", new { pti = entityId });
        await connection.ExecuteAsync("DELETE FROM dbo.ProfileItems " +
                                      "WHERE ProfileItemTypeId=@pti", new { pti = entityId });
        var result = await connection.ExecuteAsync("DELETE FROM dbo.ProfileItemTypes C WHERE C.ProfileItemTypeId=@pti",
            new { pti = entityId });
        return result > 0;
    }

    public override async Task<ProfileItemType> DetailsAsync(int entityId)
    {
        using var connection = await GetConnection();
        var profileItemType = await connection.QuerySingleOrDefaultAsync<ProfileItemType>( 
            "SELECT U.ProfileItemTypeId, U.Name, U.Description, count(P.ProfileItemId) as COUNT " +
                  "FROM ProfileItemTypes U LEFT JOIN ProfileItems P ON U.ProfileItemTypeId = P.ProfileItemTypeId " +
                  "WHERE U.ProfileItemTypeId=@entityId " +
                  "GROUP BY U.ProfileItemTypeId, U.Name, U.Description ORDER BY U.Name", new { entityId });
        return profileItemType;
    }

    public override async Task<PaginatedList<ProfileItemType>> SearchAsync(int page, int pageSize, string query)
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileItemTypeId, U.Name, U.Description, count(P.ProfileItemId) as COUNT FROM ProfileItemTypes U " +
            "LEFT JOIN ProfileItems P ON U.ProfileItemTypeId = P.ProfileItemTypeId ";
            
        if (!string.IsNullOrEmpty(query))
            sql += "WHERE U.Name LIKE '%' + @query + '%' ";

        sql += "GROUP BY U.ProfileItemTypeId, U.Name, U.Description ORDER BY U.Name";
        var result = await connection.QueryAsync<ProfileItemType>(sql, new { query });
        return PaginatedList<ProfileItemType>.Create(result.ToList(), page, pageSize, query);
    }

    public override async Task<ProfileItemType> InsertAsync(ProfileItemType entity)
    {
        using var connection = await GetConnection();
        var item = await connection.ExecuteScalarAsync<int>(
            "INSERT INTO dbo.ProfileItemTypes(Name, Description)VALUES(@name,@desc);SELECT CAST(SCOPE_IDENTITY() as bigint)",
            new { name = entity.Name, desc = entity.Description });
        entity.ProfileItemTypeId = item;
        return entity;
    }

    public override async Task<bool> UpdateAsync(ProfileItemType entity)
    {
        using var connection = await GetConnection();
        var result = await connection.ExecuteAsync(
            "UPDATE dbo.ProfileItemTypes SET Name=@name, Description=@desc WHERE ProfileItemTypeId=@id",
            new { name = entity.Name, desc = entity.Description, id = entity.ProfileItemTypeId });
        return result > 0;
    }
}
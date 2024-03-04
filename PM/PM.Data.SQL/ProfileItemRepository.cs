using Dapper;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Data.SQL;

public class ProfileItemRepository(string connectionString)
    : BaseRepository<ProfileItem>(connectionString), IProfileItemRepository
{
    public override async Task<List<ProfileItem>> GetAsync()
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileItemId, U.Name, U.Description,U.Line,U.LineContent, " +
            "P.ProfileItemTypeId,P.Name,P.Description,G.ProfileId,G.Name, G.Description," +
            "G.ProfileTypeId,G.DateCreated FROM ProfileItems U " +
            "JOIN ProfileItemTypes P ON U.ProfileItemTypeId = P.ProfileItemTypeId " +
            "LEFT JOIN Profile2ProfileItems M ON U.ProfileItemId = M.ProfileItemId " +
            "LEFT JOIN Profiles G ON M.ProfileId = G.ProfileId";

        var grid = await connection.QueryMultipleAsync(sql);
        var lookup = new Dictionary<int, ProfileItem>();

        grid.Read<ProfileItem, ProfileItemType, Profile, ProfileItem>((profileItem, itemType, profile) =>
        {
            profileItem.ItemType = itemType;
            if (!lookup.TryGetValue(profileItem.ProfileItemId, out _))
                lookup.Add(profileItem.ProfileItemId, profileItem);

            if (profile == null) return profileItem;
            lookup[profileItem.ProfileItemId].Profiles.Add(profile);

            return profileItem;
        }, splitOn: "ProfileItemTypeId,ProfileId");
        return lookup.Values.ToList();
    }

    public override async Task<bool> DeleteAsync(int entityId)
    {
        using var connection = await GetConnection();
        await connection.ExecuteAsync("DELETE FROM dbo.Profile2ProfileItems WHERE ProfileItemId=@pti",
            new { pti = entityId });
        var result = await connection.ExecuteAsync("DELETE FROM dbo.ProfileItems WHERE ProfileItemId=@pti",
            new { pti = entityId });
        return result > 0;
    }

    public override async Task<ProfileItem> DetailsAsync(int entityId)
    {
        using var connection = await GetConnection();
        var sql = "SELECT U.ProfileItemId, U.Name, U.Description, U.Line, U.LineContent, U.ProfileItemTypeId  " +
                  "FROM ProfileItems U WHERE U.ProfileItemId=@entityId;" +
                  "SELECT P.ProfileItemTypeId, P.Name,P.Description FROM ProfileItemTypes P " +
                  "JOIN ProfileItems U ON P.ProfileItemTypeId = U.ProfileItemTypeId WHERE U.ProfileItemId=@entityId;" +
                  "SELECT G.ProfileId, G.Name, G.Description, G.ProfileTypeId, G.DateCreated FROM Profiles G " +
                  "JOIN Profile2ProfileItems M ON G.ProfileId = M.ProfileId WHERE M.ProfileItemId=@entityId";
        var profileItemsReader = await connection.QueryMultipleAsync(sql, new { entityId });
        var profileItem = profileItemsReader.Read<ProfileItem>().FirstOrDefault() ?? new();
        profileItem.ItemType = profileItemsReader.Read<ProfileItemType>().FirstOrDefault() ?? new();
        profileItem.Profiles = profileItemsReader.Read<Profile>().ToList();
        return profileItem;
    }

    public override async Task<PaginatedList<ProfileItem>> SearchAsync(int page, int pageSize, string query)
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileItemId, U.Name, U.Description,U.Line,U.LineContent, " +
            "P.ProfileItemTypeId,P.Name,P.Description,G.ProfileId,G.Name, G.Description," +
            "G.ProfileTypeId,G.DateCreated FROM ProfileItems U " +
            "JOIN ProfileItemTypes P ON U.ProfileItemTypeId = P.ProfileItemTypeId " +
            "LEFT JOIN Profile2ProfileItems M ON U.ProfileItemId = M.ProfileItemId " +
            "LEFT JOIN Profiles G ON M.ProfileId = G.ProfileId ";

        if (!string.IsNullOrEmpty(query))
            sql += "WHERE U.Name LIKE '%' + @query + '%' ";

        var grid = await connection.QueryMultipleAsync(sql);
        var lookup = new Dictionary<int, ProfileItem>();

        grid.Read<ProfileItem, ProfileItemType, Profile, ProfileItem>((profileItem, itemType, profile) =>
        {
            profileItem.ItemType = itemType;
            if (!lookup.TryGetValue(profileItem.ProfileItemId, out _))
                lookup.Add(profileItem.ProfileItemId, profileItem);

            profileItem.Profiles ??= new List<Profile>();

            if (profile == null) return profileItem;
            lookup[profileItem.ProfileItemId].Profiles.Add(profile);

            return profileItem;
        }, splitOn: "ProfileItemTypeId,ProfileId");
        var result = lookup.Values.ToList();
        return PaginatedList<ProfileItem>.Create(result, page, pageSize, query);
    }

    public override async Task<ProfileItem> InsertAsync(ProfileItem entity)
    {
        using var connection = await GetConnection();
        var item = await connection.ExecuteScalarAsync<int>(
            "INSERT INTO dbo.ProfileItems(Name, Description, Line, LineContent, ProfileItemTypeId)VALUES(@name,@desc,@line,@lineCount,@profileItemTypeId);" +
            "SELECT CAST(SCOPE_IDENTITY() as bigint)",
            new
            {
                name = entity.Name,
                desc = entity.Description,
                line = entity.Line,
                lineCount = entity.LineContent,
                profileItemTypeId = entity.ItemType.ProfileItemTypeId
            });
        entity.ProfileItemId = item;
        return entity;
    }

    public override async Task<bool> UpdateAsync(ProfileItem entity)
    {
        using var connection = await GetConnection();
        var result = await connection.ExecuteAsync(
            "UPDATE dbo.ProfileItems SET Name=@name, Description=@desc, Line=@line, " +
            "LineContent=@lineCount, ProfileItemTypeId=@profileItemTypeId WHERE ProfileItemId=@id",
            new
            {
                name = entity.Name,
                desc = entity.Description,
                line = entity.Line,
                lineCount = entity.LineContent,
                profileItemTypeId = entity.ItemType.ProfileItemTypeId,
                id = entity.ProfileItemId
            });
        return result > 0;
    }
}
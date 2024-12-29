using Dapper;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Data.SQL;

public class ProfileRepository(string connectionString) : BaseRepository<Profile>(connectionString), IProfileRepository
{
    public override async Task<List<Profile>> GetAsync()
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileId, U.Name, U.Description, U.ProfileTypeId, U.DateCreated," +
            "P.ProfileTypeId,P.Name,P.Description, G.Name, G.Description, G.Line, G.LineContent, G.ProfileItemTypeId, " +
            "C.Name,C.Description " +
            "FROM Profiles U JOIN ProfileTypes P ON U.ProfileTypeId = P.ProfileTypeId " +
            "LEFT JOIN Profile2ProfileItems M ON U.ProfileId = M.ProfileId " +
            "LEFT JOIN ProfileItems G ON M.ProfileItemId = G.ProfileItemId " + 
            "LEFT JOIN Profile2Categories Z ON Z.ProfileId = U.ProfileId "+
            "LEFT JOIN Categories C ON Z.CategoryId = C.CategoryId ";

        var grid = await connection.QueryMultipleAsync(sql);
        var lookup = new Dictionary<int, Profile>();

        grid.Read<Profile, ProfileType, ProfileItem, Category, Profile>(
            (profile, itemType, currentProfileItem, category) =>
            {
                profile.Type = itemType;
                if (!lookup.TryGetValue(profile.ProfileId, out _))
                    lookup.Add(profile.ProfileId, profile);

                if (currentProfileItem == null)
                {
                    if (category == null) return profile;
                    lookup[profile.ProfileId].Categories.Add(category); 
                    
                    return profile;
                }
                lookup[profile.ProfileId].Items.Add(currentProfileItem);
                
                if (category == null) return profile;
                
                lookup[profile.ProfileId].Categories.Add(category);
                return profile;
            }, splitOn: "ProfileTypeId,ProfileItemId,CategoryId");
        return lookup.Values.ToList();
    }

    public override async Task<bool> DeleteAsync(int entityId)
    {
        using var connection = await GetConnection();
        await connection.ExecuteAsync("DELETE FROM dbo.Profile2ProfileItems WHERE ProfileId=@pti",
            new { pti = entityId });
        await connection.ExecuteAsync("DELETE FROM dbo.Profile2Categories WHERE ProfileId=@pti",
            new { pti = entityId });
        var result = await connection.ExecuteAsync("DELETE FROM dbo.Profiles WHERE ProfileId=@pti",
            new { pti = entityId });
        return result > 0;
    }

    public override async Task<Profile> DetailsAsync(int entityId)
    {
        using var connection = await GetConnection();
        var sql = "SELECT U.ProfileId, U.Name, U.Description, U.ProfileTypeId, U.DateCreated  " +
                  "FROM Profiles U WHERE U.ProfileId=@entityId;" +
                  "SELECT P.ProfileTypeId, P.Name,P.Description FROM ProfileTypes P " +
                  "JOIN Profiles U ON P.ProfileTypeId = U.ProfileTypeId WHERE U.ProfileId=@entityId;" +
                  "SELECT G.ProfileItemId, G.Name, G.Description, G.Line, G.LineContent FROM ProfileItems G " +
                  "JOIN Profile2ProfileItems M ON G.ProfileId = M.ProfileId WHERE M.ProfileId=@entityId;" +
                  "SELECT C.Name,C.Description FROM Categories C " +
                  "JOIN Profile2Categories Z ON Z.CategoryId = C.CategoryId WHERE Z.ProfileId=@entityId";
        var profileItemsReader = await connection.QueryMultipleAsync(sql, new { entityId });
        var profileItem = profileItemsReader.Read<Profile>().FirstOrDefault() ?? new();
        profileItem.Type = profileItemsReader.Read<ProfileType>().FirstOrDefault() ?? new();
        profileItem.Items = profileItemsReader.Read<ProfileItem>().ToList();
        profileItem.Categories = profileItemsReader.Read<Category>().ToList();
        return profileItem;
    }

    public override async Task<PaginatedList<Profile>> SearchAsync(int page, int pageSize, string query)
    {
        using var connection = await GetConnection();
        var sql =
            "SELECT U.ProfileId, U.Name, U.Description, U.ProfileTypeId, U.DateCreated," +
            "P.ProfileTypeId,P.Name,P.Description, G.Name, G.Description, G.Line, G.LineContent, G.ProfileItemTypeId, " +
            "C.Name,C.Description " +
            "FROM Profiles U JOIN ProfileTypes P ON U.ProfileTypeId = P.ProfileTypeId " +
            "LEFT JOIN Profile2ProfileItems M ON U.ProfileId = M.ProfileId " +
            "LEFT JOIN ProfileItems G ON M.ProfileItemId = G.ProfileItemId " + 
            "LEFT JOIN Profile2Categories Z ON Z.ProfileId = U.ProfileId "+
            "LEFT JOIN Categories C ON Z.CategoryId = C.CategoryId ";

        if (!string.IsNullOrEmpty(query))
            sql += "WHERE U.Name LIKE '%' + @query + '%' ";

        var grid = await connection.QueryMultipleAsync(sql);
        var lookup = new Dictionary<int, Profile>();

        grid.Read<Profile, ProfileType, ProfileItem, Category, Profile>(
            (profile, itemType, currentProfileItem, category) =>
            {
                profile.Type = itemType;
                if (!lookup.TryGetValue(profile.ProfileId, out _))
                    lookup.Add(profile.ProfileId, profile);

                if (currentProfileItem == null)
                {
                    if (category == null) return profile;
                    lookup[profile.ProfileId].Categories.Add(category); 
                    
                    return profile;
                }
                lookup[profile.ProfileId].Items.Add(currentProfileItem);
                
                if (category == null) return profile;
                    
                lookup[profile.ProfileId].Categories.Add(category);

                return profile;
            }, splitOn: "ProfileTypeId,ProfileItemId,CategoryId");
        var result = lookup.Values.ToList();
        return PaginatedList<Profile>.Create(result, page, pageSize, query);
    }

    public override async Task<Profile> InsertAsync(Profile entity)
    {
        using var connection = await GetConnection();
        var item = await connection.ExecuteScalarAsync<int>(
            "INSERT INTO dbo.Profile(Name, Description,ProfileTypeId,DateCreated)VALUES(@name,@desc,@ptid,@dc);" +
            "SELECT CAST(SCOPE_IDENTITY() as bigint)",
            new
            {
                name = entity.Name,
                desc = entity.Description,
                ptid = entity.Type.ProfileTypeId,
                dc = DateTime.Now
            });
        entity.ProfileId = item;

        foreach (var category in entity.Categories)
        {
            await connection.ExecuteAsync(
                "INSERT INTO dbo.Profile2Categories(ProfileId,CategoryId)VALUES(@pid,@cid)",
                new { pid = entity.ProfileId, cid = category.CategoryId });
        }

        return entity;
    }

    public override async Task<bool> UpdateAsync(Profile entity)
    {
        using var connection = await GetConnection();
        var result = await connection.ExecuteAsync(
            "UPDATE dbo.Profiles SET Name=@name, Description=@desc, ProfileTypeId=@ptid WHERE ProfileItemId=@id",
            new
            {
                name = entity.Name,
                desc = entity.Description,
                ptid = entity.Type.ProfileTypeId,
                id = entity.ProfileId
            });
        //todo: lines, categories update - delete all and re-add them
        return result > 0;
    }
}
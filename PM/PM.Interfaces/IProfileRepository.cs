using PM.Models;

namespace PM.Interfaces;

public interface IProfileRepository : IDataRepository<Profile>
{
    Task<string> DownloadAsync(string profileTypeId);
}
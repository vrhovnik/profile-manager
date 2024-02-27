using PM.Models;

namespace PM.Interfaces;

public interface ISettingsService
{
    Task<bool> UpdateAsync(Settings settings);
    Task<Settings> GetAsync(string settingsId);
}
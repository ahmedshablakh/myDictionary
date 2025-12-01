using MyDictionary.Core.Entities;

namespace MyDictionary.Core.Interfaces;

public interface IUserSettingsRepository
{
    Task<UserSettings> GetSettingsAsync();
    Task UpdateSettingsAsync(UserSettings settings);
}

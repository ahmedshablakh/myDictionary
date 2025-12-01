using Microsoft.EntityFrameworkCore;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Infrastructure.Data;

namespace MyDictionary.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly ApplicationDbContext _context;

    public UserSettingsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserSettings> GetSettingsAsync()
    {
        var settings = await _context.UserSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new UserSettings { Id = 1 };
            _context.UserSettings.Add(settings);
            await _context.SaveChangesAsync();
        }
        return settings;
    }

    public async Task UpdateSettingsAsync(UserSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _context.Entry(settings).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}

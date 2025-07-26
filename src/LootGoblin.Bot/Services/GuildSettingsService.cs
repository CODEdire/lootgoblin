using Microsoft.Extensions.Caching.Memory;

namespace LootGoblin.Bot.Services;

/// <summary>
/// Service interface for managing and retrieving guild settings.
/// </summary>
public interface IGuildSettingsService
{
    /// <summary>
    /// Sets the loot channel ID for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="channelId">The channel ID to set, or null to clear the setting.</param>
    Task SetLootChannelAsync(ulong guildId, ulong? channelId);

    /// <summary>
    /// Sets the event channel ID for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="channelId">The channel ID to set, or null to clear the setting.</param>
    Task SetEventChannelAsync(ulong guildId, ulong? channelId);

    /// <summary>
    /// Sets the event organizer role ID for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="roleId">The role ID to set, or null to clear the setting.</param>
    Task SetEventOrganizerRoleAsync(ulong guildId, ulong? roleId);

    /// <summary>
    /// Sets the event participant role ID for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="roleId">The role ID to set, or null to clear the setting.</param>
    Task SetEventParticipantRoleAsync(ulong guildId, ulong? roleId);

    /// <summary>
    /// Gets the <see cref="GuildSettings"/> for the specified guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <returns>The <see cref="GuildSettings"/> instance for the guild.</returns>
    Task<GuildSettings> GetGuildSettingsAsync(ulong guildId);
}

/// <summary>
/// Service for managing and retrieving guild settings, with caching support.
/// </summary>
public class GuildSettingsService : IGuildSettingsService
{
    private readonly EventDbContext _db;  // EF Core DB context for the bot
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildSettingsService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for guild events and settings.</param>
    /// <param name="memoryCache">The memory cache for caching guild settings.</param>
    public GuildSettingsService(EventDbContext dbContext, IMemoryCache memoryCache)
    {
        _db = dbContext;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Ensures a <see cref="GuildSettings"/> record exists for the given guild, creating one if needed.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <returns>The <see cref="GuildSettings"/> instance for the guild.</returns>
    private async Task<GuildSettings> GetOrCreateGuildSettingsAsync(ulong guildId)
    {
        var settings = await _db.GuildSettings.FindAsync(guildId);
        if (settings == null)
        {
            settings = new GuildSettings { Id = guildId };
            _db.GuildSettings.Add(settings);
        }
        return settings;
    }

    /// <inheritdoc />
    public async Task SetLootChannelAsync(ulong guildId, ulong? channelId)
    {
        var settings = await GetOrCreateGuildSettingsAsync(guildId);
        settings.LootChannelId = channelId;  // null means clearing the setting
        await SaveChangesAsync();
        _memoryCache.Remove(CacheConstants.GetGuildSettingsCacheKey(guildId));
    }

    /// <inheritdoc />
    public async Task SetEventChannelAsync(ulong guildId, ulong? channelId)
    {
        var settings = await GetOrCreateGuildSettingsAsync(guildId);
        settings.EventChannelId = channelId;
        await SaveChangesAsync();
        _memoryCache.Remove(CacheConstants.GetGuildSettingsCacheKey(guildId));
    }

    /// <inheritdoc />
    public async Task SetEventOrganizerRoleAsync(ulong guildId, ulong? roleId)
    {
        var settings = await GetOrCreateGuildSettingsAsync(guildId);
        settings.EventOrganizerRoleId = roleId;
        await SaveChangesAsync();
        _memoryCache.Remove(CacheConstants.GetGuildSettingsCacheKey(guildId));
    }

    /// <inheritdoc />
    public async Task SetEventParticipantRoleAsync(ulong guildId, ulong? roleId)
    {
        var settings = await GetOrCreateGuildSettingsAsync(guildId);
        settings.EventParticipantRoleId = roleId;
        await SaveChangesAsync();
        _memoryCache.Remove(CacheConstants.GetGuildSettingsCacheKey(guildId));
    }

    /// <inheritdoc />
    public async Task<GuildSettings> GetGuildSettingsAsync(ulong guildId)
    {
        var settings = await _db.GuildSettings.FindAsync(guildId);
        return settings ?? new GuildSettings { Id = guildId }; //Return an empty instance if none exist
    }

    /// <summary>
    /// Saves changes to the database, handling update exceptions.
    /// </summary>
    /// <exception cref="Exception">
    /// Thrown when a database update fails, with a user-friendly error message.
    /// </exception>
    private async Task SaveChangesAsync()
    {
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception (not shown) and throw a user-friendly error
            throw new Exception("Database update failed while saving guild settings. Please try again.", ex);
        }
    }
}

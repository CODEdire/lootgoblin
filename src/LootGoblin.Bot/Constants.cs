namespace LootGoblin.Bot;

/// <summary>
/// Provides constants and helper methods for caching within the LootGoblin bot.
/// </summary>
public static class CacheConstants
{
    /// <summary>
    /// Gets the cache key string for storing or retrieving <see cref="GuildSettings"/> for a specific guild.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <returns>A string representing the cache key for the guild's settings.</returns>
    public static string GetGuildSettingsCacheKey(ulong guildId) => $"guild-settings-{guildId}";
}

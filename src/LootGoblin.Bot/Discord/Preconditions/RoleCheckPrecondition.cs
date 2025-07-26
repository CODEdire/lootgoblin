using Microsoft.Extensions.Caching.Memory;

namespace LootGoblin.Bot.Discord.Preconditions;

/// <summary>
/// Abstract precondition attribute that checks if a user has a specific role in a guild before allowing command execution.
/// </summary>
/// <typeparam name="TContext">The type of the command context, which must implement <see cref="IUserContext"/> and <see cref="IGuildContext"/>.</typeparam>
public abstract class RoleCheckPrecondition<TContext> : PreconditionAttribute<TContext>
    where TContext : IUserContext, IGuildContext
{
    private readonly Func<GuildSettings, ulong?> _roleSelector;
    private readonly string _roleType;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoleCheckPrecondition{TContext}"/> class.
    /// </summary>
    /// <param name="roleSelector">A function to select the required role ID from <see cref="GuildSettings"/>.</param>
    /// <param name="roleType">A string describing the type of role required (used in error messages).</param>
    protected RoleCheckPrecondition(Func<GuildSettings, ulong?> roleSelector, string roleType)
    {
        _roleSelector = roleSelector;
        _roleType = roleType;
    }

    /// <summary>
    /// Ensures that the user in the provided context can execute the command, based on their roles in the guild.
    /// </summary>
    /// <param name="context">The command context containing user and guild information.</param>
    /// <param name="serviceProvider">The service provider for dependency resolution.</param>
    /// <returns>
    /// A <see cref="PreconditionResult"/> indicating success if the user has the required role, or failure with a message otherwise.
    /// </returns>
    public override async ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var db = serviceProvider.GetRequiredService<EventDbContext>();
        var cache = serviceProvider.GetRequiredService<IMemoryCache>();

        ulong? guildId = context.Guild?.Id;

        if (!guildId.HasValue)
            return PreconditionResult.Fail($"Guild could not be found.");

        // Try cache
        var settings = await cache.GetOrCreateAsync(CacheConstants.GetGuildSettingsCacheKey(guildId.Value), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await db.GuildSettings.AsNoTracking().FirstOrDefaultAsync(gs => gs.Id == guildId);
        });

        if (settings == null)
            return PreconditionResult.Fail("Guild settings not found.");

        var requiredRoleId = _roleSelector(settings);
        if (requiredRoleId == null)
        {
            // No restriction
            return PreconditionResult.Success;
        }

        // Check if user has the role
        if (context.User is GuildInteractionUser guildUser)
        {
            if (guildUser.RoleIds.Contains(requiredRoleId.Value))
                return PreconditionResult.Success;
        }

        return PreconditionResult.Fail($"You must have the {_roleType} role to use this command.");
    }
}
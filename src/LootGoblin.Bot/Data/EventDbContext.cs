namespace LootGoblin.Bot.Data;

/// <summary>
/// Represents the Entity Framework Core database context for guild events, loot piles, participants, and related entities.
/// </summary>
/// <remarks>
/// This context provides access to all core entities used by the LootGoblin bot for event and loot management.
/// </remarks>
public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for guild settings.
    /// </summary>
    public DbSet<GuildSettings> GuildSettings => Set<GuildSettings>();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for guild events.
    /// </summary>
    public DbSet<GuildEvent> GuildEvents => Set<GuildEvent>();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for loot piles.
    /// </summary>
    public DbSet<LootPile> LootPiles => Set<LootPile>();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for event participants.
    /// </summary>
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for participant sessions.
    /// </summary>
    public DbSet<ParticipantSession> ParticipantSessions => Set<ParticipantSession>();

    /// <summary>
    /// Configures the entity mappings and relationships for the context.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventDbContext).Assembly);
    }

    /// <summary>
    /// Retrieves a list of <see cref="GuildEvent"/> entities for a specified guild, optionally filtered by event status.
    /// </summary>
    /// <param name="guildId">The unique identifier of the guild.</param>
    /// <param name="filter">An optional filter for the event status.</param>
    /// <returns>
    /// A read-only list of <see cref="GuildEvent"/> entities, ordered by creation date descending.
    /// </returns>
    public async Task<IReadOnlyList<GuildEvent>> GetGuildEventsForGuildAsync(ulong guildId, EventStatus? filter = null)
    {
        var query = GuildEvents
            .AsQueryable()
            .Where(e => e.GuildId == guildId);

        if (filter.HasValue)
            query = query.Where(e => e.CurrentState == filter.Value);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
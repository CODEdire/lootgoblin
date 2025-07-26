namespace LootGoblin.Bot.Data.Entities;

/// <summary>
/// Entity class representing settings for a guild in the LootGoblin bot.
/// </summary>
public class GuildSettings
{
    /// <summary>
    /// Gets or sets the unique identifier for the guild.
    /// </summary>
    public ulong Id { get; set; }

    /// <summary>
    /// Gets or sets the ID of the channel where event messages are sent.
    /// </summary>
    /// <remarks>If null, we use the channel from the command</remarks>
    public ulong? EventChannelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the channel where loot messages are sent.
    /// </summary>
    /// <remarks>If null, we use the channel from the event message</remarks>
    public ulong? LootChannelId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the role that is required for event organizers.
    /// </summary>
    /// <remarks>If null, anyone can set up or manage an event</remarks>
    public ulong? EventOrganizerRoleId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the role that is required for event participants.
    /// </summary>
    /// <remarks>If null, anyone can participate in an event</remarks>
    public ulong? EventParticipantRoleId { get; set; }
}

/// <summary>
/// Configures the <see cref="GuildSettings"/> entity for Entity Framework Core.
/// </summary>
public class GuildSettingsConfiguration : IEntityTypeConfiguration<GuildSettings>
{
    /// <summary>
    /// Configures the entity properties and relationships for <see cref="GuildSettings"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<GuildSettings> builder)
    {
        // Set the table name
        builder.ToTable("GuildSettings");

        // Set the primary key
        builder.HasKey(gs => gs.Id);

        // Configure the Id property
        builder.Property(gs => gs.Id)
            .ValueGeneratedNever() // Guild IDs are assigned externally (e.g., by Discord)
            .IsRequired();

        // Configure optional channel and role IDs
        builder.Property(gs => gs.EventChannelId)
            .IsRequired(false);

        builder.Property(gs => gs.LootChannelId)
            .IsRequired(false);

        builder.Property(gs => gs.EventOrganizerRoleId)
            .IsRequired(false);

        builder.Property(gs => gs.EventParticipantRoleId)
            .IsRequired(false);
    }
}
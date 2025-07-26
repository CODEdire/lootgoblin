namespace LootGoblin.Bot.Data.Entities;

/// <summary>
/// Represents a session of a participant in a guild event.
/// </summary>
public class ParticipantSession
{
    /// <summary>
    /// Gets the unique identifier for the participant session.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets or sets the identifier of the event this session is associated with.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the event participant associated with this session, if any.
    /// </summary>
    public int? EventParticipantId { get; set; }

    /// <summary>
    /// Gets or sets the Discord user ID of the participant.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Gets or sets the Discord channel ID where the session took place.
    /// </summary>
    public ulong ChannelId { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session started.
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session ended, if available.
    /// </summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>
    /// Gets or sets the event this session is associated with.
    /// </summary>
    public virtual GuildEvent Event { get; set; } = default!;
}

/// <summary>
/// Configures the <see cref="ParticipantSession"/> entity for Entity Framework Core.
/// </summary>
public class ParticipantSessionConfiguration : IEntityTypeConfiguration<ParticipantSession>
{
    /// <summary>
    /// Configures the <see cref="ParticipantSession"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<ParticipantSession> builder)
    {
        // Table name
        builder.ToTable("ParticipantSessions");

        // Primary key
        builder.HasKey(ps => ps.Id);

        // Properties
        builder.Property(ps => ps.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(ps => ps.EventId)
            .IsRequired();

        builder.Property(ps => ps.EventParticipantId)
            .IsRequired(false);

        builder.Property(ps => ps.UserId)
            .IsRequired();

        builder.Property(ps => ps.ChannelId)
            .IsRequired();

        builder.Property(ps => ps.StartedAt)
            .IsRequired();

        builder.Property(ps => ps.EndedAt)
            .IsRequired(false);
    }
}
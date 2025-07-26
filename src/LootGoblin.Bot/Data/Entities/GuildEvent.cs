namespace LootGoblin.Bot.Data.Entities;

/// <summary>
/// Entity class representing a guild event in the LootGoblin bot.
/// </summary>
public class GuildEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for the event.
    /// </summary>
    public int Id { get; private set; } //TODO: Possibly obfuscate by using a short ID conversion?

    /// <summary>
    /// Gets or sets the guild the event belongs to.
    /// </summary>
    public ulong GuildId { get; set; }

    /// <summary>
    /// Gets or sets the associated message ID for the event to manage. Null means it has not been posted yet.
    /// </summary>
    public ulong? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the channel where the event originated. Null means it has not been posted yet.
    /// </summary>
    public ulong? OriginChannelId { get; set; }

    // Event Information

    /// <summary>
    /// Gets or sets a name for the event.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets a description for the event message.
    /// </summary>
    public string? Description { get; set; }

    // Tracking/Auditing

    /// <summary>
    /// Gets or sets the date and time when the event was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event was started.
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the event was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the creator of the event.
    /// </summary>
    public ulong CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the user who started the event.
    /// </summary>
    public ulong? StartedBy { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the user who completed the event.
    /// </summary>
    public ulong? CompletedBy { get; set; }

    // Event Settings

    /// <summary>
    /// Gets or sets a minimum time needed to participate in the event, in minutes.
    /// </summary>
    public int? MinimumParticipantMinutes { get; set; }

    /// <summary>
    /// Gets or sets a maximum number of participants that can join the event.
    /// </summary>
    public int? MaximumParticipants { get; set; }

    // Event State

    /// <summary>
    /// Gets or sets the current state of the event.
    /// </summary>
    public EventStatus CurrentState { get; set; }

    // Relations

    /// <summary>
    /// Gets or sets the collection of participant channels for this event.
    /// </summary>
    public virtual ICollection<GuildEventChannel> ParticipantChannels { get; set; } = [];

    /// <summary>
    /// Gets or sets associated piles of loot for this event.
    /// </summary>
    public virtual ICollection<LootPile> LootPiles { get; set; } = [];

    /// <summary>
    /// Gets or sets a collection of participant sessions for this event.
    /// </summary>
    public virtual ICollection<ParticipantSession> ParticipantSessions { get; set; } = [];

    /// <summary>
    /// Gets or sets a collection of event participants for this event.
    /// </summary>
    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = [];
}

/// <summary>
/// Represents a channel associated with a guild event.
/// </summary>
public class GuildEventChannel //This is kind of a cheat as we need a class for JSON column types. It does not support value types directly.
{
    //TODO: If we need channel specific settings/overrides, we can add them here.

    /// <summary>
    /// Gets or sets the unique identifier of the channel.
    /// </summary>
    public ulong ChannelId { get; set; }
}

/// <summary>
/// Configures the <see cref="GuildEvent"/> entity for Entity Framework Core.
/// </summary>
public class GuildEventConfiguration : IEntityTypeConfiguration<GuildEvent>
{
    /// <summary>
    /// Configures the entity properties and relationships for <see cref="GuildEvent"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<GuildEvent> builder)
    {
        // Set the table name
        builder.ToTable("GuildEvents");

        // Set the primary key
        builder.HasKey(e => e.Id);

        // Configure the Id property
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Configure the GuildId property
        builder.Property(e => e.GuildId)
            .IsRequired();

        // Configure the MessageId property
        builder.Property(e => e.MessageId)
            .IsRequired(false);

        // Configure the OriginChannelId property
        builder.Property(e => e.OriginChannelId)
            .IsRequired(false);

        // Configure the Name property
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(256);

        // Configure the Description property
        builder.Property(e => e.Description)
            .HasMaxLength(2048);

        // Configure the MinimumParticipantMinutes property
        builder.Property(e => e.MinimumParticipantMinutes)
            .IsRequired(false);

        // Configure the MaximumParticipants property
        builder.Property(e => e.MaximumParticipants)
            .IsRequired(false);

        // Configure the CurrentState property
        builder.Property(e => e.CurrentState)
            .IsRequired();

        // Configure the CreatedAt property
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Configure the StartedAt property
        builder.Property(e => e.StartedAt)
            .IsRequired(false);

        // Configure the CompletedAt property
        builder.Property(e => e.CompletedAt)
            .IsRequired(false);

        // Configure the CreatedBy property
        builder.Property(e => e.CreatedBy)
            .IsRequired();

        // Configure the StartedBy property
        builder.Property(e => e.StartedBy)
            .IsRequired(false);

        // Configure the CompletedBy property
        builder.Property(e => e.CompletedBy)
            .IsRequired(false);

        // Configure the ParticipantChannels property as JSON
        builder.OwnsMany(
            p => p.ParticipantChannels,
            detailsBuilder => detailsBuilder.ToJson()
        );

        builder.HasMany(e => e.LootPiles)
            .WithOne(lp => lp.Event)
            .HasForeignKey(lp => lp.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ParticipantSessions)
            .WithOne(ps => ps.Event)
            .HasForeignKey(ps => ps.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EventParticipants)
            .WithOne(ep => ep.Event)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
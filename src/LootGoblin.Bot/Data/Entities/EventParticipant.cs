namespace LootGoblin.Bot.Data.Entities;
//TODO: Turn this into a Table By Type entity that can be used for different loot type calculations so we can keep track of loot rolls and such.
//Alternatively, we can create a JSON Column with the loot type properties for the event participant.


/// <summary>
/// Represents a participant in a guild event, including participation details and loot eligibility.
/// </summary>
public class EventParticipant
{
    /// <summary>
    /// Gets the unique identifier for the event participant.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets or sets the identifier of the event this participant is associated with.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the Discord user ID of the participant.
    /// </summary>
    public ulong UserId { get; set; }

    /// <summary>
    /// Gets or sets the total participation time of the user in the event.
    /// </summary>
    public TimeSpan TotalParticipation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the participant is excluded from loot calculations.
    /// </summary>
    public bool ExcludedFromLoot { get; set; }

    /// <summary>
    /// Gets or sets the event this participant is associated with.
    /// </summary>
    public GuildEvent Event { get; set; } = default!;
}

/// <summary>
/// Configures the <see cref="EventParticipant"/> entity for Entity Framework Core.
/// </summary>
public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
{
    /// <summary>
    /// Configures the <see cref="EventParticipant"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<EventParticipant> builder)
    {
        // Table name
        builder.ToTable("EventParticipants");

        // Primary key
        builder.HasKey(ep => ep.Id);

        // Properties
        builder.Property(ep => ep.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(ep => ep.EventId)
            .IsRequired();

        builder.Property(ep => ep.UserId)
            .IsRequired();

        builder.Property(ep => ep.TotalParticipation)
            .IsRequired();

        builder.Property(ep => ep.ExcludedFromLoot)
            .IsRequired();
    }
}
namespace LootGoblin.Bot.Data.Entities;

/// <summary>
/// Entity class representing a loot pile in the LootGoblin bot.
/// </summary>
public class LootPile
{
    /// <summary>
    /// Gets the unique identifier for the loot pile.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets or sets the event ID this loot pile belongs to.
    /// </summary>
    public int EventId { get; set; }

    /// <summary>
    /// Gets or sets the associated message ID for the loot to manage. Null means it has not been posted yet.
    /// </summary>
    public ulong? MessageId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the channel where the loot originated. Null means it has not been posted yet.
    /// </summary>
    public ulong? OriginChannelId { get; set; }

    /// <summary>
    /// Gets or sets a name for the loot pile.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description for the loot pile.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of the loot pile.
    /// </summary>
    public LootStatus CurrentStatus { get; set; }

    /// <summary>
    /// Gets or sets the type of loot pile.
    /// </summary>
    public LootType LootType { get; set; }

    /// <summary>
    /// Gets or sets the roll type for the loot pile.
    /// </summary>
    public LootRollType RollType { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the loot pile was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the loot pile was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the creator of the loot pile.
    /// </summary>
    public ulong CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the user who completed the loot pile.
    /// </summary>
    public ulong? CompletedBy { get; set; }

    //TODO: This is where we need to set up the "Rules" for the loot pile.
    //If shared rules, point to a shared rule table

    //Relations

    /// <summary>
    /// Gets or sets the associated event for this loot pile.
    /// </summary>
    public GuildEvent Event { get; set; } = default!;
}

/// <summary>
/// Configures the <see cref="LootPile"/> entity for Entity Framework Core.
/// </summary>
public class LootPileConfiguration : IEntityTypeConfiguration<LootPile>
{
    /// <summary>
    /// Configures the entity properties and relationships for <see cref="LootPile"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LootPile> builder)
    {
        // Set the table name
        builder.ToTable("LootPiles");

        // Set the primary key
        builder.HasKey(lp => lp.Id);

        // Configure the Id property
        builder.Property(lp => lp.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        // Configure the EventId property (foreign key)
        builder.Property(lp => lp.EventId)
            .IsRequired();

        // Configure the MessageId property
        builder.Property(lp => lp.MessageId)
            .IsRequired(false);

        // Configure the OriginChannelId property
        builder.Property(lp => lp.OriginChannelId)
            .IsRequired(false);

        // Configure the Name property
        builder.Property(lp => lp.Name)
            .IsRequired()
            .HasMaxLength(256);

        // Configure the Description property
        builder.Property(lp => lp.Description)
            .HasMaxLength(2048);

        // Configure the CurrentStatus property
        builder.Property(lp => lp.CurrentStatus)
            .IsRequired();

        // Configure the LootType property
        builder.Property(lp => lp.LootType)
            .IsRequired();

        // Configure the RollType property
        builder.Property(lp => lp.RollType)
            .IsRequired();

        // Configure the CreatedAt property
        builder.Property(lp => lp.CreatedAt)
            .IsRequired();

        // Configure the CompletedAt property
        builder.Property(lp => lp.CompletedAt)
            .IsRequired(false);

        // Configure the CreatedBy property
        builder.Property(lp => lp.CreatedBy)
            .IsRequired();

        // Configure the CompletedBy property
        builder.Property(lp => lp.CompletedBy)
            .IsRequired(false);

        // Configure the relationship with GuildEvent
        builder.HasOne(lp => lp.Event)
            .WithMany(e => e.LootPiles)
            .HasForeignKey(lp => lp.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
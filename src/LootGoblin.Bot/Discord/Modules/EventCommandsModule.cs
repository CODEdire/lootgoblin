namespace LootGoblin.Bot.Discord.Modules;

/// <summary>
/// Provides slash commands for managing events within a guild.
/// </summary>
[SlashCommand("event", "Manage events for your guild")]
[RequireEventOrganizerRole<ApplicationCommandContext>]
public partial class EventCommandsModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IGuildEventService _eventService;
    private readonly EventDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventCommandsModule"/> class.
    /// </summary>
    /// <param name="eventService">The service for managing guild events.</param>
    /// <param name="db">The database context for event data.</param>
    public EventCommandsModule(IGuildEventService eventService, EventDbContext db)
    {
        _eventService = eventService;
        _db = db;
    }

    /// <summary>
    /// Creates a new event in the guild.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="description">The description of the event.</param>
    /// <param name="minMinutes">The minimum participation time in minutes.</param>
    /// <param name="maxParticipants">The maximum number of participants allowed.</param>
    [SubSlashCommand("create", "Create a new event")]
    public async Task CreateEventAsync(
        string name,
        string? description = null,
        int? minMinutes = null,
        int? maxParticipants = null)
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new InvalidOperationException("Must be used in a guild.");
        ulong userId = Context.Interaction.User.Id;

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        var createResult = await _eventService.CreateEventAsync(guildId, userId, name, description, minMinutes, maxParticipants);
        if (!createResult.IsSuccess)
        {
            await ModifyResponseAsync(message =>
            {
                message.Content = $"❌ {createResult.Message}";
            });
            return;
        }

        var ev = createResult.Value!;

        var settings = await _db.GuildSettings.FindAsync(guildId);
        ulong targetChannelId = settings?.EventChannelId ?? Context.Interaction.Channel.Id;
        var eventChannel = (TextGuildChannel)await Context.Client.Rest.GetChannelAsync(targetChannelId); //TODO: We need to handle forums later so we can choose forum or text channels.

        var eventMessage = await eventChannel.SendMessageAsync(new MessageProperties
        {
            Embeds = [BuildEventEmbed(ev, userId)]
        });

        await _eventService.SetEventMessageAsync(ev.Id, eventMessage.Id, eventChannel.Id);

        await ModifyResponseAsync(message =>
        {
            message.Content = $"✅ Event **{ev.Name}**({ev.Id}) created in <#{targetChannelId}>! [Jump to message](https://discord.com/channels/{eventChannel.GuildId}/{eventChannel.Id}/{eventMessage.Id})";
        });
    }

    /// <summary>
    /// Starts an existing event.
    /// </summary>
    /// <param name="eventId">The ID of the event to start.</param>
    [SubSlashCommand("start", "Start an existing event")]
    public async Task StartEventAsync(int eventId)
        => await HandleTransitionAsync(eventId, (id, userId) => _eventService.StartEventAsync(id, userId), "started");

    /// <summary>
    /// Pauses an active event.
    /// </summary>
    /// <param name="eventId">The ID of the event to pause.</param>
    [SubSlashCommand("pause", "Pause an active event")]
    public async Task PauseEventAsync(int eventId)
        => await HandleTransitionAsync(eventId, (id, userId) => _eventService.PauseEventAsync(id, userId), "paused");

    /// <summary>
    /// Resumes a paused event.
    /// </summary>
    /// <param name="eventId">The ID of the event to resume.</param>
    [SubSlashCommand("resume", "Resume a paused event")]
    public async Task ResumeEventAsync(int eventId)
        => await HandleTransitionAsync(eventId, (id, userId) => _eventService.ResumeEventAsync(id, userId), "resumed");

    /// <summary>
    /// Completes an active event.
    /// </summary>
    /// <param name="eventId">The ID of the event to complete.</param>
    [SubSlashCommand("complete", "Complete an active event")]
    public async Task CompleteEventAsync(int eventId)
        => await HandleTransitionAsync(eventId, (id, userId) => _eventService.CompleteEventAsync(id, userId), "completed");

    /// <summary>
    /// Cancels an event.
    /// </summary>
    /// <param name="eventId">The ID of the event to cancel.</param>
    [SubSlashCommand("cancel", "Cancel an event")]
    public async Task CancelEventAsync(int eventId)
        => await HandleTransitionAsync(eventId, (id, userId) => _eventService.CancelEventAsync(id, userId), "cancelled");

    /// <summary>
    /// Handles the transition of an event's state and updates the corresponding message.
    /// </summary>
    /// <param name="eventId">The ID of the event to transition.</param>
    /// <param name="action">The action to perform for the transition.</param>
    /// <param name="successMessage">The message to display on success.</param>
    private async Task HandleTransitionAsync(
        int eventId,
        Func<int, ulong, Task<OperationResult<GuildEvent>>> action,
        string successMessage)
    {
        ulong userId = Context.Interaction.User.Id;

        // Step 1: Immediate deferred ephemeral response
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        // Step 2: Perform DB update (may take time)
        var result = await action(eventId, userId);

        // Step 3: Update the deferred response with result
        await ModifyResponseAsync(message =>
        {
            message.Content = result.IsSuccess
                ? $"✅ Event **{result.Value!.Name}** has been {successMessage}."
                : $"❌ {result.Message}";
        });

        // Step 4: Update the original event message if success
        if (result.IsSuccess)
        {
            var ev = result.Value!;
            if (ev.MessageId.HasValue && ev.OriginChannelId.HasValue)
            {
                var embed = BuildEventEmbed(ev, ev.CreatedBy);

                var channel = (TextGuildChannel)await Context.Client.Rest.GetChannelAsync(ev.OriginChannelId.Value);
                await channel.ModifyMessageAsync(ev.MessageId.Value, message =>
                {
                    message.Embeds = [embed];
                });
            }
        }
    }

    /// <summary>
    /// Builds an embed representing the event details.
    /// </summary>
    /// <param name="ev">The event to display.</param>
    /// <param name="createdBy">The user ID of the event creator.</param>
    /// <returns>An <see cref="EmbedProperties"/> object representing the event.</returns>
    private static EmbedProperties BuildEventEmbed(GuildEvent ev, ulong createdBy)
    {
        return new EmbedProperties
        {
            Title = ev.Name,
            Description = string.IsNullOrWhiteSpace(ev.Description) ? "_No description provided._" : ev.Description,
            Color = ev.CurrentState switch
            {
                EventStatus.Created => new Color(0, 180, 255),
                EventStatus.Active => new Color(0, 200, 150),
                EventStatus.Completed => new Color(200, 150, 0),
                EventStatus.Cancelled => new Color(200, 0, 0),
                _ => new Color(100, 100, 100)
            },
            Fields =
            [
                new EmbedFieldProperties { Name = "Status", Value = ev.CurrentState.ToString(), Inline = true },
                new EmbedFieldProperties { Name = "Created By", Value = $"<@{createdBy}>", Inline = true },
                new EmbedFieldProperties { Name = "Minimum Time", Value = ev.MinimumParticipantMinutes?.ToString() ?? "None", Inline = true },
                new EmbedFieldProperties { Name = "Max Participants", Value = ev.MaximumParticipants?.ToString() ?? "Unlimited", Inline = true }
            ],
            Timestamp = ev.CreatedAt
        };
    }
}

namespace LootGoblin.Bot.Services;

/// <summary>
/// Provides methods for managing guild events, including creation, state transitions, participant channel management, and loot phase operations.
/// </summary>
public interface IGuildEventService
{
    /// <summary>
    /// Creates a new guild event.
    /// </summary>
    /// <param name="guildId">The ID of the guild where the event is created.</param>
    /// <param name="userId">The ID of the user creating the event.</param>
    /// <param name="name">The name of the event.</param>
    /// <param name="description">The description of the event.</param>
    /// <param name="minParticipationMinutes">The minimum participation time in minutes.</param>
    /// <param name="maxParticipants">The maximum number of participants allowed.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the created event or an error.</returns>
    Task<OperationResult<GuildEvent>> CreateEventAsync(ulong guildId, ulong userId, string name, string? description, int? minParticipationMinutes, int? maxParticipants);

    /// <summary>
    /// Sets the associated message ID and origin channel ID for a guild event.
    /// </summary>
    /// <param name="eventId">The unique identifier of the event to update.</param>
    /// <param name="messageId">The message ID to associate with the event.</param>
    /// <param name="originChannelId">The ID of the channel where the event originated.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> SetEventMessageAsync(int eventId, ulong messageId, ulong originChannelId);

    /// <summary>
    /// Starts an event, transitioning it to the Active state.
    /// </summary>
    /// <param name="eventId">The ID of the event to start.</param>
    /// <param name="userId">The ID of the user starting the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> StartEventAsync(int eventId, ulong userId);

    /// <summary>
    /// Pauses an active event.
    /// </summary>
    /// <param name="eventId">The ID of the event to pause.</param>
    /// <param name="userId">The ID of the user pausing the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> PauseEventAsync(int eventId, ulong userId);

    /// <summary>
    /// Resumes a paused event.
    /// </summary>
    /// <param name="eventId">The ID of the event to resume.</param>
    /// <param name="userId">The ID of the user resuming the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> ResumeEventAsync(int eventId, ulong userId);

    /// <summary>
    /// Completes an active event.
    /// </summary>
    /// <param name="eventId">The ID of the event to complete.</param>
    /// <param name="userId">The ID of the user completing the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> CompleteEventAsync(int eventId, ulong userId);

    /// <summary>
    /// Cancels an event.
    /// </summary>
    /// <param name="eventId">The ID of the event to cancel.</param>
    /// <param name="userId">The ID of the user canceling the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> CancelEventAsync(int eventId, ulong userId);

    /// <summary>
    /// Adds a participant channel to the event.
    /// </summary>
    /// <param name="eventId">The ID of the event to modify.</param>
    /// <param name="channelId">The ID of the channel to add.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> AddParticipantChannelAsync(int eventId, ulong channelId);

    /// <summary>
    /// Removes a participant channel from the event.
    /// </summary>
    /// <param name="eventId">The ID of the event to modify.</param>
    /// <param name="channelId">The ID of the channel to remove.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    Task<OperationResult<GuildEvent>> RemoveParticipantChannelAsync(int eventId, ulong channelId);

    /// <summary>
    /// Gets the participant channels associated with the event.
    /// </summary>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>A read-only collection of <see cref="GuildEventChannel"/> objects.</returns>
    Task<IReadOnlyCollection<GuildEventChannel>> GetParticipantChannelsAsync(int eventId);
}

/// <summary>
/// Implementation of <see cref="IGuildEventService"/> for managing guild events.
/// </summary>
public class GuildEventService : IGuildEventService
{
    private readonly EventDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildEventService"/> class.
    /// </summary>
    /// <param name="db">The database context for event operations.</param>
    public GuildEventService(EventDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> CreateEventAsync(ulong guildId, ulong userId, string name, string? description, int? minParticipationMinutes, int? maxParticipants)
    {
        if (string.IsNullOrWhiteSpace(name))
            return OperationResult<GuildEvent>.Fail(OperationStatus.ValidationError, "Event name is required.");

        if (minParticipationMinutes.HasValue && minParticipationMinutes < 0)
            return OperationResult<GuildEvent>.Fail(OperationStatus.ValidationError, "Minimum participation cannot be negative.");

        if (maxParticipants.HasValue && maxParticipants < 1)
            return OperationResult<GuildEvent>.Fail(OperationStatus.ValidationError, "Maximum participants must be at least 1.");

        var ev = new GuildEvent
        {
            GuildId = guildId,
            Name = name.Trim(),
            Description = description?.Trim(),
            MinimumParticipantMinutes = minParticipationMinutes,
            MaximumParticipants = maxParticipants,
            CurrentState = EventStatus.Created,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = userId
        };

        _db.GuildEvents.Add(ev);
        await _db.SaveChangesAsync();

        return OperationResult<GuildEvent>.Success(ev);
    }

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> SetEventMessageAsync(int eventId, ulong messageId, ulong originChannelId)
    {
        var ev = await _db.GuildEvents.FindAsync(eventId);
        if (ev == null)
            return OperationResult<GuildEvent>.Fail(OperationStatus.NotFound, "Event not found.");

        ev.MessageId = messageId;
        ev.OriginChannelId = originChannelId;

        await _db.SaveChangesAsync();
        return OperationResult<GuildEvent>.Success(ev);
    }

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> StartEventAsync(int eventId, ulong userId)
        => await UpdateEventStateAsync(eventId, EventStatus.Active, [EventStatus.Created],
            onUpdate: ev =>
            {
                ev.StartedAt = DateTimeOffset.UtcNow;
                ev.StartedBy = userId;
            });

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> PauseEventAsync(int eventId, ulong userId)
        => await UpdateEventStateAsync(eventId, EventStatus.Paused, [EventStatus.Active]);

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> ResumeEventAsync(int eventId, ulong userId)
        => await UpdateEventStateAsync(eventId, EventStatus.Active, [EventStatus.Paused]);

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> CompleteEventAsync(int eventId, ulong userId)
        => await UpdateEventStateAsync(eventId, EventStatus.Completed, [EventStatus.Active],
            onUpdate: ev =>
            {
                ev.CompletedAt = DateTimeOffset.UtcNow;
                ev.CompletedBy = userId;
            });

    /// <inheritdoc/>
    public async Task<OperationResult<GuildEvent>> CancelEventAsync(int eventId, ulong userId)
        => await UpdateEventStateAsync(eventId, EventStatus.Cancelled, [EventStatus.Created, EventStatus.Active, EventStatus.Paused, EventStatus.Completed]);

    /// <summary>
    /// Adds a participant channel to the event.
    /// </summary>
    /// <param name="eventId">The ID of the event to modify.</param>
    /// <param name="channelId">The ID of the channel to add.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    public async Task<OperationResult<GuildEvent>> AddParticipantChannelAsync(int eventId, ulong channelId)
    {
        var ev = await _db.GuildEvents.Include(e => e.ParticipantChannels)
                                      .FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev == null)
            return OperationResult<GuildEvent>.Fail(OperationStatus.NotFound, "Event not found.");

        if (ev.CurrentState != EventStatus.Created)
            return OperationResult<GuildEvent>.Fail(OperationStatus.InvalidState, "Channels can only be modified while the event is in Created state.");

        if (ev.ParticipantChannels.Any(c => c.ChannelId == channelId))
            return OperationResult<GuildEvent>.Fail(OperationStatus.ValidationError, "This channel is already added.");

        ev.ParticipantChannels.Add(new GuildEventChannel { ChannelId = channelId });
        await _db.SaveChangesAsync();

        return OperationResult<GuildEvent>.Success(ev);
    }

    /// <summary>
    /// Removes a participant channel from the event.
    /// </summary>
    /// <param name="eventId">The ID of the event to modify.</param>
    /// <param name="channelId">The ID of the channel to remove.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    public async Task<OperationResult<GuildEvent>> RemoveParticipantChannelAsync(int eventId, ulong channelId)
    {
        var ev = await _db.GuildEvents.Include(e => e.ParticipantChannels)
                                      .FirstOrDefaultAsync(e => e.Id == eventId);
        if (ev == null)
            return OperationResult<GuildEvent>.Fail(OperationStatus.NotFound, "Event not found.");

        var existing = ev.ParticipantChannels.FirstOrDefault(c => c.ChannelId == channelId);
        if (existing == null)
            return OperationResult<GuildEvent>.Fail(OperationStatus.ValidationError, "This channel is not associated with the event.");

        ev.ParticipantChannels.Remove(existing);
        await _db.SaveChangesAsync();

        return OperationResult<GuildEvent>.Success(ev);
    }

    /// <summary>
    /// Gets the participant channels associated with the event.
    /// </summary>
    /// <param name="eventId">The ID of the event.</param>
    /// <returns>A read-only collection of <see cref="GuildEventChannel"/> objects.</returns>
    public async Task<IReadOnlyCollection<GuildEventChannel>> GetParticipantChannelsAsync(int eventId)
    {
        var ev = await _db.GuildEvents.Include(e => e.ParticipantChannels)
                                      .FirstOrDefaultAsync(e => e.Id == eventId);
        return ev?.ParticipantChannels.ToList() ?? [];
    }

    /// <summary>
    /// Updates the state of an event if the current state is allowed.
    /// </summary>
    /// <param name="eventId">The ID of the event to update.</param>
    /// <param name="targetStatus">The target status to set.</param>
    /// <param name="allowedFrom">The allowed current states for the transition.</param>
    /// <param name="onUpdate">An optional action to perform additional updates on the event.</param>
    /// <returns>An <see cref="OperationResult{GuildEvent}"/> containing the updated event or an error.</returns>
    private async Task<OperationResult<GuildEvent>> UpdateEventStateAsync(int eventId, EventStatus targetStatus, EventStatus[] allowedFrom, Action<GuildEvent>? onUpdate = null)
    {
        var ev = await _db.GuildEvents.FindAsync(eventId);
        if (ev == null)
            return OperationResult<GuildEvent>.Fail(OperationStatus.NotFound, "Event not found.");

        if (!allowedFrom.Contains(ev.CurrentState))
            return OperationResult<GuildEvent>.Fail(OperationStatus.InvalidState,
                $"Cannot change event from {ev.CurrentState} to {targetStatus}.");

        ev.CurrentState = targetStatus;
        onUpdate?.Invoke(ev);

        await _db.SaveChangesAsync();
        return OperationResult<GuildEvent>.Success(ev);
    }
}


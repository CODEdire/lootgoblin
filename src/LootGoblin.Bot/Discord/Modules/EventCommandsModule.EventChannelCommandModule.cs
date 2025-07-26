namespace LootGoblin.Bot.Discord.Modules;

public partial class EventCommandsModule
{
    /// <summary>
    /// Provides slash commands for managing event participant channels.
    /// </summary>
    [SubSlashCommand("channel", "Manage event participant channels")]
    [RequireEventOrganizerRole<ApplicationCommandContext>]
    public class EventChannelCommandModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        private readonly IGuildEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventChannelCommandModule"/> class.
        /// </summary>
        /// <param name="eventService">The service for managing guild events.</param>
        public EventChannelCommandModule(IGuildEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Adds a channel to the event's participant list.
        /// </summary>
        /// <param name="eventId">The ID of the event to modify.</param>
        /// <param name="channel">The channel to add as a participant channel.</param>
        [SubSlashCommand("add", "Add a channel to the event's participant list")]
        public async Task AddChannelAsync(int eventId, Channel channel)
        {
            await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var result = await _eventService.AddParticipantChannelAsync(eventId, channel.Id);

            await ModifyResponseAsync(msg =>
            {
                msg.Content = result.IsSuccess
                    ? $"✅ Channel <#{channel.Id}> added to event **{result.Value!.Name}**."
                    : $"❌ {result.Message}";
            });
        }

        /// <summary>
        /// Removes a channel from the event's participant list.
        /// </summary>
        /// <param name="eventId">The ID of the event to modify.</param>
        /// <param name="channel">The channel to remove from the participant list.</param>
        [SubSlashCommand("remove", "Remove a channel from the event's participant list")]
        public async Task RemoveChannelAsync(int eventId, Channel channel)
        {
            await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var result = await _eventService.RemoveParticipantChannelAsync(eventId, channel.Id);

            await ModifyResponseAsync(msg =>
            {
                msg.Content = result.IsSuccess
                    ? $"✅ Channel <#{channel.Id}> removed from event **{result.Value!.Name}**."
                    : $"❌ {result.Message}";
            });
        }

        /// <summary>
        /// Shows all participant channels for the event.
        /// </summary>
        /// <param name="eventId">The ID of the event to list channels for.</param>
        [SubSlashCommand("list", "Show all participant channels for the event")]
        public async Task ListChannelsAsync(int eventId)
        {
            await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

            var channels = await _eventService.GetParticipantChannelsAsync(eventId);

            string content = channels.Count > 0
                ? $"📋 **Participant Channels:**\n{string.Join("\n", channels.Select(c => $"• <#{c.ChannelId}>"))}"
                : "No channels configured for this event.";

            await ModifyResponseAsync(msg => msg.Content = content);
        }
    }
}

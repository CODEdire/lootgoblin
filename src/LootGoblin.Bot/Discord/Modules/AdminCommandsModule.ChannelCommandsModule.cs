namespace LootGoblin.Bot.Discord.Modules;

/// <summary>
/// Contains administrative subcommands for managing channel settings in a guild.
/// </summary>
/// <remarks>
/// This module is a subcommand group under <c>/admin channel</c> and allows administrators to set or clear bot channel settings such as loot and event channels.
/// </remarks>
public partial class AdminCommandsModule
{
    /// <summary>
    /// Provides subcommands for configuring bot channel settings within a guild.
    /// </summary>
    /// <remarks>
    /// Subcommand group: <c>/admin channel ...</c>
    /// Only users with administrator permissions can execute these commands.
    /// </remarks>
    [SubSlashCommand("channel", "Set or clear bot channel settings")]
    [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)] // Ensure only admins
    public class ChannelCommandsModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        private readonly IGuildSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelCommandsModule"/> class.
        /// </summary>
        /// <param name="settingsService">The service used to manage guild settings.</param>
        public ChannelCommandsModule(IGuildSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Sets or clears the loot channel for the guild.
        /// </summary>
        /// <param name="channel">The channel to set as the loot channel, or <c>null</c> to clear the setting.</param>
        /// <returns>
        /// A confirmation message indicating whether the loot channel was set or cleared.
        /// </returns>
        /// <remarks>
        /// Command: <c>/admin channel loot [channel|null]</c> – sets or clears the loot channel.
        /// Ensures the command is used in a guild context and validates that the specified channel is a text, announcement, or forum channel.
        /// </remarks>
        [SubSlashCommand("loot", "Sets or clears the loot channel")]
        public async Task<string> SetLootChannelAsync(Channel? channel = null)
        {
            ulong guildId = Context.Interaction.GuildId ?? throw new InvalidOperationException("This command can only be used within a guild.");

            // Validate channel type if provided (expect a text channel for loot announcements)
            if (channel is not null)
            {
                // Only allow text-based guild channels (e.g., Text or News/Announcement channels)
                if (channel is not TextGuildChannel && channel is not AnnouncementGuildChannel && channel is not ForumGuildChannel)
                {
                    return "❌ Please specify a text, announcement, or forum channel for loot drops.";
                }
            }

            // Persist the setting (channel.Id or null to clear)
            await _settingsService.SetLootChannelAsync(guildId, channel?.Id);

            // Return a confirmation message
            return channel is null
                 ? "✅ Loot channel has been **cleared**. Loot will be created in the channel the command is entered."
                 : $"✅ Loot channel set to <#{channel.Id}>.";
        }

        /// <summary>
        /// Sets or clears the event channel for the guild.
        /// </summary>
        /// <param name="channel">The channel to set as the event channel, or <c>null</c> to clear the setting.</param>
        /// <returns>
        /// A confirmation message indicating whether the event channel was set or cleared.
        /// </returns>
        /// <remarks>
        /// Command: <c>/admin channel event [channel|null]</c> – sets or clears the event channel.
        /// Ensures the command is used in a guild context and validates that the specified channel is a text or announcement channel.
        /// </remarks>
        [SubSlashCommand("event", "Sets or clears the event channel")]
        public async Task<string> SetEventChannelAsync(Channel? channel = null)
        {
            ulong guildId = Context.Interaction.GuildId ?? throw new InvalidOperationException("This command can only be used within a guild.");

            if (channel is not null)
            {
                if (channel is not TextGuildChannel && channel is not AnnouncementGuildChannel)
                {
                    return "❌ Please specify a text channel for event announcements.";
                }
            }

            await _settingsService.SetEventChannelAsync(guildId, channel?.Id);

            return channel is null
                ? "✅ Event channel has been **cleared**. Events will be created in the channel the command is entered."
                : $"✅ Event channel set to <#{channel.Id}>.";
        }
    }
}

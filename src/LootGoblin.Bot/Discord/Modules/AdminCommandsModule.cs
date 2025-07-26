namespace LootGoblin.Bot.Discord.Modules;

/// <summary>
/// Provides administrative guild configuration commands.
/// </summary>
/// <remarks>
/// This module contains commands for configuring various administrative settings within a guild.
/// Only users with administrator permissions can execute these commands.
/// </remarks>
[SlashCommand("admin", "Administrative guild configuration commands")]
[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
public partial class AdminCommandsModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly IGuildSettingsService _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminCommandsModule"/> class.
    /// </summary>
    /// <param name="settingsService">The service used to manage guild settings.</param>
    public AdminCommandsModule(IGuildSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    /// <summary>
    /// Displays the current guild settings in an embed.
    /// </summary>
    /// <returns>An embed containing the current configuration.</returns>
    [SubSlashCommand("settings", "View the current configuration for this guild")]
    public async Task GetSettingsAsync()
    {
        ulong guildId = Context.Interaction.GuildId
            ?? throw new InvalidOperationException("This command can only be used within a guild.");

        var settings = await _settingsService.GetGuildSettingsAsync(guildId);

        var embed = new EmbedProperties {
            Title = "Guild Settings",
            Description = "Here are the current configuration values for this server:",
            Color = new Color(0, 180, 255),
            Fields =
            [
                new EmbedFieldProperties {
                    Name = "Loot Channel",
                    Value = settings.LootChannelId.HasValue ? $"<#{settings.LootChannelId.Value}>" : "Not set",
                    Inline = true
                },
                new EmbedFieldProperties {
                    Name = "Event Channel",
                    Value = settings.EventChannelId.HasValue ? $"<#{settings.EventChannelId.Value}>" : "Not set",
                    Inline = true
                },
                new EmbedFieldProperties {
                    Name = "Organizer Role",
                    Value = settings.EventOrganizerRoleId.HasValue ? $"<@&{settings.EventOrganizerRoleId.Value}>" : "Not set",
                    Inline = true
                },
                new EmbedFieldProperties {
                    Name = "Participant Role",
                    Value = settings.EventParticipantRoleId.HasValue ? $"<@&{settings.EventParticipantRoleId.Value}>" : "Not set",
                    Inline = true
                },
            ],
            Timestamp = DateTimeOffset.UtcNow
        };

        await RespondAsync(InteractionCallback.Message(new InteractionMessageProperties()
        {
            Embeds = [embed],
            Flags = MessageFlags.Ephemeral //Only person that ran command should see
        }));
    }
}

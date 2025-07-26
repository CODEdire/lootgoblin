namespace LootGoblin.Bot.Discord.Modules;

/// <summary>
/// Contains administrative subcommands for managing required roles in a guild.
/// </summary>
/// <remarks>
/// This module is a subcommand group under <c>/admin role</c> and allows administrators to set or clear required roles such as organizer and participant roles.
/// </remarks>
public partial class AdminCommandsModule
{
    /// <summary>
    /// Provides subcommands for configuring required roles within a guild.
    /// </summary>
    /// <remarks>
    /// Subcommand group: <c>/admin role ...</c>
    /// Only users with administrator permissions can execute these commands.
    /// </remarks>
    [SubSlashCommand("role", "Set or clear required roles")]
    [RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
    public class RoleCommandsModule : ApplicationCommandModule<ApplicationCommandContext>
    {
        private readonly IGuildSettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleCommandsModule"/> class.
        /// </summary>
        /// <param name="settingsService">The service used to manage guild settings.</param>
        public RoleCommandsModule(IGuildSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Sets or clears the required Organizer role for the guild.
        /// </summary>
        /// <param name="role">The role to set as the required Organizer role, or <c>null</c> to clear the requirement.</param>
        /// <returns>
        /// A confirmation message indicating whether the Organizer role requirement was set or cleared.
        /// </returns>
        /// <remarks>
        /// Command: <c>/admin role organizer [role|null]</c> – sets or clears the organizer role requirement.
        /// If a role is provided, ensures it's from the same guild (Discord UI enforces this by default).
        /// Persists the organizer role setting (<c>role.Id</c> or <c>null</c>).
        /// </remarks>
        [SubSlashCommand("organizer", "Sets or clears the required Organizer role")]
        public async Task<string> SetEventOrganizerRoleAsync(Role? role = null)
        {
            ulong guildId = Context.Interaction.GuildId ?? throw new InvalidOperationException("This command can only be used within a guild.");

            await _settingsService.SetEventOrganizerRoleAsync(guildId, role?.Id);

            return role is null
                ? "✅ Organizer role requirement **cleared**. (No specific organizer role required.)"
                : $"✅ Organizer role set to **@{role.Name}**.";
        }

        /// <summary>
        /// Sets or clears the required Participant role for the guild.
        /// </summary>
        /// <param name="role">The role to set as the required Participant role, or <c>null</c> to clear the requirement.</param>
        /// <returns>
        /// A confirmation message indicating whether the Participant role requirement was set or cleared.
        /// </returns>
        /// <remarks>
        /// Command: <c>/admin role participant [role|null]</c> – sets or clears the participant role requirement.
        /// </remarks>
        [SubSlashCommand("participant", "Sets or clears the required Participant role")]
        public async Task<string> SetParticipantRoleAsync(Role? role = null)
        {
            ulong guildId = Context.Interaction.GuildId ?? throw new InvalidOperationException("This command can only be used within a guild.");

            await _settingsService.SetEventParticipantRoleAsync(guildId, role?.Id);

            return role is null
                ? "✅ Participant role requirement **cleared**. (No specific participant role required.)"
                : $"✅ Participant role set to **@{role.Name}**.";
        }
    }
}

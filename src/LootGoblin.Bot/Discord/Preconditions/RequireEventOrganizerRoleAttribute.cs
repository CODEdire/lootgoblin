namespace LootGoblin.Bot.Discord.Preconditions;

/// <summary>
/// Precondition attribute that requires the user to have the "Event Organizer" role in the guild to execute the command.
/// </summary>
/// <typeparam name="TContext">
/// The type of the command context, which must implement <see cref="IUserContext"/> and <see cref="IGuildContext"/>.
/// </typeparam>
public class RequireEventOrganizerRoleAttribute<TContext> : RoleCheckPrecondition<TContext>
    where TContext : IUserContext, IGuildContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequireEventOrganizerRoleAttribute{TContext}"/> class.
    /// </summary>
    public RequireEventOrganizerRoleAttribute()
        : base(gs => gs.EventOrganizerRoleId, "Event Organizer") { }
}

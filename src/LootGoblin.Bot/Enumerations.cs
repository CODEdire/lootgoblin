namespace LootGoblin.Bot;

/// <summary>
/// Represents the life cycle states of a guild event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event has been created but not yet started. Organizer can edit or cancel.
    /// </summary>
    Created = 0,

    /// <summary>
    /// Event is active; participant tracking is in progress.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Event is temporarily paused; tracking is on hold.
    /// </summary>
    Paused = 2,

    /// <summary>
    /// Event tracking is finished; preparing for loot phase.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Event was canceled and is no longer active.
    /// </summary>
    Cancelled = 6
}

/// <summary>
/// Represents the life cycle states of a loot pile within an event.
/// </summary>
public enum LootStatus : byte
{
    /// <summary>
    /// Loot pile is created but not yet open for rolls.
    /// </summary>
    Created = 0,

    /// <summary>
    /// Loot pile is open for participants to roll.
    /// </summary>
    Open = 1,

    /// <summary>
    /// Loot pile is closed; no more rolls can be made.
    /// </summary>
    Closed = 2,

    /// <summary>
    /// Loot pile has been completed; winners have been determined.
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Loot pile was canceled and is no longer active.
    /// </summary>
    Cancelled = 4
}

/// <summary>
/// Specifies the type of loot pile.
/// </summary>
public enum LootType : byte
{
    /// <summary>
    /// Loot pile is restricted to participants.
    /// </summary>
    Restricted = 0,

    /// <summary>
    /// Loot pile is open to anyone who can see it.
    /// </summary>
    Open = 1
}

/// <summary>
/// Specifies the type of loot roll.
/// </summary>
public enum LootRollType : byte
{
    /// <summary>
    /// Participants can roll for loot.
    /// </summary>
    Roll = 0,

    /// <summary>
    /// Participants can bid for loot. Tickets awarded based on participation or handouts.
    /// </summary>
    Bid = 1
}

/// <summary>
/// Represents the result status of an operation.
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success,
    /// <summary>
    /// The operation failed due to an invalid state.
    /// </summary>
    InvalidState,
    /// <summary>
    /// The operation failed due to lack of authorization.
    /// </summary>
    Unauthorized,
    /// <summary>
    /// The requested entity was not found.
    /// </summary>
    NotFound,
    /// <summary>
    /// The operation failed due to validation error.
    /// </summary>
    ValidationError,
    /// <summary>
    /// The operation failed for an unspecified reason.
    /// </summary>
    Failed
}
using Content.Shared.Actions;

namespace Content.Shared._Harmony.Emoting;

/// <summary>
/// Action components which should write a message to ICChat on use
/// </summary>
public sealed partial class EmoteActionEvent : InstantActionEvent
{
    /// <summary>
    /// The emote used
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Emote;
}

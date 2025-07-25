using Content.Shared.Actions;
using Robust.Shared.Audio;

namespace Content.Shared._Harmony.RandomTeleport;

/// <summary>
/// Teleports the user randomly on use
/// </summary>
public sealed partial class RandomTeleportEvent : InstantActionEvent
{
    /// <summary>
    /// Distance you travel
    /// </summary>
    [DataField]
    public float Radius = 4;

    /// <summary>
    /// Effect spawned at teleportation site
    /// </summary>
    [DataField]
    public string Effect = "Smoke";

    /// <summary>
    /// Audio played at teleportation site
    /// </summary>
    [DataField]
    public SoundSpecifier SoundSpecifier =  new SoundPathSpecifier("/Audio/Magic/blink.ogg");
}

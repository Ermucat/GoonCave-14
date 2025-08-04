using Content.Shared.Actions;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

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
    public EntProtoId Effect = "Smoke";

    /// <summary>
    /// Audio played at teleportation site
    /// </summary>
    [DataField]
    public SoundSpecifier SoundSpecifier =  new SoundPathSpecifier("/Audio/Magic/blink.ogg");
}

using Content.Shared.Actions;
using Robust.Shared.Audio;

namespace Content.Shared._Harmony.RandomTeleport;

public sealed partial class RandomTeleportEvent : InstantActionEvent
{
    [DataField]
    public float Radius = 7;

    [DataField]
    public string Effect = "Smoke";

    [DataField]
    public SoundSpecifier SoundSpecifier =  new SoundPathSpecifier("/Audio/Magic/blink.ogg");
}

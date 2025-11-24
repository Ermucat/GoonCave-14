using Robust.Shared.GameStates;

namespace Content.Shared._Harmony.ScryingOrb;

[RegisterComponent, NetworkedComponent]
public sealed partial class ScryingOrbSpiritComponent : Component
{
    [DataField]
    public EntityUid Parent;

}

using Robust.Shared.GameStates;

namespace Content.Shared._Harmony.ScryingOrb;

[RegisterComponent, NetworkedComponent]
public sealed partial class ScryingOrbComponent : Component
{
    [DataField]
    public string ScryingOrbPrototype = "MobScryingSpirit";

    [DataField]
    public string ScryingOrbReturnAction = "ActionReturnToBody";

}

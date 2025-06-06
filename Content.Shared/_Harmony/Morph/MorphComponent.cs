using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared._Harmony.Morph;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MorphComponent : Component
{
    [DataField]
    public float Amount = 10;

    /// <summary>
    /// The total amount of Essence the revenant has. Functions
    /// as health and is regenerated.
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public FixedPoint2 Biomass;

    [DataField]
    public float ReplicateCost = 10;

    [DataField]
    public float ReplicationDelay = 10f;

    [DataField]
    public float MorphDelay = 2.5f;

    [DataField]
    public string MorphPrototype = "MobMorph";

    [DataField]
    public string MorphAction = "ActionMorphReplicate";

    [DataField] public EntityUid? Action;
}

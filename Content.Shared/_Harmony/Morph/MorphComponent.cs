using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Audio;

namespace Content.Shared._Harmony.Morph;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MorphComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public FixedPoint2 Biomass;

    [DataField]
    public float ReplicateCost = 45;

    [DataField]
    public float ReplicationDelay = 5f;

    [DataField]
    public static int Children;


    [DataField]
    public string MorphPrototype = "MobMorph";

    [DataField]
    public ProtoId<AlertPrototype> BiomassAlert = "Biomass";

    [NetSerializable, Serializable]
    public enum MorphVisualLayers : byte
    {
        Digit1,
        Digit2,
        Digit3
    }

    // Morph Actions
    [DataField]
    public string MorphReplicate = "ActionMorphReplicate";

    [DataField]
    public string Morph = "ActionMorph";

    [DataField]
    public string UnMorph = "ActionUnMorph";

    // Morph Sounds
    [DataField]
    public SoundSpecifier ReplicateSound = new SoundPathSpecifier("/Audio/_Harmony/Misc/Mutate.ogg");
}

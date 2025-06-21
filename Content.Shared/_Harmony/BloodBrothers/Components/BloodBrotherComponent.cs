using Content.Shared.StatusIcon;
using Content.Shared.Tag;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.BloodBrothers.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class BloodBrotherComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Brother;

    [DataField]
    public ProtoId<FactionIconPrototype> BloodBrotherIcon = "BloodBrotherFaction";

    public override bool SessionSpecific => true;

    /// <summary>
    /// Tag added on component startup
    /// </summary>
    [DataField]
    public ProtoId<TagPrototype> BloodBrotherTag = "BloodBrother";
}

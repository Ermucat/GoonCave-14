using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.Arcfiend;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class ArcfiendComponent : Component
{
    /// <summary>
    /// The alert to add on MapInit
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> PowerAlert = "Power";

    /// <summary>
    /// The alert to add on MapInit for draining
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> DrainerAlert = "Drainer";

    /// <summary>
    /// The alert to add on MapInit
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public FixedPoint2 Charge = 10;

    /// <summary>
    /// The alert to add on MapInit
    /// </summary>
    [DataField]
    public float MaxCharge = 1000;

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public TimeSpan Stuntime = TimeSpan.FromSeconds(4);

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public TimeSpan Flashtime = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public string BoltPrototype = "ArcfiendLightning";
}

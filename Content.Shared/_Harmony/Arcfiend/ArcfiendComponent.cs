using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Server._Harmony.Arcfiend;

[RegisterComponent]
public sealed partial class ArcfiendComponent : Component
{
    /// <summary>
    /// The alert to add on MapInit
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> PowerAlert = "Power";

    /// <summary>
    /// The alert to add on MapInit
    /// </summary>
    [DataField]
    public FixedPoint2 Charge = 0;
}

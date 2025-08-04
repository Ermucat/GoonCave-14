using Content.Shared.Actions;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.Arcfiend;

public sealed partial class ArcfiendDischargeEvent : EntityTargetActionEvent
{
    [DataField]
    public float Cost = 100;
}

public sealed partial class ArcfiendFlashEvent : InstantActionEvent
{
    [DataField]
    public float Cost = 150;

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public string FlashPrototype = "GrenadeFlashEffect";

    /// <summary>
    ///     What kind of entities get pushed
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? PushWhitelist;

}

public sealed partial class ArcfiendJammerEvent : InstantActionEvent
{
    [DataField]
    public float Cost = 200;

    [DataField]
    public EntProtoId JammerEffect = "StatusEffectJammingField";

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public TimeSpan EffectTime = TimeSpan.FromSeconds(30);
}

public sealed partial class ArcfiendArcBoltEvent : EntityTargetActionEvent
{
    [DataField]
    public float Cost = 250;

    /// <summary>
    /// The
    /// </summary>
    [DataField]
    public string BoltPrototype = "ArcfiendLightning";
}

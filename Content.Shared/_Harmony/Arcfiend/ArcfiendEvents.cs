using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Ninja.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Harmony.Arcfiend;

public sealed partial class ArcfiendDischargeEvent : EntityTargetActionEvent
{
    [DataField]
    public float Cost = 100;

    /// <summary>
    /// The time you stun the opponent
    /// </summary>
    [DataField]
    public TimeSpan Stuntime = TimeSpan.FromSeconds(4);
}

public sealed partial class ArcfiendFlashEvent : InstantActionEvent
{
    [DataField]
    public float Cost = 150;

    [DataField]
    public float StaminaDamage = 50;

    /// <summary>
    /// The effect to spawn on use
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
    /// The time the ffect last
    /// </summary>
    [DataField]
    public TimeSpan EffectTime = TimeSpan.FromSeconds(30);
}

public sealed partial class ArcfiendArcBoltEvent : EntityTargetActionEvent
{
    [DataField]
    public float Cost = 250;

    /// <summary>
    /// The bolt to spawn
    /// </summary>
    [DataField]
    public EntProtoId BoltPrototype = "ArcfiendLightning";
}

/// <summary>
/// DoAfter event for Arcfiend
/// </summary>
[Serializable, NetSerializable]
public sealed partial class DrainDoAfterEvent : SimpleDoAfterEvent;

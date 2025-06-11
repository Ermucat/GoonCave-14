using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Harmony.Kicking.Components;

public sealed partial class KickingEvent : EntityTargetActionEvent
{
    /// <summary>
    /// How long the peroformer is slowed when kicking
    /// </summary>
    [DataField]
    public TimeSpan Kicktime = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// How far the kick sends you
    /// </summary>
    [DataField]
    public float KickPower = 2500;

    /// <summary>
    /// How much stamina damage is done to the user when the action is use
    /// </summary>
    [DataField]
    public float SlowdownMultiplier = 50;

    /// <summary>
    /// How long the target is stunned when kicked
    /// </summary>
    [DataField]
    public TimeSpan Stuntime = TimeSpan.FromSeconds(3);

    /// <summary>
    /// How long the peroformer is slowed when kicking
    /// </summary>
    [DataField]
    public TimeSpan Slowtime = TimeSpan.FromSeconds(4);
}
[Serializable, NetSerializable]
public sealed partial class KickDoAfterEvent : SimpleDoAfterEvent
{
}

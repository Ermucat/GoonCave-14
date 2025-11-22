using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
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
    [DataField]
    [AutoNetworkedField]
    public FixedPoint2 Charge = 10;

    /// <summary>
    /// How damaged can a target be before their no longer able to be drained
    /// <summar>
    [DataField]
    public float TargetAbsorbsionLimit = 300;

    /// <summary>
    /// How much charge you get every time you absorb a body
    /// <summar>
    [DataField]
    public float TargetAbsorbsionGain = 10;

    /// <summary>
    /// The range of the lightning that is produced when you are EMP'd
    /// <summar>
    [DataField]
    public float EMPDischargeRange = 10;

    /// <summary>
    /// The time you are stunned on emp
    /// </summary>
    [DataField]
    public TimeSpan Stuntime = TimeSpan.FromSeconds(4);

    /// <summary>
    /// The bolt thats shot out on emp
    /// </summary>
    [DataField]
    public string BoltPrototype = "ArcfiendLightning";

    /// <summary>
    /// Time that the do after takes to drain charge from a lifeform, in seconds.
    /// </summary>
    [DataField]
    public TimeSpan DrainTime = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Time that the do after takes to drain charge from a lifeform, in seconds.
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier DrainDamage;

    /// <summary>
    /// Sound played after the doafter ends.
    /// </summary>
    [DataField]
    public SoundSpecifier SparkSound = new SoundCollectionSpecifier("sparks");
}

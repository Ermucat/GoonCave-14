using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;

namespace Content.Shared.Magic.Events;

public sealed partial class LightningBoltSpellEvent : EntityTargetActionEvent
{
    [DataField]
    public TimeSpan ChargeUpTime = TimeSpan.FromSeconds(10);

    [DataField]
    public string Effect = "EffectElectricity";

    [DataField]
    public SoundSpecifier? SoundCharge = new SoundPathSpecifier("/Audio/_Harmony/Magic/sound_magic_lightning_chargeup.ogg");
}

[Serializable, NetSerializable]
public sealed partial class LightningBoltDoAfterEvent : SimpleDoAfterEvent
{
}


using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.StatusEffect.Components;

[RegisterComponent]
public sealed partial class ModifyStatusEffectOnProjectileHitComponent : Component
{
    [DataField(required: true)]
    public EntProtoId Effect;

    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(5);
}

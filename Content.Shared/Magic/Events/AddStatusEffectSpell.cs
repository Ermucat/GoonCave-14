using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.Magic.Events;

public sealed partial class AddStatusEffectSpell : EntityTargetActionEvent
{
    /// <summary>
    /// Duration Effect
    /// </summary>
    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Effect for new system
    /// </summary>
    [DataField]
    public EntProtoId Effect;

    /// <summary>
    /// Effect for old system
    /// </summary>
    [DataField]
    public string Key = default!;

    [DataField]
    public string Component = String.Empty;
}

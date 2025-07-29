using Content.Shared.Actions;

namespace Content.Shared.Magic.Events;

public sealed partial class AddStatusEffectSpell : EntityTargetActionEvent
{
    [DataField]
    public string Key;

    [DataField]
    public TimeSpan Duration = TimeSpan.FromSeconds(10);

    [DataField]
    public string Component = String.Empty;
}

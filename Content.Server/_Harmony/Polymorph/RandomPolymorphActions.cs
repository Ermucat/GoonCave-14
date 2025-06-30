using Content.Shared.Actions;

namespace Content.Server._Harmony.Polymorph;

public sealed partial class RandomPolymorphActionEvent : InstantActionEvent
{
    /// <summary>
    /// Polymorph settings.
    /// </summary>
    [DataField(required: true)]
    public List<string?> Polymorphs;
}

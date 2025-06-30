using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;

namespace Content.Server._Harmony.Polymorph;

/// <summary>
/// Intended for use with the trigger system.
/// Polymorphs the user of the trigger.
/// </summary>
[RegisterComponent]
public sealed partial class RandomPolymorphOnTriggerComponent : Component
{
    /// <summary>
    /// Polymorph settings.
    /// </summary>
    [DataField(required: true)]
    public List<string?> Polymorphs;
}

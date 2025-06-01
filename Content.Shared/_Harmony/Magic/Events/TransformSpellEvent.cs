using Content.Shared.Actions;
using Content.Shared.Magic;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.Magic.Events;

public sealed partial class TransformSpellEvent : EntityTargetActionEvent
{
    /// <summary>
    /// Transforms an entities clothes and components
    /// </summary>
    [DataField]
    public string Loadout;

    [DataField]
    [AlwaysPushInheritance]
    public ComponentRegistry ToAdd = new();

    [DataField]
    [AlwaysPushInheritance]
    public HashSet<string> ToRemove = new();

    [DataField]
    public string? Speech { get; private set; }

    [DataField]
    public TimeSpan StunTime = TimeSpan.FromSeconds(5);
}

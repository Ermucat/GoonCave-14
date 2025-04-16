using Content.Shared.Actions;
using Robust.Shared.Prototypes;


namespace Content.Shared.Magic.Events;

public sealed partial class ClothesChangeSpellEvent : EntityTargetActionEvent, ISpeakSpell
{
    // TODO: Make part of gib method
    /// <summary>
    /// Should this smite delete all parts/mechanisms gibbed except for the brain?
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
}

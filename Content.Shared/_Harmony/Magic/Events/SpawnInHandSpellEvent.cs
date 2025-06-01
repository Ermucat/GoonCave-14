using Content.Shared.Actions;

namespace Content.Shared._Harmony.Magic.Events;

public sealed partial class SpawnInHandSpellEvent : InstantActionEvent
{
    /// <summary>
    /// What entity should be spawned.
    /// </summary>
    [DataField(required: true)]
    public string Prototype;
}

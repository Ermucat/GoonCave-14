using Content.Shared.Actions;
using Robust.Shared.Audio;

namespace Content.Shared._Harmony.Magic.Events;

public sealed partial class SpawnInHandSpellEvent : InstantActionEvent
{
    /// <summary>
    /// What entity should be spawned.
    /// </summary>
    [DataField(required: true)]
    public string Prototype;

    /// <summary>
    /// Sound that will play globally when cast
    /// </summary>
    [DataField]
    public SoundSpecifier Sound;
}

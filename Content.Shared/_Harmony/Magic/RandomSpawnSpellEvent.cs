using Content.Shared.Actions;
using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.Magic;

public sealed partial class RandomSpawnSpellEvent : InstantActionEvent
{
    /// <summary>
    /// The list of prototypes this spell will spawn
    /// </summary>
    [DataField]
    public List<EntProtoId> Prototypes = new();

    [DataField]
    public int RangeMin = -3;

    [DataField]
    public int RangeMax = 3;

    [DataField]
    public float TrapsToSpawn = 5;
}

using System.Numerics;
using Content.Shared.Actions;
using Content.Shared.Polymorph;
using Content.Shared.Storage;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic.Events;

public sealed partial class PolymorphItemEvent : EntityTargetActionEvent
{
    /// <summary>
    /// Lifetime to set for the entities to self delete
    /// </summary>
    [DataField]
    public ProtoId<PolymorphPrototype> Polymorph;
}

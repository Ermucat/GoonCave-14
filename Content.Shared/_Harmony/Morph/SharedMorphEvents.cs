using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Harmony.Morph;

//[Serializable, NetSerializable]
public sealed partial class MorphReplicateEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class ReplicateDoAfterEvent : SimpleDoAfterEvent
{
}

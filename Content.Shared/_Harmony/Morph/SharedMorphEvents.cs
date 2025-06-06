using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._Harmony.Morph;

public sealed partial class MorphReplicateEvent : InstantActionEvent
{
}

public sealed partial class MorphEvent : EntityTargetActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class ReplicateDoAfterEvent : SimpleDoAfterEvent
{
}

[Serializable, NetSerializable]
public sealed partial class MorphDoAfterEvent : SimpleDoAfterEvent
{
}

using Content.Server._Goobstation.GameeTicking.Rules.Components;
using Content.Server._Goobstation.Blob;
using Content.Server.GameTicking.Rules.Components;

namespace Content.Server._Goobstation.Blob;

public sealed class BlobChangeLevelEvent : EntityEventArgs
{
    public EntityUid Station;
    public BlobStage Level;
}

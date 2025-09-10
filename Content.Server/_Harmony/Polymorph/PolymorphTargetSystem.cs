using Content.Server.Polymorph.Systems;

namespace Content.Server._Harmony.Polymorph;

public sealed class PolymorphTargetSystem : EntitySystem
{
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<PolymorphTargetActionEvent>(OnPolymorphTarget);
    }

    public void OnPolymorphTarget(PolymorphTargetActionEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        _polymorph.PolymorphEntity(args.Target, args.ProtoId);
    }
}

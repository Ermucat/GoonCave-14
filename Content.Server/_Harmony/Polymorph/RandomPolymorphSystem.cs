using Content.Server.Explosion.EntitySystems;
using Content.Server.Polymorph.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.Polymorph;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Harmony.Polymorph;

public sealed class RandomPolymorphSystem: EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    private void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RandomPolymorphOnTriggerComponent, TriggerEvent>(OnTrigger);

        SubscribeLocalEvent<RandomPolymorphActionEvent>(OnPolymorphActionEvent);

    }

    public void RandomPolymorph(EntityUid uid, List<string?> list)
    {
        ProtoId<PolymorphPrototype> polymorph =  _random.Pick(list)!;

        _polymorph.PolymorphEntity(uid, polymorph);
    }

    private void OnTrigger(EntityUid uid, RandomPolymorphOnTriggerComponent comp , ref TriggerEvent args)
    {
        if (args.User == null)
            return;

        RandomPolymorph(uid, comp.Polymorphs);

        args.Handled = true;
    }

    private void OnPolymorphActionEvent(ref RandomPolymorphActionEvent ev)
    {
        RandomPolymorph(ev.Performer, ev.Polymorphs);

        ev.Handled = true;
    }


}

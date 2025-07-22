using Content.Server.Charges;
using Content.Server.Light.Components;
using Content.Server.Light.EntitySystems;
using Content.Shared.Charges.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;
using Content.Shared.Timing;

namespace Content.Server._Harmony.LightBreaker;

public sealed class LightBreakerSystem : EntitySystem
{

    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly PoweredLightSystem _light = default!;
    [Dependency] private readonly ChargesSystem _charges = default!;
    [Dependency] private readonly UseDelaySystem _usedelay = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LightBreakerComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnUseInHand(EntityUid uid, LightBreakerComponent comp, UseInHandEvent args)
    {
        var lookup = _lookup.GetEntitiesInRange(uid, comp.Radius, LookupFlags.Approximate | LookupFlags.Static);

        if (_usedelay.IsDelayed(uid))
            return;

        if (_charges.IsEmpty(uid))
            return;

        foreach (var ent in lookup)
        {
            //break windows
            if (HasComp<PoweredLightComponent>(ent))
            {
                _light.TryDestroyBulb(ent);
            }



            args.Handled = true;
        }

        if (args.Handled)
        {
            _charges.TryUseCharge(uid);
            _usedelay.TryResetDelay(uid);
        }

    }
}

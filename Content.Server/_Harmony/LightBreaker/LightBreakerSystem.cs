using Content.Server.Charges;
using Content.Server.Light.Components;
using Content.Server.Light.EntitySystems;
using Content.Shared.Charges.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;

namespace Content.Server._Harmony.LightBreaker;

public sealed class LightBreakerSystem : EntitySystem
{

    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly PoweredLightSystem _light = default!;
    [Dependency] private readonly ChargesSystem _charges = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LightBreakerComponent, UseInHandEvent>(OnUseInHand);
    }

    private void OnUseInHand(EntityUid uid, LightBreakerComponent comp, UseInHandEvent args)
    {
        var lookup = _lookup.GetEntitiesInRange(uid, comp.Radius, LookupFlags.Approximate | LookupFlags.Static);

        if (TryComp<LimitedChargesComponent>(uid, out var limitedCharges))
        {

            _charges.TryUseCharge(uid);
        }

        foreach (var ent in lookup)
        {
            //break windows
            if (HasComp<PoweredLightComponent>(ent))
            {
                _light.TryDestroyBulb(ent);
            }
        }
    }
}

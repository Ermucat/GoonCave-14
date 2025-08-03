using Content.Shared._Harmony.Arcfiend;
using Content.Shared.Alert.Components;

namespace Content.Client._Harmony.Arcfiend;

public sealed class ArcfiendSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArcfiendComponent, GetGenericAlertCounterAmountEvent>(OnGetCounterAmount);
    }

    private void OnGetCounterAmount(Entity<ArcfiendComponent> ent, ref GetGenericAlertCounterAmountEvent args)
    {
        if (args.Handled)
            return;

        if (ent.Comp.PowerAlert != args.Alert)
            return;

        args.Amount = ent.Comp.Charge.Int();
    }
}

using Content.Server.Power.Components;
using Content.Shared.Alert;
using Content.Shared.Alert.Components;

namespace Content.Server._Harmony.Arcfiend;

public sealed class ArcfiendSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArcfiendComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, ArcfiendComponent component, MapInitEvent args)
    {

        UpdatePower(uid);

        Dirty(uid, component);
    }

    private void UpdatePower(EntityUid uid)
    {
        if (!TryComp<BatteryComponent>(uid, out var battery))
            return;

        if (!TryComp<ArcfiendComponent>(uid, out var arcfiend))
            return;

        arcfiend.Charge = battery.CurrentCharge;

        _alerts.ShowAlert(uid, arcfiend.PowerAlert);
    }
}

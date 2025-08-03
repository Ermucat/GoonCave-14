using System.ComponentModel;
using System.Diagnostics;
using Content.Server.Actions;
using Content.Server.Damage.Systems;
using Content.Server.Doors.Systems;
using Content.Server.Electrocution;
using Content.Server.Emp;
using Content.Server.Flash;
using Content.Server.Lightning;
using Content.Server.Ninja.Systems;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Server.Radio;
using Content.Server.Stunnable;
using Content.Server.Tesla.EntitySystems;
using Content.Shared._Harmony.Arcfiend;
using Content.Shared.Alert;
using Content.Shared.Alert.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Content.Shared.RepulseAttract;
using Content.Shared.Stunnable;
using Content.Shared.StatusEffectNew;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server._Harmony.Arcfiend;

public sealed class ArcfiendSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly BatteryDrainerSystem _batteryDrainer = default!;
    [Dependency] private readonly ElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly RepulseAttractSystem _repulseAttract = default!;
    [Dependency] private readonly SharedTransformSystem _xForm = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly FlashSystem _flash = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly DoorSystem _door = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly LightningSystem _lightningArc = default!;
    [Dependency] private readonly StunSystem _stun = default!;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArcfiendComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<ArcfiendComponent, OnBatteryDrained>(OnBatteryDrained);

        SubscribeLocalEvent<ArcfiendComponent, EmpPulseEvent>(OnEMP);

        SubscribeLocalEvent<ArcfiendDischargeEvent>(OnDischarge);
        SubscribeLocalEvent<ArcfiendFlashEvent>(OnFlash);
        SubscribeLocalEvent<ArcfiendJammerEvent>(OnJammer);
        SubscribeLocalEvent<ArcfiendArcBoltEvent>(OnArcBolt);

        SubscribeLocalEvent<RadioSendAttemptEvent>(OnRadioBlockAttempt);
    }

    private void OnComponentInit(EntityUid uid, ArcfiendComponent component, ComponentInit args)
    {
        _alerts.ShowAlert(uid, component.PowerAlert);

        if (!EnsureComp<BatteryComponent>(uid, out var battery))
            _battery.SetMaxCharge(uid, component.MaxCharge);

        if (!EnsureComp<BatteryDrainerComponent>(uid, out var drainer))
            _batteryDrainer.SetBattery(uid, uid);

        UpdatePower(uid);
    }

    private void OnBatteryDrained(EntityUid uid, ArcfiendComponent component, OnBatteryDrained args)
    {
        UpdatePower(uid);
    }

    private void UpdatePower(EntityUid uid)
    {
        if (!TryComp<BatteryComponent>(uid, out var battery))
            return;

        if (!TryComp<ArcfiendComponent>(uid, out var arcfiend))
            return;

        arcfiend.Charge = battery.CurrentCharge;

        _alerts.ShowAlert(uid, arcfiend.PowerAlert);

        Dirty(uid, arcfiend);
    }

    private void OnEMP(EntityUid uid, ArcfiendComponent component, EmpPulseEvent args)
    {
        _stun.TryParalyze(uid, component.Stuntime, refresh: false);

        var BoltCount = (component.Charge.Int() / 10);

        _lightningArc.ShootRandomLightnings(uid, 10, BoltCount, lightningPrototype: component.BoltPrototype ,triggerLightningEvents: false);

        UpdatePower(uid);
    }

    private void OnDischarge(ArcfiendDischargeEvent args)
    {

        if (!TryComp<ArcfiendComponent>(args.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(args.Performer, out var battery))
            return;

        if (battery.CurrentCharge <= args.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", args.Performer, args.Performer);
            return;
        }


        if (HasComp<DamageableComponent>(args.Target))
        {
            _electrocution.TryDoElectrocution(args.Target, args.Performer, 15, arcfiend.Stuntime, refresh: false, ignoreInsulation: true);
        }

        if (TryComp<AirlockComponent>(args.Target, out var airlock))
        {
            _door.TryOpen(args.Target);
            _popup.PopupEntity("Fryed the door open!", args.Performer, args.Performer);
        }

        _battery.UseCharge(args.Performer, args.Cost, battery);

        UpdatePower(args.Performer);

        args.Handled = true;
    }

    private void OnFlash(ArcfiendFlashEvent args)
    {
        if (!TryComp<ArcfiendComponent>(args.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(args.Performer, out var battery))
            return;

        if (battery.CurrentCharge <= args.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", args.Performer, args.Performer);
            return;
        }

        var coordinates = _xForm.GetMapCoordinates(args.Performer);

        var lookup = _lookup.GetEntitiesInRange(coordinates, 5);

        foreach (var target in lookup)
        {
            if (!HasComp<ArcfiendComponent>(target))
            {

                if (TryComp<StaminaComponent>(target, out var stamina))
                {
                    _stamina.TryTakeStamina(target, 50, stamina);
                }

                _flash.Flash(target, args.Performer, args.Performer, arcfiend.Stuntime, 0.5f);
            }
        }

        Spawn(args.FlashPrototype, coordinates);

        _repulseAttract.TryRepulseAttract(coordinates, args.Performer, 10, 5);

        _battery.UseCharge(args.Performer, args.Cost, battery);

        UpdatePower(args.Performer);
        args.Handled = true;
    }

    private void OnJammer(ArcfiendJammerEvent ev)
    {
        if (!TryComp<ArcfiendComponent>(ev.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(ev.Performer, out var battery))
            return;

        if (battery.CurrentCharge <= ev.Cost)
        {
            _popup.PopupEntity("Not enough energy", ev.Performer, ev.Performer);
            return;
        }

        _statusEffects.TryAddStatusEffectDuration(ev.Performer, ev.JammerEffect, ev.EffectTime);

        _battery.UseCharge(ev.Performer, ev.Cost, battery);

        UpdatePower(ev.Performer);

        ev.Handled = true;
    }

    private void OnRadioBlockAttempt(ref RadioSendAttemptEvent args)
    {
        if (ShouldCancelSend(args.RadioSource))
        {
            args.Cancelled = true;
        }
    }

    private bool ShouldCancelSend(EntityUid sourceUid)
    {
        var source = Transform(sourceUid).Coordinates;
        var query = EntityQueryEnumerator<RadioBlockerComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var blocker, out var transform))
        {
            if (_xForm.InRange(source, transform.Coordinates, 6))
            {
                return true;
            }
        }

        return false;
    }

    private void OnArcBolt(ArcfiendArcBoltEvent ev)
    {
        if (!TryComp<ArcfiendComponent>(ev.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(ev.Performer, out var battery))
            return;

        if (battery.CurrentCharge <= ev.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", ev.Performer, ev.Performer);
            return;
        }

        _lightningArc.ShootLightning(ev.Performer, ev.Target, ev.BoltPrototype);

        _battery.UseCharge(ev.Performer, ev.Cost, battery);

        UpdatePower(ev.Performer);

        ev.Handled = true;
    }
}

using System.ComponentModel;
using System.Diagnostics;
using Content.Server.Actions;
using Content.Server.Damage.Systems;
using Content.Server.Destructible.Thresholds;
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
using Content.Shared.Humanoid;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Ninja.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.RepulseAttract;
using Content.Shared.Stunnable;
using Content.Shared.StatusEffectNew;
using Content.Shared.Throwing;
using Robust.Server.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server._Harmony.Arcfiend;

public sealed class ArcfiendSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly BatterySystem _battery = default!;
    [Dependency] private readonly BatteryDrainerSystem _batteryDrainer = default!;
    [Dependency] private readonly ElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly RepulseAttractSystem _repulseAttract = default!;
    [Dependency] private readonly SharedTransformSystem _xForm = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly FlashSystem _flash = default!;
    [Dependency] private readonly StaminaSystem _stamina = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffects = default!;
    [Dependency] private readonly LightningSystem _lightningArc = default!;
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly SharedDoAfterSystem _doafter = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;



    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ArcfiendComponent, BeforeInteractHandEvent>(OnBeforeInteractHand);
        SubscribeLocalEvent<ArcfiendComponent, DoAfterAttemptEvent<DrainDoAfterEvent>>(OnDoAfterAttempt);
        SubscribeLocalEvent<ArcfiendComponent, DrainDoAfterEvent>(OnDoAfter);

        SubscribeLocalEvent<ArcfiendComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<ArcfiendComponent, OnBatteryDrained>(OnBatteryDrained);

        SubscribeLocalEvent<ArcfiendComponent, EmpPulseEvent>(OnEMP);

        SubscribeLocalEvent<ArcfiendDischargeEvent>(OnDischarge);
        SubscribeLocalEvent<ArcfiendFlashEvent>(OnFlash);
        SubscribeLocalEvent<ArcfiendJammerEvent>(OnJammer);
        SubscribeLocalEvent<ArcfiendArcBoltEvent>(OnArcBolt);

        SubscribeLocalEvent<RadioSendAttemptEvent>(OnRadioBlockAttempt);
    }

    private void OnBeforeInteractHand(Entity<ArcfiendComponent> ent, ref BeforeInteractHandEvent args)
    {
        // Harmony Start
        if (!TryComp<BatteryDrainerComponent>(ent, out var batteryDrainer))
            return;

        if (batteryDrainer.Draining == false)
            return;

        if (!_mobState.IsIncapacitated(args.Target))
            return;

        if (!HasComp<HumanoidAppearanceComponent>(args.Target))
            return;


        var doAfterArgs = new DoAfterArgs(EntityManager, ent, ent.Comp.DrainTime, new DrainDoAfterEvent(), target: args.Target, eventTarget: ent)
        {
            MovementThreshold = 0.5f,
            BreakOnMove = true,
            CancelDuplicate = false,
            AttemptFrequency = AttemptFrequency.StartAndEnd
        };

        _doafter.TryStartDoAfter(doAfterArgs);
    }

    protected void OnDoAfterAttempt(Entity<ArcfiendComponent> ent, ref DoAfterAttemptEvent<DrainDoAfterEvent> args)
    {
        if (!TryComp<BatteryComponent>(args.Event.User, out var batteryComponent))
            return;

        if (_battery.IsFull(args.Event.User, batteryComponent))
        {
            _popup.PopupEntity(Loc.GetString("arcfiend-energy-drain-full"), args.Event.User);
            args.Cancel();
            return;
        }

        if (!TryComp<DamageableComponent>(args.Event.Target, out var damageable))
            return;

        if (damageable.TotalDamage > 300)
        {
            _popup.PopupEntity(Loc.GetString("arcfiend-energy-drain-damaged"), args.Event.User);
            args.Cancel();
            return;
        }

        _damage.TryChangeDamage(args.Event.Target, ent.Comp.DrainDamage, ignoreResistances: true);
        _battery.ChangeCharge(args.Event.User, 10, batteryComponent);
        _popup.PopupEntity(Loc.GetString("arcfiend-energy-drain-success"), args.Event.User);
        _audio.PlayPvs(ent.Comp.SparkSound, ent);

        UpdatePower(args.Event.User);
    }

    private void OnDoAfter(Entity<ArcfiendComponent> ent, ref DrainDoAfterEvent args)
    {
        if (args.Cancelled || args.Handled || args.Target is not { } target)
            return;

        // repeat if there is still power to drain
        args.Repeat = true;
    }


    private void OnComponentInit(EntityUid uid, ArcfiendComponent component, ComponentInit args)
    {
        _alerts.ShowAlert(uid, component.PowerAlert);

        if (!EnsureComp<BatteryComponent>(uid, out var battery))
            _battery.SetMaxCharge(uid, component.MaxCharge);

        if (!EnsureComp<BatteryDrainerComponent>(uid, out var drainer))
        {
            _batteryDrainer.SetBattery(uid, uid);
            _alerts.ShowAlert(uid, component.DrainerAlert, 1);
        }


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
        _stun.TryAddParalyzeDuration(uid, component.Stuntime);

        var BoltCount = (component.Charge.Int() / 100);

        _lightningArc.ShootRandomLightnings(uid, 10, BoltCount, lightningPrototype: component.BoltPrototype , triggerLightningEvents: false);

        UpdatePower(uid);
    }

    private void OnDischarge(ArcfiendDischargeEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<ArcfiendComponent>(args.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(args.Performer, out var battery))
            return;

        if (battery.CurrentCharge < args.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", args.Performer, args.Performer);
            return;
        }

        args.Handled = true;

        if (HasComp<DamageableComponent>(args.Target))
        {
            _electrocution.TryDoElectrocution(args.Target, args.Performer, 15, args.Stuntime, refresh: false, ignoreInsulation: true);
        }

        if (TryComp<AirlockComponent>(args.Target, out var airlock))
        {
            _power.SetPowerDisabled(args.Target, true);
            _popup.PopupEntity("Fryed the doors power!", args.Performer, args.Performer);
        }

        _battery.UseCharge(args.Performer, args.Cost, battery);

        UpdatePower(args.Performer);
    }

    private void OnFlash(ArcfiendFlashEvent args)
    {
        if (args.Handled)
            return;

        if (!TryComp<ArcfiendComponent>(args.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(args.Performer, out var battery))
            return;

        if (battery.CurrentCharge < args.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", args.Performer, args.Performer);
            return;
        }

        args.Handled = true;

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

        _repulseAttract.TryRepulseAttract(coordinates, args.Performer, 1000, 5, args.PushWhitelist, CollisionGroup.GhostImpassable);

        _battery.UseCharge(args.Performer, args.Cost, battery);

        UpdatePower(args.Performer);
    }

    private void OnJammer(ArcfiendJammerEvent ev)
    {
        if (ev.Handled)
            return;

        if (!TryComp<ArcfiendComponent>(ev.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(ev.Performer, out var battery))
            return;

        if (battery.CurrentCharge < ev.Cost)
        {
            _popup.PopupEntity("Not enough energy", ev.Performer, ev.Performer);
            return;
        }

        ev.Handled = true;

        _statusEffects.TryAddStatusEffectDuration(ev.Performer, ev.JammerEffect, ev.EffectTime);

        _battery.UseCharge(ev.Performer, ev.Cost, battery);

        UpdatePower(ev.Performer);
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
            if (_xForm.InRange(source, transform.Coordinates, blocker.Range))
            {
                return true;
            }
        }

        return false;
    }

    private void OnArcBolt(ArcfiendArcBoltEvent ev)
    {
        if (ev.Handled)
            return;

        if (!TryComp<ArcfiendComponent>(ev.Performer, out var arcfiend))
            return;

        if (!TryComp<BatteryComponent>(ev.Performer, out var battery))
            return;

        if (battery.CurrentCharge < ev.Cost)
        {
            _popup.PopupEntity("Not enough energy to discharge", ev.Performer, ev.Performer);
            return;
        }
        ev.Handled = true;

        _lightningArc.ShootLightning(ev.Performer, ev.Target, ev.BoltPrototype);

        _battery.UseCharge(ev.Performer, ev.Cost, battery);

        UpdatePower(ev.Performer);
    }
}

using Content.Shared.Alert;
using Content.Shared.Inventory;
using Content.Shared.Mindshield.Components; // Harmony Change
using Content.Shared.Popups; // Harmony Change
using Content.Shared.Strip.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.Strip;

public sealed partial class ThievingSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alertsSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!; // Harmony

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ThievingComponent, BeforeStripEvent>(OnBeforeStrip);
        SubscribeLocalEvent<ThievingComponent, InventoryRelayedEvent<BeforeStripEvent>>((e, c, ev) =>
            OnBeforeStrip(e, c, ev.Args));
        SubscribeLocalEvent<ThievingComponent, ToggleThievingEvent>(OnToggleStealthy);
        SubscribeLocalEvent<ThievingComponent, ComponentInit>(OnCompInit);
        SubscribeLocalEvent<ThievingComponent, ComponentRemove>(OnCompRemoved);

        // Harmony Start
        SubscribeLocalEvent<MindShieldComponent, ComponentInit>(OnMindShieldImplanted);
        SubscribeLocalEvent<MindShieldComponent, ComponentShutdown>(OnMindShieldRemoved);
        // Harmony End
    }

    private void OnBeforeStrip(EntityUid uid, ThievingComponent component, BeforeStripEvent args)
    {
        args.Stealth |= component.Stealthy;
        if (args.Stealth)
        {
            args.Additive -= component.StripTimeReduction;
        }
    }

    private void OnCompInit(Entity<ThievingComponent> entity, ref ComponentInit args)
    {
        _alertsSystem.ShowAlert(entity, entity.Comp.StealthyAlertProtoId, 1);
    }

    private void OnCompRemoved(Entity<ThievingComponent> entity, ref ComponentRemove args)
    {
        _alertsSystem.ClearAlert(entity, entity.Comp.StealthyAlertProtoId);
    }

    private void OnToggleStealthy(Entity<ThievingComponent> ent, ref ToggleThievingEvent args)
    {
        if (args.Handled)
            return;

        // Harmony Start - doesnt allow you to reactivate thieving is mindshielded
        if (HasComp<MindShieldComponent>(ent) && ent.Comp.MindshieldBlock)
        {

            ent.Comp.Stealthy = false;
            _alertsSystem.ShowAlert(ent.Owner, ent.Comp.StealthyAlertProtoId, 2);
            _popup.PopupEntity(Loc.GetString("alerts-thieving-blocked"), ent, ent.Owner, PopupType.Medium);
            return;
        }
        // Harmony End


        ent.Comp.Stealthy = !ent.Comp.Stealthy;
        _alertsSystem.ShowAlert(ent.Owner, ent.Comp.StealthyAlertProtoId, (short)(ent.Comp.Stealthy ? 1 : 0));
        DirtyField(ent.AsNullable(), nameof(ent.Comp.Stealthy), null);

        args.Handled = true;
    }

    // Harmony Start - so I wanted to use mapInit but revolutionary system already uses it and it would be awkward if I just hid it in there
    private void OnMindShieldImplanted(EntityUid uid, MindShieldComponent comp, ComponentInit init)
    {
        if (!TryComp<ThievingComponent>(uid, out var thiefcomp))
            return;

        if (!thiefcomp.MindshieldBlock)
            return;

        thiefcomp!.Stealthy = false;
        _alertsSystem.ShowAlert(uid, thiefcomp.StealthyAlertProtoId, 2);
    }

    // So this should probably just be moved to mindshield system for general effects in the future, but this is what revolutionary system does so Im just gonna go off those guidelines
    private void OnMindShieldRemoved(EntityUid uid, MindShieldComponent comp, ComponentShutdown init)
    {
        if (!TryComp<ThievingComponent>(uid, out var thiefcomp))
            return;

        _alertsSystem.ShowAlert(uid, thiefcomp.StealthyAlertProtoId, (short)(thiefcomp.Stealthy ? 1 : 0));
    }
    // Harmony End
}

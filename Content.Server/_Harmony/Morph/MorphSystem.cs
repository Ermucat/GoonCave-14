using System.Numerics;
using Content.Server._Harmony.Morph.Components;
using Content.Server.Actions;
using Content.Server.Devour;
using Content.Server.GameTicking;
using Content.Server.Store.Systems;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Devour;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Eye;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Revenant;
using Content.Shared.Revenant.Components;
using Content.Shared.StatusEffect;
using Content.Shared.Store.Components;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Content.Shared.Popups;
using Robust.Shared.Random;
using Content.Shared._Harmony.Morph;
using Content.Server.Popups;

namespace Content.Server._Harmony.Morph;

public sealed partial class MorphSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly ActionsSystem _action = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MorphComponent, DevourDoAfterEvent>(OnMorphDevour);

        SubscribeLocalEvent<MorphComponent, MorphReplicateEvent>(OnMorphReplicate);

        SubscribeLocalEvent<MorphComponent, MapInitEvent>(OnMapInit);


    }


    private void OnMapInit(EntityUid uid, MorphComponent component, MapInitEvent args)
    {
        _action.AddAction(uid, ref component.Action, component.MorphAction);
    }

    public void ChangeBiomassAmount(FixedPoint2 amount,EntityUid uid, MorphComponent? component = null)
    {
        if (component != null)
        component.Biomass += amount;
        if (component != null)
            Dirty(uid, component);
    }


    public void OnMorphDevour(EntityUid uid , MorphComponent component, DevourDoAfterEvent arg)
    {
        var Biomass = typeof(BiomassComponent);


        if (TryComp<BiomassComponent>(arg.Target, out var amount))
        {
            string text = "fuck you bitch";

            _popupSystem.PopupEntity(text, uid, arg.User, PopupType.Small);
        }

        if (amount != null)
        ChangeBiomassAmount(amount.Amount , uid, component);
    }

    public void OnMorphReplicate(EntityUid uid , MorphComponent component, MorphReplicateEvent arg)
    {
        if (arg.Handled)
            return;

        if (!TryUseAbility(uid, component, component.ReplicateCost))
            return;

        var doafterArgs = new DoAfterArgs(EntityManager, arg.Performer, component.ReplicationDelay, new ReplicateDoAfterEvent(), uid, used: uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 0.5f,
        };


        _doAfterSystem.TryStartDoAfter(doafterArgs);

        Spawn(component.MorphPrototype);
    }

    private bool TryUseAbility(EntityUid uid, MorphComponent component, FixedPoint2 abilityCost)
    {
        if (component.Biomass <= abilityCost)
        {
            _popupSystem.PopupEntity(Loc.GetString("revenant-not-enough-essence"), uid, uid);
            return false;
        }


        ChangeBiomassAmount(-abilityCost, uid, component);

        return true;
    }
}

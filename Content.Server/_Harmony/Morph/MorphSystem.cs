using System.Collections;
using Content.Server._Harmony.Morph.Components;
using Content.Server.Actions;
using Content.Shared.Alert;
using Content.Shared.Devour;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared._Harmony.Morph;
using Content.Server.Popups;
using Content.Shared.Polymorph.Systems;
using Content.Shared.Polymorph.Components;
using Content.Server.GameTicking;
using Content.Server.Antag;
using Robust.Shared.Audio.Systems;
using Content.Shared.Examine;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Roles;
using Content.Shared.GameTicking.Components;
using Robust.Shared.Utility;

namespace Content.Server._Harmony.Morph;

public sealed partial class MorphSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly ActionsSystem _action = default!;
    [Dependency] private readonly SharedChameleonProjectorSystem _chamleon = default!;
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ExamineSystemShared _examine = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MorphComponent, DevourDoAfterEvent>(OnMorphDevour);
        SubscribeLocalEvent<MorphComponent, MorphReplicateEvent>(OnMorphReplicate);
        SubscribeLocalEvent<MorphComponent, ReplicateDoAfterEvent>(OnMorphReplicateDoAfter);
        SubscribeLocalEvent<MorphComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<ChameleonProjectorComponent, MorphEvent>(TryMorph);
        SubscribeLocalEvent<ChameleonDisguisedComponent, UnMorphEvent>(TryUnMorph);


        SubscribeLocalEvent<MorphDisguiseComponent, ExaminedEvent>(AddMorphExamine);
    }

    private void OnMapInit(EntityUid uid, MorphComponent component, MapInitEvent args)
    {
        _action.AddAction(uid, component.MorphReplicate);
        _action.AddAction(uid, component.Morph);
        _action.AddAction(uid, component.UnMorph);

        _alerts.ShowAlert(uid, component.BiomassAlert);
    }

    # region Actions

    public void ChangeBiomassAmount(FixedPoint2 amount, EntityUid uid, MorphComponent? component = null)
    {
        if (component != null)
        {
            component.Biomass += amount;
            Dirty(uid, component);
            _alerts.ShowAlert(uid, component.BiomassAlert);
        }
    }

    public void OnMorphDevour(EntityUid uid , MorphComponent component, DevourDoAfterEvent arg)
    {
        if (arg.Handled || arg.Cancelled)
            return;

        if (!TryComp<BiomassComponent>(arg.Target, out var amount))
        {
            _popupSystem.PopupEntity(Loc.GetString("morph-no-biomass-target"), uid, arg.User, PopupType.Medium);
        }
        else
        {
                ChangeBiomassAmount(amount.Amount , uid, component);
        }
    }

    public void OnMorphReplicate(EntityUid uid , MorphComponent component, MorphReplicateEvent arg)
    {
        if (component.Biomass <= component.ReplicateCost)
        {
            _popupSystem.PopupEntity(Loc.GetString("morph-no-biomass"), uid, arg.Performer, PopupType.Medium);

            return;
        }
        var doafterArgs = new DoAfterArgs(EntityManager, arg.Performer, component.ReplicationDelay, new ReplicateDoAfterEvent(), uid, used: uid)
        {
            BreakOnDamage = true,
            BreakOnMove = true,
            MovementThreshold = 0.5f,
        };



        _doAfterSystem.TryStartDoAfter(doafterArgs);

        _popupSystem.PopupEntity(Loc.GetString("morph-reproduce-start"), uid, arg.Performer, PopupType.Medium);

        arg.Handled = true;
    }

    public void OnMorphReplicateDoAfter(EntityUid uid , MorphComponent component, ReplicateDoAfterEvent arg)
    {
        if (arg.Handled || arg.Cancelled)
            return;

        var UserCoords = Transform(arg.User);
        var MorphSpawnCoords = UserCoords.Coordinates;


        arg.Handled = true;

        ChangeBiomassAmount(-(component.ReplicateCost), uid, component);

        Spawn(component.MorphPrototype, MorphSpawnCoords);
        _audio.PlayPvs(component.ReplicateSound, uid, null);
        MorphComponent.Children += 1;
    }
    # endregion

    # region Morph Disguise
    public void TryMorph(Entity<ChameleonProjectorComponent> ent, ref MorphEvent arg)
    {
        _chamleon.TryDisguise(ent, arg.Performer, arg.Target);
    }

    public void TryUnMorph(Entity<ChameleonDisguisedComponent> ent, ref UnMorphEvent arg)
    {
        _chamleon.TryReveal(ent!);
    }

    private void AddMorphExamine(EntityUid uid, MorphDisguiseComponent component, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString(component.ExamineMessage), component.Priority);
    }
    # endregion

}


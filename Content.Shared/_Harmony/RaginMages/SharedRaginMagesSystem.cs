using Content.Shared.Antag;
using Content.Shared.Popups;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Stunnable;
using Robust.Shared.GameStates;
using Robust.Shared.Player;

namespace Content.Shared._Harmony.RaginMages;

public abstract class SharedRaginMagesSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedStunSystem _sharedStun = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RaginMagesComponent, ComponentGetStateAttemptEvent>(OnWizCompGetStateAttempt);
        SubscribeLocalEvent<RaginMagesComponent, ComponentStartup>(DirtyWizComps);
    }


    /// <summary>
    /// Determines if a Wizard component should be sent to the client
    /// </summary>
    private void OnWizCompGetStateAttempt(EntityUid uid, RaginMagesComponent comp, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = !CanGetState(args.Player);
    }


    private bool CanGetState(ICommonSession? player)
    {
        if (player?.AttachedEntity is not {} uid)
            return true;

        if (HasComp<RaginMagesComponent>(uid))
            return true;

        return HasComp<RaginMagesComponent>(uid);
    }

    private void DirtyWizComps<T>(EntityUid someUid, T someComp, ComponentStartup ev)
    {
        var wizComps = AllEntityQuery<RaginMagesComponent>();
        while (wizComps.MoveNext(out var uid, out var comp))
        {
            Dirty(uid, comp);
        }
    }
}

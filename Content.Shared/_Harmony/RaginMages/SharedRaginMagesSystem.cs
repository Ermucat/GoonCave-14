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
        //SubscribeLocalEvent<ShowAntagIconsComponent, ComponentStartup>(DirtyWizComps);
    }


    /// <summary>
    /// Determines if a Rev component should be sent to the client.
    /// </summary>
    private void OnWizCompGetStateAttempt(EntityUid uid, RaginMagesComponent comp, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = !CanGetState(args.Player);
    }

    /// <summary>
    /// The criteria that determine whether a Rev/HeadRev component should be sent to a client.
    /// </summary>
    /// <param name="player"> The Player the component will be sent to.</param>
    /// <returns></returns>
    private bool CanGetState(ICommonSession? player)
    {
        //Apparently this can be null in replays so I am just returning true.
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

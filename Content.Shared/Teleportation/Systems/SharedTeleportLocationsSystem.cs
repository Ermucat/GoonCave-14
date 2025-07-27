using Content.Shared.Charges.Systems; // Harmony Change
using Content.Shared.Teleportation.Components;
using Content.Shared.Timing;
using Content.Shared.UserInterface;
using Content.Shared.Warps;

namespace Content.Shared.Teleportation.Systems;

/// <summary>
/// <inheritdoc cref="TeleportLocationsComponent"/>
/// </summary>
public abstract partial class SharedTeleportLocationsSystem : EntitySystem
{
    [Dependency] protected readonly UseDelaySystem Delay = default!;

    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;
    [Dependency] private readonly SharedChargesSystem _charges = default!; // Harmony Change

    protected const string TeleportDelay = "TeleportDelay";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TeleportLocationsComponent, ActivatableUIOpenAttemptEvent>(OnUiOpenAttempt);
        SubscribeLocalEvent<TeleportLocationsComponent, TeleportLocationDestinationMessage>(OnTeleportToLocationRequest);
    }

    private void OnUiOpenAttempt(Entity<TeleportLocationsComponent> ent, ref ActivatableUIOpenAttemptEvent args)
    {
        if (!Delay.IsDelayed(ent.Owner, TeleportDelay) && !_charges.IsEmpty(ent.Owner)) // Harmony Change - checks to see if it still has charges
            return;

        args.Cancel();
    }

    protected virtual void OnTeleportToLocationRequest(Entity<TeleportLocationsComponent> ent, ref TeleportLocationDestinationMessage args)
    {
        if (!TryGetEntity(args.NetEnt, out var telePointEnt) || TerminatingOrDeleted(telePointEnt) || !HasComp<WarpPointComponent>(telePointEnt) || Delay.IsDelayed(ent.Owner, TeleportDelay))
            return;

        // Harmony start - checks if item still has charges
        if (_charges.IsEmpty(ent.Owner))
            return;
        // Harmony End

        var comp = ent.Comp;
        var originEnt = args.Actor;
        var telePointXForm = Transform(telePointEnt.Value);

        SpawnAtPosition(comp.TeleportEffect, Transform(originEnt).Coordinates);

        _xform.SetMapCoordinates(originEnt, _xform.GetMapCoordinates(telePointEnt.Value, telePointXForm));

        SpawnAtPosition(comp.TeleportEffect, telePointXForm.Coordinates);

        Delay.TryResetDelay(ent.Owner, true, id: TeleportDelay);

        _charges.TryUseCharge(ent.Owner); // Harmony Change

        if (!ent.Comp.CloseAfterTeleport)
            return;

        // Teleport's done, now tell the BUI to close if needed.
        _ui.CloseUi(ent.Owner, TeleportLocationUiKey.Key);
    }
}

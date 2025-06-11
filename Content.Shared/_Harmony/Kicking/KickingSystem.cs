using Content.Shared._Harmony.Kicking.Components;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Systems;


namespace Content.Shared._Harmony.Kicking;

public sealed class KickingSystem : EntitySystem
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<KickingEvent>(OnKick);
    }
    private void OnKick(KickingEvent ev)
    {

        if (ev.Handled)
            return;

        ev.Handled = true;

        var direction = _transform.GetMapCoordinates(ev.Target, Transform(ev.Target)).Position - _transform.GetMapCoordinates(ev.Performer, Transform(ev.Performer)).Position;
        var impulseVector = direction * ev.KickPower;

        _physics.ApplyLinearImpulse(ev.Target, impulseVector);

        _stun.TrySlowdown(ev.Performer, ev.Slowtime, refresh: false, ev.SlowdownMultiplier, ev.SlowdownMultiplier);
        _stun.TryParalyze(ev.Target, ev.Stuntime, refresh: false);
    }
}

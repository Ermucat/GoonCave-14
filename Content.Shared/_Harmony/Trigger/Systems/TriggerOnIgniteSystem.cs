using Content.Server.Explosion.Components;
using Content.Shared.Atmos;
using Content.Shared.Trigger.Systems;

namespace Content.Shared._Harmony.Trigger.Systems;

public sealed class TriggerOnIgniteOpenSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;
    private void InitializeOnIgnite()
    {
        SubscribeLocalEvent<TriggerOnIgniteComponent, IgnitedEvent>(OnIgnited);
    }

    private void OnIgnited(Entity<TriggerOnIgniteComponent> ent, ref IgnitedEvent args)
    {
        _trigger.Trigger(ent.Owner);
    }
}

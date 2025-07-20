using Content.Server.Explosion.Components;
using Content.Shared.Atmos;

namespace Content.Server.Explosion.EntitySystems;

public sealed partial class TriggerSystem
{
    private void InitializeOnIgnite()
    {
        SubscribeLocalEvent<TriggerOnIgniteComponent, IgnitedEvent>(OnIgnited);
    }

    private void OnIgnited(Entity<TriggerOnIgniteComponent> ent, ref IgnitedEvent args)
    {
        Trigger(ent.Owner);
    }
}

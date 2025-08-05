using Content.Shared._Harmony.StatusEffect.Components;
using Content.Shared.Projectiles;
using Content.Shared.StatusEffectNew;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Shared._Harmony.StatusEffect;

public sealed class ModifyStatusEffectOnHitSystem : EntitySystem
{

    [Dependency] private readonly StatusEffectsSystem _statiseffect = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ModifyStatusEffectOnMeleeHitComponent, MeleeHitEvent>(OnMeleeHit);
        SubscribeLocalEvent<ModifyStatusEffectOnProjectileHitComponent, ProjectileHitEvent>(OnProjectileHit);
    }

    private void OnMeleeHit(Entity<ModifyStatusEffectOnMeleeHitComponent> ent, ref MeleeHitEvent args)
    {
        foreach (var entity in args.HitEntities)
        {
            _statiseffect.TryAddStatusEffectDuration(entity, ent.Comp.Effect, ent.Comp.Duration);
        }

    }

    private void OnProjectileHit(Entity<ModifyStatusEffectOnProjectileHitComponent> ent, ref ProjectileHitEvent args)
    {
        _statiseffect.TryAddStatusEffectDuration(args.Target, ent.Comp.Effect, ent.Comp.Duration);
    }
}

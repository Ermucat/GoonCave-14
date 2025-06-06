using Content.Client.Alerts;
using Content.Shared._Harmony.Morph;
using Content.Shared.Revenant;
using Robust.Client.GameObjects;

namespace Content.Client._Harmony.Morph;

public sealed class MorphSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MorphComponent, UpdateAlertSpriteEvent>(OnUpdateAlert);
    }


    private void OnUpdateAlert(Entity<MorphComponent> ent, ref UpdateAlertSpriteEvent args)
    {
        if (args.Alert.ID != ent.Comp.BiomassAlert)
            return;

        var biomass = Math.Clamp(ent.Comp.Biomass.Int(), 0, 999);
        _sprite.LayerSetRsiState(args.SpriteViewEnt.AsNullable(), MorphComponent.MorphVisualLayers.Digit1, $"{(biomass / 100) % 10}");
        _sprite.LayerSetRsiState(args.SpriteViewEnt.AsNullable(), MorphComponent.MorphVisualLayers.Digit2, $"{(biomass / 10) % 10}");
        _sprite.LayerSetRsiState(args.SpriteViewEnt.AsNullable(), MorphComponent.MorphVisualLayers.Digit3, $"{biomass % 10}");
    }
}

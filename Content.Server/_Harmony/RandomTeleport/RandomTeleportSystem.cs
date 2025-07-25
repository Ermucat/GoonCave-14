using System.Numerics;
using Content.Shared._Harmony.RandomTeleport;
using Content.Shared.Administration.Logs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server._Harmony.RandomTeleport;

public sealed class RandomTeleportSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly SharedTransformSystem _xform = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<RandomTeleportEvent>(OnRandomTeleport);
    }

    private void OnRandomTeleport(RandomTeleportEvent args)
    {
        if (args.Handled)
            return;

        var xform = Transform(args.Performer);
        var mapPos = _xform.GetWorldPosition(xform);
        var radius = args.Radius;
        var gridBounds = new Box2(mapPos - new Vector2(radius, radius), mapPos + new Vector2(radius, radius));

        var randomX = _random.NextFloat(gridBounds.Left, gridBounds.Right);
        var randomY = _random.NextFloat(gridBounds.Bottom, gridBounds.Top);

        var pos = new Vector2(randomX, randomY);

        _xform.SetWorldPosition(args.Performer, pos);


        MapCoordinates coords = _xform.GetMapCoordinates(args.Performer);

        Spawn(args.Effect, coords);

        _audio.PlayPvs(args.SoundSpecifier, args.Performer);

        args.Handled = true;
    }
}

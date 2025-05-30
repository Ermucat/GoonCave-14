using Content.Shared._Harmony.RaginMages;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client._Harmony.RaginMages;

/// <summary>
/// Used for the client to get status icons from other revs.
/// </summary>
public sealed class RaginMagesSystem : SharedRaginMagesSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RaginMagesComponent, GetStatusIconsEvent>(GetWizIcon);
    }

    private void GetWizIcon(Entity<RaginMagesComponent> ent, ref GetStatusIconsEvent args)
    {
        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }
}

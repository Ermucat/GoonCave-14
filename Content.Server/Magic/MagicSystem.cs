using Content.Server.Chat.Systems;
using Content.Server.Database.Migrations.Sqlite;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Polymorph.Systems;
using Content.Shared.Body.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Magic;
using Content.Shared.Magic.Events;
using Content.Shared.Mind;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Server.Magic;

public sealed class MagicSystem : SharedMagicSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;


    private static readonly ProtoId<TagPrototype> InvalidForSurvivorAntagTag = "InvalidForSurvivorAntag";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PolymorphItemEvent>(OnPolymorphItem);
    }

    public override void OnVoidApplause(VoidApplauseSpellEvent ev)
    {
        base.OnVoidApplause(ev);

        _chat.TryEmoteWithChat(ev.Performer, ev.Emote);

        var perfXForm = Transform(ev.Performer);
        var targetXForm = Transform(ev.Target);

        Spawn(ev.Effect, perfXForm.Coordinates);
        Spawn(ev.Effect, targetXForm.Coordinates);
    }

    protected override void OnRandomGlobalSpawnSpell(RandomGlobalSpawnSpellEvent ev)
    {
        base.OnRandomGlobalSpawnSpell(ev);

        if (!ev.MakeSurvivorAntagonist)
            return;

        if (_mind.TryGetMind(ev.Performer, out var mind, out _) && !_tag.HasTag(mind, InvalidForSurvivorAntagTag))
            _tag.AddTag(mind, InvalidForSurvivorAntagTag);

        EntProtoId survivorRule = "Survivor";

        if (!_gameTicker.IsGameRuleActive<SurvivorRuleComponent>())
            _gameTicker.StartGameRule(survivorRule);
    }

    public void OnPolymorphItem(PolymorphItemEvent ev)
    {
        if (HasComp<HandsComponent>(ev.Target))
        {

            if (!_hands.TryGetActiveItem(ev.Target, out var item))
                return;

            ev.Target = (EntityUid)item!;

            _polymorph.PolymorphEntity(ev.Target, ev.Polymorph);

            ev.Handled = true;

            return;
        }

        _polymorph.PolymorphEntity(ev.Target, ev.Polymorph);

        ev.Handled = true;
    }
}

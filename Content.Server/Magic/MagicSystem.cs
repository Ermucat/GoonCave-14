using Content.Server.Actions; // Harmony change
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules.Components;
// Harmony Start
using Content.Server.Ghost;
using Content.Shared._Harmony.ScryingOrb;
using Content.Shared.Hands;
// Harmony End
using Content.Shared.Magic;
using Content.Shared.Magic.Events;
using Content.Shared.Mind;
using Content.Shared.Tag;
using Content.Shared.Wieldable; // Harmony change
using Robust.Shared.Prototypes;

namespace Content.Server.Magic;

public sealed class MagicSystem : SharedMagicSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    // Harmony Start
    [Dependency] private readonly GhostSystem _ghost = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly SharedEyeSystem _eye = default!;
    // Harmony end

    private static readonly ProtoId<TagPrototype> InvalidForSurvivorAntagTag = "InvalidForSurvivorAntag";

    public override void Initialize()
    {
        base.Initialize();

        // Harmony Start
        SubscribeLocalEvent<ScryingOrbComponent, ItemWieldedEvent>(OnScryingOrbWielded);
        SubscribeLocalEvent<ScryingOrbComponent, GotEquippedHandEvent>(OnScryingOrbPickup);
        SubscribeLocalEvent<ScryingOrbComponent, GotUnequippedHandEvent>(OnScryingOrbDropped);
        SubscribeLocalEvent<ReturnToOwnerEvent>(OnScryingOrbReturn);
        // Harmony End

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
    // Harmony start
    #region Items

    private void OnScryingOrbWielded(EntityUid uid, ScryingOrbComponent spiritComponent, ref ItemWieldedEvent args)
    {
        _mind.TryGetMind(args.User, out var mindID, out var mind);

        var OwnerCoordinates = Transform(args.User);

        var Orb = Spawn(spiritComponent.ScryingOrbPrototype, OwnerCoordinates.Coordinates);

        _mind.Visit(mindID, Orb);

        _actions.AddAction(Orb, spiritComponent.ScryingOrbReturnAction);
    }

    private void OnScryingOrbReturn(ReturnToOwnerEvent ev)
    {
        _mind.TryGetMind(ev.Performer, out var mindID, out var mind);
        _mind.UnVisit(mindID, mind);

        EntityManager.DeleteEntity(ev.Performer);
    }

    private void OnScryingOrbPickup(EntityUid uid,ScryingOrbComponent comp, GotEquippedHandEvent ev)
    {
        _eye.SetDrawFov(ev.User, false);
    }

    private void OnScryingOrbDropped(EntityUid uid , ScryingOrbComponent comp, GotUnequippedHandEvent ev)
    {
        _eye.SetDrawFov(ev.User, true);
    }
    #endregion
    // Harmony end
}

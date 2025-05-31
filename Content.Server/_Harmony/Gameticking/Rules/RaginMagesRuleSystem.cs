using Content.Server._Harmony.GameTicking.Rules.Components;
using Content.Shared._Harmony.RaginMages;
using Content.Server.GameTicking.Rules;
using Content.Shared.Mobs;
using System.Linq;
using Content.Shared.Mobs.Components;
using Content.Server.RoundEnd;
using Robust.Shared.Timing;
using Content.Server.Antag;
using Content.Shared.GameTicking.Components;
using Content.Server.GameTicking;
using Content.Server.Chat.Systems;
using Timer = Robust.Shared.Timing.Timer;
using Robust.Shared.Random;

namespace Content.Server._Harmony.GameTicking.Rules;

public sealed class RaginMagesRuleSystem : GameRuleSystem<RaginMagesRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RaginMagesComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<RaginMagesComponent, MobStateChangedEvent>(OnMobStateChanged);
    }


    protected override void Started(EntityUid uid, RaginMagesRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {

        var message = Loc.GetString(component.Message);
        var title = Loc.GetString(component.Title);
        var color = component.Color;
        var sound = component.Sound;
        var cooldown = TimeSpan.FromSeconds(component.Cooldown);
        Timer.Spawn(cooldown,
            () =>
            {
                _chat.DispatchGlobalAnnouncement(message, title, true, sound, color);
            }
        );
    }

    protected override void AppendRoundEndText(EntityUid uid, RaginMagesRuleComponent component, GameRuleComponent gameRule, ref RoundEndTextAppendEvent args)
    {

        args.AddLine(Loc.GetString("raginmages-list-start"));

        var antags =_antag.GetAntagIdentifiers(uid);

        foreach (var (_, sessionData, name) in antags)
        {
            args.AddLine(Loc.GetString("raginmages-list-name-user", ("name", name), ("user", sessionData.UserName)));
        }
    }


    private void OnComponentRemove(EntityUid uid, RaginMagesComponent component, ComponentRemove args)
    {
        CheckRoundShouldEnd();
    }

    private void OnMobStateChanged(EntityUid uid, RaginMagesComponent component, MobStateChangedEvent ev)
    {
        if (ev.NewMobState == MobState.Dead)
            CheckRoundShouldEnd();
    }

    // This is more or less copied from NukeopsRuleSystem. All I know is that it works.
    private void CheckRoundShouldEnd()
    {
        var query = QueryActiveRules();
        while (query.MoveNext(out _, out _, out var wizard, out _))
        {
            CheckRoundShouldEnd(wizard);
        }
    }

    private void CheckRoundShouldEnd(RaginMagesRuleComponent wizardRule)
    {
        // Only checks if the mob with the wizard component is alive somewhere. Doesn't check where.
        var wizard = EntityQuery<RaginMagesComponent, MobStateComponent>(true);
        var wizardAlive = wizard
            .Any(wiz => wiz.Item2.CurrentState == MobState.Alive && wiz.Item1.Running);

        if (wizardAlive)
            return;

        _roundEndSystem.DoRoundEndBehavior(wizardRule.RoundEndBehavior,
            wizardRule.EvacShuttleTime,
            wizardRule.RoundEndTextSender,
            wizardRule.RoundEndTextShuttleCall,
            wizardRule.RoundEndTextAnnouncement);

        // Don't call multiple times
        wizardRule.RoundEndBehavior = RoundEndBehavior.Nothing;
        // Set the flag to schedule a sleeper event if the automatic shuttle is recalled to keep the round interesting post-wizard.

    }
}

using Content.Server._Harmony.GameTicking.Rules.Components;
using Content.Shared._Harmony.RaginMages;
using Content.Server.GameTicking.Rules;
using Content.Shared.Mobs;
using System.Linq;
using Content.Shared.Mobs.Components;
using Content.Server.RoundEnd;
using Robust.Shared.Timing;

namespace Content.Server._Harmony.GameTicking.Rules;

public sealed class RaginMagesRuleSystem : GameRuleSystem<RaginMagesRuleComponent>
{
    [Dependency] private readonly RoundEndSystem _roundEndSystem = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RaginMagesComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<RaginMagesComponent, MobStateChangedEvent>(OnMobStateChanged);
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

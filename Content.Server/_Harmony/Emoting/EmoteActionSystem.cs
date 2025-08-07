using Content.Server.Chat.Systems;
using Content.Shared._Harmony.Emoting;
using Content.Shared.Actions.Events;
using Content.Shared.Speech.Components;
using Content.Shared.Speech.EntitySystems;

namespace Content.Server._Harmony.Emoting;


public sealed class EmoteActionSystem : SharedSpeakOnActionSystem
{
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EmoteActionEvent>(OnActionPerformed);
    }

    private void OnActionPerformed(EmoteActionEvent args)
    {
        var user = args.Performer;

        if (string.IsNullOrWhiteSpace(args.Emote))
            return;

        _chat.TryEmoteWithoutChat(user, args.Emote);

        args.Handled = true;
    }
}

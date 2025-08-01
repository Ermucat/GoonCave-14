using System.Collections.Frozen;
using Content.Server.Popups;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Emoting;
using Content.Shared.Speech;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Chat.Systems;

// emotes using emote prototype
public partial class ChatSystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    private FrozenDictionary<string, EmotePrototype> _wordEmoteDict = FrozenDictionary<string, EmotePrototype>.Empty;

    protected override void OnPrototypeReload(PrototypesReloadedEventArgs obj)
    {
        base.OnPrototypeReload(obj);
        if (obj.WasModified<EmotePrototype>())
            CacheEmotes();
    }

    private void CacheEmotes()
    {
        var dict = new Dictionary<string, EmotePrototype>();
        var emotes = _prototypeManager.EnumeratePrototypes<EmotePrototype>();
        foreach (var emote in emotes)
        {
            foreach (var word in emote.ChatTriggers)
            {
                var lowerWord = word.ToLower();
                if (dict.TryGetValue(lowerWord, out var value))
                {
                    var errMsg = $"Duplicate of emote word {lowerWord} in emotes {emote.ID} and {value.ID}";
                    Log.Error(errMsg);
                    continue;
                }

                dict.Add(lowerWord, emote);
            }
        }

        _wordEmoteDict = dict.ToFrozenDictionary();
    }

    /// <summary>
    ///     Makes selected entity to emote using <see cref="EmotePrototype"/> and sends message to chat.
    /// </summary>
    /// <param name="source">The entity that is speaking</param>
    /// <param name="emoteId">The id of emote prototype. Should has valid <see cref="EmotePrototype.ChatMessages"/></param>
    /// <param name="hideLog">Whether or not this message should appear in the adminlog window</param>
    /// <param name="range">Conceptual range of transmission, if it shows in the chat window, if it shows to far-away ghosts or ghosts at all...</param>
    /// <param name="nameOverride">The name to use for the speaking entity. Usually this should just be modified via <see cref="TransformSpeakerNameEvent"/>. If this is set, the event will not get raised.</param>
    /// <param name="forceEmote">Bypasses whitelist/blacklist/availibility checks for if the entity can use this emote</param>
    /// <returns>True if an emote was performed. False if the emote is unvailable, cancelled, etc.</returns>
    public bool TryEmoteWithChat(
        EntityUid source,
        string emoteId,
        ChatTransmitRange range = ChatTransmitRange.Normal,
        bool hideLog = false,
        string? nameOverride = null,
        bool ignoreActionBlocker = false,
        bool forceEmote = false
        )
    {
        if (!_prototypeManager.TryIndex<EmotePrototype>(emoteId, out var proto))
            return false;
        return TryEmoteWithChat(source, proto, range, hideLog: hideLog, nameOverride, ignoreActionBlocker: ignoreActionBlocker, forceEmote: forceEmote);
    }

    /// <summary>
    ///     Makes selected entity to emote using <see cref="EmotePrototype"/> and sends message to chat.
    /// </summary>
    /// <param name="source">The entity that is speaking</param>
    /// <param name="emote">The emote prototype. Should has valid <see cref="EmotePrototype.ChatMessages"/></param>
    /// <param name="hideLog">Whether or not this message should appear in the adminlog window</param>
    /// <param name="hideChat">Whether or not this message should appear in the chat window</param>
    /// <param name="range">Conceptual range of transmission, if it shows in the chat window, if it shows to far-away ghosts or ghosts at all...</param>
    /// <param name="nameOverride">The name to use for the speaking entity. Usually this should just be modified via <see cref="TransformSpeakerNameEvent"/>. If this is set, the event will not get raised.</param>
    /// <param name="forceEmote">Bypasses whitelist/blacklist/availibility checks for if the entity can use this emote</param>
    /// <returns>True if an emote was performed. False if the emote is unvailable, cancelled, etc.</returns>
    public bool TryEmoteWithChat(
        EntityUid source,
        EmotePrototype emote,
        ChatTransmitRange range = ChatTransmitRange.Normal,
        bool hideLog = false,
        string? nameOverride = null,
        bool ignoreActionBlocker = false,
        bool forceEmote = false
    )
    {
        if (!forceEmote && !AllowedToUseEmote(source, emote))
            return false;

        var didEmote = TryEmoteWithoutChat(source, emote, ignoreActionBlocker);

        // check if proto has valid message for chat
        if (didEmote && emote.ChatMessages.Count != 0)
        {
            // not all emotes are loc'd, but for the ones that are we pass in entity
            var action = Loc.GetString(_random.Pick(emote.ChatMessages), ("entity", source));
            SendEntityEmote(source, action, range, nameOverride, hideLog: hideLog, checkEmote: false, ignoreActionBlocker: ignoreActionBlocker);
        }

        return didEmote;
    }

    /// <summary>
    ///     Makes selected entity to emote using <see cref="EmotePrototype"/> without sending any messages to chat.
    /// </summary>
    /// <returns>True if an emote was performed. False if the emote is unvailable, cancelled, etc.</returns>
    public bool TryEmoteWithoutChat(EntityUid uid, string emoteId, bool ignoreActionBlocker = false)
    {
        if (!_prototypeManager.TryIndex<EmotePrototype>(emoteId, out var proto))
            return false;

        return TryEmoteWithoutChat(uid, proto, ignoreActionBlocker);
    }

    /// <summary>
    ///     Makes selected entity to emote using <see cref="EmotePrototype"/> without sending any messages to chat.
    /// </summary>
    /// <returns>True if an emote was performed. False if the emote is unvailable, cancelled, etc.</returns>
    public bool TryEmoteWithoutChat(EntityUid uid, EmotePrototype proto, bool ignoreActionBlocker = false)
    {
        if (!_actionBlocker.CanEmote(uid) && !ignoreActionBlocker)
            return false;

        return TryInvokeEmoteEvent(uid, proto);
    }

    /// <summary>
    ///     Tries to find and play relevant emote sound in emote sounds collection.
    /// </summary>
    /// <returns>True if emote sound was played.</returns>
    public bool TryPlayEmoteSound(EntityUid uid, EmoteSoundsPrototype? proto, EmotePrototype emote, AudioParams? audioParams = null)
    {
        return TryPlayEmoteSound(uid, proto, emote.ID, audioParams);
    }

    /// <summary>
    ///     Tries to find and play relevant emote sound in emote sounds collection.
    /// </summary>
    /// <returns>True if emote sound was played.</returns>
    public bool TryPlayEmoteSound(EntityUid uid, EmoteSoundsPrototype? proto, string emoteId, AudioParams? audioParams = null)
    {
        if (proto == null)
            return false;

        // try to get specific sound for this emote
        if (!proto.Sounds.TryGetValue(emoteId, out var sound))
        {
            // no specific sound - check fallback
            sound = proto.FallbackSound;
            if (sound == null)
                return false;
        }

        // optional override params > general params for all sounds in set > individual sound params
        var param = audioParams ?? proto.GeneralParams ?? sound.Params;
        _audio.PlayPvs(sound, uid, param);
        return true;
    }
    /// <summary>
    /// Checks if a valid emote was typed, to play sounds and etc and invokes an event.
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="textInput"></param>
    /// <returns>True if the chat message should be displayed (because the emote was explicitly cancelled), false if it should not be.</returns>
    private bool TryEmoteChatInput(EntityUid uid, string textInput)
    {
        var actionTrimmedLower = TrimPunctuation(textInput.ToLower());
        if (!_wordEmoteDict.TryGetValue(actionTrimmedLower, out var emote))
            return true;

        if (!AllowedToUseEmote(uid, emote))
            return true;

        return TryInvokeEmoteEvent(uid, emote);

        static string TrimPunctuation(string textInput)
        {
            var trimEnd = textInput.Length;
            while (trimEnd > 0 && char.IsPunctuation(textInput[trimEnd - 1]))
            {
                trimEnd--;
            }

            var trimStart = 0;
            while (trimStart < trimEnd && char.IsPunctuation(textInput[trimStart]))
            {
                trimStart++;
            }

            return textInput[trimStart..trimEnd];
        }
    }
    /// <summary>
    /// Checks if we can use this emote based on the emotes whitelist, blacklist, and availibility to the entity.
    /// </summary>
    /// <param name="source">The entity that is speaking</param>
    /// <param name="emote">The emote being used</param>
    /// <returns></returns>
    private bool AllowedToUseEmote(EntityUid source, EmotePrototype emote)
    {
        // If emote is in AllowedEmotes, it will bypass whitelist and blacklist
        if (TryComp<SpeechComponent>(source, out var speech) &&
            speech.AllowedEmotes.Contains(emote.ID))
        {
            return true;
        }

        // Check the whitelist and blacklist
        if (_whitelistSystem.IsWhitelistFail(emote.Whitelist, source) ||
            _whitelistSystem.IsBlacklistPass(emote.Blacklist, source))
        {
            return false;
        }

        // Check if the emote is available for all
        if (!emote.Available)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates and raises <see cref="BeforeEmoteEvent"/> and then <see cref="EmoteEvent"/> to let other systems do things like play audio.
    /// In the case that the Before event is cancelled, EmoteEvent will NOT be raised, and will optionally show a message to the player
    /// explaining why the emote didn't happen.
    /// </summary>
    /// <param name="uid">The entity which is emoting</param>
    /// <param name="proto">The emote which is being performed</param>
    /// <returns>True if the emote was performed, false otherwise.</returns>
    private bool TryInvokeEmoteEvent(EntityUid uid, EmotePrototype proto)
    {
        var beforeEv = new BeforeEmoteEvent(uid, proto);
        RaiseLocalEvent(uid, ref beforeEv);

        if (beforeEv.Cancelled)
        {
            if (beforeEv.Blocker != null)
            {
                _popupSystem.PopupEntity(
                    Loc.GetString(
                        "chat-system-emote-cancelled-blocked",
                        ("emote", Loc.GetString(proto.Name).ToLower()),
                        ("blocker", beforeEv.Blocker.Value)
                    ),
                    uid,
                    uid
                );
            }
            else
            {
                _popupSystem.PopupEntity(
                    Loc.GetString("chat-system-emote-cancelled-generic",
                        ("emote", Loc.GetString(proto.Name).ToLower())),
                    uid,
                    uid
                );
            }

            return false;
        }

        var ev = new EmoteEvent(proto);
        // Harmony start - broadcast if animated
        if (proto.Animated)
            RaiseLocalEvent(uid, ref ev, true);
        else
            RaiseLocalEvent(uid, ref ev);
        // Harmony end

        return true;
    }
}

/// <summary>
///     Raised by chat system when entity made some emote.
///     Use it to play sound, change sprite or something else.
/// </summary>
[ByRefEvent]
public sealed class EmoteEvent : HandledEntityEventArgs
{
    public readonly EmotePrototype Emote;

    public EmoteEvent(EmotePrototype emote)
    {
        Emote = emote;
        Handled = false;
    }
}

using Content.Server.GameTicking.Rules;
using Content.Server.RoundEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._Harmony.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(RaginMagesRuleSystem))]
public sealed partial class RaginMagesRuleComponent : Component
{
    /// <summary>
    /// What will happen if the wizard dies.
    /// </summary>
    [DataField]
    public RoundEndBehavior RoundEndBehavior = RoundEndBehavior.ShuttleCall;

    /// <summary>
    /// Text sender for shuttle call if the wizard dies.
    /// </summary>
    [DataField]
    public string RoundEndTextSender = "comms-console-announcement-title-centcom";

    /// <summary>
    /// Text for shuttle call if the wizard dies.
    /// </summary>
    [DataField]
    public string RoundEndTextShuttleCall = "wizard-no-more-threat-announcement-shuttle-call";

    /// <summary>
    /// Text for shuttle call if the wizard dies. Used if shuttle is already called.
    /// </summary>
    [DataField]
    public string RoundEndTextAnnouncement = "wizard-no-more-threat-announcement";

    /// <summary>
    /// Time until the emergency shuttle arrives after the wizard dies.
    /// </summary>
    [DataField]
    public TimeSpan EvacShuttleTime = TimeSpan.FromMinutes(3);
}
